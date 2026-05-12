using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Threading;
using static PMD.PMD2X_Device;


namespace PMD
{
    
    using DeviceConfigStruct = DeviceConfigStructV0; // Use V0 as the current version

    public class PMD2X_Device : IPMD_Device
    {

        #region Statics

        public static List<PMD2X_Device> GetAllDevices(int speed = 1)
        {
            DeviceCounter = 0;

            List<PMD2X_Device> device_list = new List<PMD2X_Device>();
            // Open registry to find matching STM32 USB-Serial ports
            RegistryKey masterRegKey = null;

            try
            {
                masterRegKey = Registry.LocalMachine.OpenSubKey(STM32_USB_SERIAL_REGKEY);
                if (masterRegKey == null)
                {
                    throw new Exception("Couldn't open registry key STM32_USB_SERIAL_REGKEY:" + STM32_USB_SERIAL_REGKEY);
                }
            }
            catch
            {
                return device_list;
            }

            foreach (string subKey in masterRegKey.GetSubKeyNames())
            {
                // Name must contain either VCP or Serial to be valid. Process any entries NOT matching
                // Compare to subKey (name of RegKey entry)
                try
                {
                    RegistryKey subRegKey = masterRegKey.OpenSubKey($"{subKey}\\Device Parameters");
                    if (subRegKey == null) continue;

                    if (subRegKey.GetValueKind("PortName") != RegistryValueKind.String) continue;

                    string value = (string)subRegKey.GetValue("PortName");

                    if (value != null)
                    {
                        device_list.Add(new PMD2X_Device(value));
                    }
                }
                catch
                {
                    continue;
                }
            }
            masterRegKey.Close();

            return device_list;

        }

        public static int DeviceCounter = 0;

        private static UInt32 MCU_CMD_KEY = 0x0BADC0DE; // Key to unlock MCU CMDs

        #endregion

        #region Enums

        // USB interface commands
        private enum USB_CMD : byte
        { 
            CMD_WELCOME,
            CMD_READ_VENDOR_DATA,
            CMD_READ_UID,
            CMD_READ_DEVICE_DATA,
            CMD_READ_SENSOR_VALUES,
            CMD_WRITE_CONT_TX,
            CMD_READ_CONFIG,
            CMD_WRITE_CONFIG,
            CMD_RESET = 0xF0,
            CMD_BOOTLOADER = 0xF1,
            CMD_MCU_CMD = 0xF2,
            CMD_NOP = 0xFF
        }

        private enum MCU_CMD : byte
        {
            MCU_CMD_CONFIG_STORE,
            MCU_CMD_CONFIG_LOAD,
            MCU_CMD_CONFIG_RESET,
            MCU_CMD_RESET,
            MCU_CMD_BOOTLOADER,
            MCU_CMD_NONE = 0xFF
        };

        public enum AVG : byte
        {
            AVG_263_US,
            AVG_526_US,
            AVG_1_MS,
            AVG_2_MS,
            AVG_4_MS,
            AVG_8_MS,
            AVG_17_MS,
            AVG_34_MS,
            AVG_67_MS,
            AVG_135_MS,
            AVG_NUM
        }

        public enum OCP_SCALE : byte
        {
            OCP_SCALE_100,
            OCP_SCALE_110,
            OCP_SCALE_120,
            OCP_SCALE_130,
            OCP_SCALE_150,
            OCP_SCALE_170,
            OCP_SCALE_200,
            OCP_SCALE_OFF,
            OCP_SCALE_NUM
        }

        public enum DISPLAY_ROTATION : byte {
            DISPLAY_ROTATION_0,
            DISPLAY_ROTATION_180
        }

        #endregion

        private const int SENSOR_POWER_NUM = 16;
        private const int SENSOR_TS_NUM = 2;

        #region Structs
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct PowerSensor {
            public UInt16 Voltage;
            public UInt32 Current;
            public UInt32 Power;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SensorStruct
        {
            public UInt16 Vdd;
            public UInt16 Tchip;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SENSOR_TS_NUM)] public Int16[] Ts;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SENSOR_POWER_NUM)] public PowerSensor[] PowerReadings;
            public UInt16 EpsPower;
            public UInt16 PciePower;
            public UInt16 MbPower;
            public UInt16 TotalPower;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SENSOR_POWER_NUM)] public byte[] Ocp;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CalibrationValueStruct
        {
            public Int16 Offset;
            public Int16 GainOffset;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CalibrationStruct
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SENSOR_POWER_NUM)] public CalibrationValueStruct[] PowerReadingVoltage;  // Arrays for SENSOR_POWER_NUM
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SENSOR_POWER_NUM)] public CalibrationValueStruct[] PowerReadingCurrent;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DeviceConfigStructV0
        {
            public byte Version;               // uint8_t corresponds to C# byte
            public UInt16 Crc;                 // uint16_t corresponds to C# ushort
            public AVG Average;
            public OCP_SCALE OcpScale;
            DISPLAY_ROTATION DisplayRotation;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SENSOR_POWER_NUM)] public byte[] OcpPerChannel;       // Arrays for SENSOR_POWER_NUM
            public CalibrationStruct Calibration;
        }

        #endregion

        private const string STM32_USB_SERIAL_REGKEY = "SYSTEM\\CurrentControlSet\\Enum\\USB\\VID_0483&PID_5740";
        //private const byte PMD2_VID = 0xEE;
        //private const byte PMD2_PID = 0x0A;

        public event AllSensorsUpdatedEventHandler AllSensorsUpdated;

        public string Port { get; private set; }
        public int Id { get; private set; }
        public Guid Guid { get; private set; } = Guid.Empty;
        public string Name { get; private set; }
        public int FirmwareVersion { get; private set; }
        public int MonitoringInterval { get; set; } = 100;
        public List<Sensor> Sensors { get; private set; }

        private SerialPort serial_port;

        List<byte> rx_buffer = new List<byte>();

        Mutex DeviceMutex = new Mutex();
        private const int DeviceMutexTimeout = 500;

        private bool PMD2_SendCmd(byte cmd, int rx_len)
        {

            if (serial_port == null)
            {
                return false;
            }

            if (!DeviceMutex.WaitOne(DeviceMutexTimeout))
            {
                return false;
            }

            lock (rx_buffer) rx_buffer.Clear();

            try
            {
                serial_port.Write(new byte[] { cmd }, 0, 1);
            }
            catch
            {
                DeviceMutex.ReleaseMutex();
                return false;
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (rx_buffer.Count < rx_len && sw.ElapsedMilliseconds < 100)
            {

            }

            DeviceMutex.ReleaseMutex();

            return rx_buffer.Count == rx_len;
        }

        public PMD2X_Device(string port)
        {
            this.Port = port;

            Id = DeviceCounter++;
            Name = $"PMD2X-{Id}";
            int i = 0;

            /*string voltage_format = "{0:0.00}";
            string current_format = "{0:0.0}";
            string power_format = "{0:0}";*/

            string voltage_format = "F2";
            string current_format = "F1";
            string power_format = "F0";
            string temp_format = "F0";

            Sensors = new List<Sensor>() {
                new Sensor("Total Power", "POWER", power_format, "W"),
                new Sensor("EPS Power", "EPS", power_format, "W"),
                new Sensor("PCIE Power", "PCIE", power_format, "W"),
                new Sensor("MB Power", "MB", power_format, "W"),
                new Sensor("ATX 12V Voltage", "ATX12_V", voltage_format, "V"), new Sensor("ATX 12V Current", "ATX12_I", current_format, "A"), new Sensor("ATX 12V Power", "ATX12_P", power_format, "W"),
                new Sensor("ATX 5V Voltage", "ATX5_V", voltage_format, "V"), new Sensor("ATX 5V Current", "ATX5_I", current_format, "A"), new Sensor("ATX 5V Power", "ATX5_P", power_format, "W"),
                new Sensor("ATX 5VSB Voltage", "ATX5V_V", voltage_format, "V"), new Sensor("ATX 5VSB Current", "ATX5S_I", current_format, "A"), new Sensor("ATX 5VSB Power", "ATX5S_P", power_format, "W"),
                new Sensor("ATX 3V3 Voltage", "ATX3V_V", voltage_format, "V"), new Sensor("ATX 3V Current", "ATX3_I", current_format, "A"), new Sensor("ATX 3V3 Power", "ATX3_P", power_format, "W"),
                new Sensor("HPWR Voltage", "HPWR_V", voltage_format, "V"), new Sensor("HPWR Current", "PCIE3_I", current_format, "A"), new Sensor("HPWR Power", "PCIE3_P", power_format, "W"),
                new Sensor("EPS1 Voltage", "EPS1_V", voltage_format, "V"), new Sensor("EPS1 Current", "EPS1_I", current_format, "A"), new Sensor("EPS1 Power", "EPS1_P", power_format, "W"),
                new Sensor("EPS2 Voltage", "EPS2_V", voltage_format, "V"), new Sensor("EPS2 Current", "EPS2_I", current_format, "A"), new Sensor("EPS2 Power", "EPS2_P", power_format, "W"),
                new Sensor("PCIE1 Voltage", "PCIE1_V", voltage_format, "V"), new Sensor("PCIE1 Current", "PCIE1_I", current_format, "A"), new Sensor("PCIE1 Power", "PCIE1_P", power_format, "W"),
                new Sensor("PCIE2 Voltage", "PCIE2_V", voltage_format, "V"), new Sensor("PCIE2 Current", "PCIE2_I", current_format, "A"), new Sensor("PCIE2 Power", "PCIE2_P", power_format, "W"),
                new Sensor("PCIE3 Voltage", "PCIE3_V", voltage_format, "V"), new Sensor("PCIE3 Current", "PCIE3_I", current_format, "A"), new Sensor("PCIE3 Power", "PCIE3_P", power_format, "W"),
                new Sensor("HPWR Wire 1 Voltage", "HPWR_W1_V", voltage_format, "V"), new Sensor("HPWR Wire 1 Current", "HPWR_W1_I", current_format, "A"), new Sensor("HPWR Wire 1 Power", "HPWR_W1_P", power_format, "W"),
                new Sensor("HPWR Wire 2 Voltage", "HPWR_W2_V", voltage_format, "V"), new Sensor("HPWR Wire 2 Current", "HPWR_W2_I", current_format, "A"), new Sensor("HPWR Wire 2 Power", "HPWR_W2_P", power_format, "W"),
                new Sensor("HPWR Wire 3 Voltage", "HPWR_W3_V", voltage_format, "V"), new Sensor("HPWR Wire 3 Current", "HPWR_W3_I", current_format, "A"), new Sensor("HPWR Wire 3 Power", "HPWR_W3_P", power_format, "W"),
                new Sensor("HPWR Wire 4 Voltage", "HPWR_W4_V", voltage_format, "V"), new Sensor("HPWR Wire 4 Current", "HPWR_W4_I", current_format, "A"), new Sensor("HPWR Wire 4 Power", "HPWR_W4_P", power_format, "W"),
                new Sensor("HPWR Wire 5 Voltage", "HPWR_W5_V", voltage_format, "V"), new Sensor("HPWR Wire 5 Current", "HPWR_W5_I", current_format, "A"), new Sensor("HPWR Wire 5 Power", "HPWR_W5_P", power_format, "W"),
                new Sensor("HPWR Wire 6 Voltage", "HPWR_W6_V", voltage_format, "V"), new Sensor("HPWR Wire 6 Current", "HPWR_W6_I", current_format, "A"), new Sensor("HPWR Wire 6 Power", "HPWR_W6_P", power_format, "W"),
                new Sensor("HPWR Input Temperature", "HPWR_IN_T", temp_format, "°C"), new Sensor("HPWR Output Temperature", "HPWR_OUT_T", temp_format, "°C")
            };

            serial_port = new SerialPort(Port);

            serial_port.BaudRate = 115200;
            serial_port.Parity = Parity.None;
            serial_port.StopBits = StopBits.One;
            serial_port.DataBits = 8;

            serial_port.Handshake = Handshake.None;
            serial_port.ReadTimeout = 100;
            serial_port.WriteTimeout = 100;

            serial_port.RtsEnable = false;
            serial_port.DtrEnable = false;

            serial_port.DataReceived += Serial_port_DataReceived;

            // Check ID
            rx_buffer.Clear();

            serial_port.Open();
            serial_port.DiscardInBuffer();

            serial_port.RtsEnable = true;

            int timeout = 10;
            while (rx_buffer.Count < 15 && timeout-- > 1)
            {
                Thread.Sleep(10);
            }

            if (timeout != 0)
            {
                // Convert rx_buffer to ASCII string
                string str = System.Text.Encoding.ASCII.GetString(rx_buffer.ToArray()).TrimEnd('\0');
                if(str.Equals("ElmorLabs PMD2X"))
                {
                    // Read vendor data
                    if(PMD2_SendCmd((byte)USB_CMD.CMD_READ_VENDOR_DATA, 3))
                    {
                        byte vid = rx_buffer[0];
                        byte pid = rx_buffer[1];
                        byte fw = rx_buffer[2];

                        if(vid != 0xEE || pid != 0x17)
                        {
                            serial_port.Close();
                            throw new Exception("Device identification failed (VID/PID mismatch)");
                        }
                        FirmwareVersion = fw;
                    }
                    if (PMD2_SendCmd((byte)USB_CMD.CMD_READ_UID, 12))
                    {
                        byte[] guid_buffer = new byte[16];
                        for(i = 0; i < 12; i++)
                        {
                            guid_buffer[i+4] = rx_buffer[11 - i];
                        }
                        Guid = new Guid(guid_buffer);

                    }
                    serial_port.Close();
                    return;
                }
            }

            serial_port.Close();

            throw new Exception("Device identification failed");

        }

        public bool ReadConfig(out DeviceConfigStruct deviceConfig)
        {

            deviceConfig = new DeviceConfigStruct();

            // Get device config
            int config_struct_size = Marshal.SizeOf(typeof(DeviceConfigStruct));

            if(FirmwareVersion < 2)
            {
                config_struct_size = Marshal.SizeOf(typeof(DeviceConfigStructV0));
            }
            bool result = PMD2_SendCmd((byte)USB_CMD.CMD_READ_CONFIG, config_struct_size);

            if (result)
            {
                byte[] buffer = null;
                lock (rx_buffer)
                {
                    buffer = rx_buffer.ToArray();
                }

                try
                {
                    GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    deviceConfig = (DeviceConfigStruct)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(DeviceConfigStruct));
                    handle.Free();
                    return true;
                }
                catch { }
            }
            
            return false;

        }

        public bool WriteConfig(DeviceConfigStruct deviceConfig)
        {
            int config_struct_size = Marshal.SizeOf(typeof(DeviceConfigStruct));
            byte[] buffer;

            config_struct_size = Marshal.SizeOf(typeof(DeviceConfigStruct));
            buffer = new byte[config_struct_size];
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            Marshal.StructureToPtr(deviceConfig, handle.AddrOfPinnedObject(), false);
            handle.Free();

            // Send 62 bytes each time
            for (int i = 0; i < config_struct_size; i += 62)
            {
                int chunk_size = Math.Min(62, config_struct_size - i);
                byte[] tx_buffer = new byte[2 + chunk_size];
                tx_buffer[0] = (byte)USB_CMD.CMD_WRITE_CONFIG;
                tx_buffer[1] = (byte)i; // Offset
                Array.Copy(buffer, i, tx_buffer, 2, chunk_size);
                serial_port.Write(tx_buffer, 0, tx_buffer.Length);
            }

            return true;

        }

        private bool WriteMcuCmd(MCU_CMD cmd)
        {
            // Send command to load NVM
            byte[] tx_buffer = new byte[6];

            tx_buffer[0] = (byte)USB_CMD.CMD_MCU_CMD;
            tx_buffer[1] = (byte)(MCU_CMD_KEY & 0xFF); // 32-bit Key to unlock MCU CMDs
            tx_buffer[2] = (byte)((MCU_CMD_KEY >> 8) & 0xFF);
            tx_buffer[3] = (byte)((MCU_CMD_KEY >> 16) & 0xFF);
            tx_buffer[4] = (byte)((MCU_CMD_KEY >> 24) & 0xFF);
            tx_buffer[5] = (byte)cmd;

            serial_port.Write(tx_buffer, 0, tx_buffer.Length);

            return true;
        }

        public bool ConfigLoad()
        {
            return WriteMcuCmd(MCU_CMD.MCU_CMD_CONFIG_LOAD);
        }

        public bool ConfigStore()
        {
            return WriteMcuCmd(MCU_CMD.MCU_CMD_CONFIG_STORE);
        }

        private void Serial_port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int bytes = serial_port.BytesToRead;
            byte[] data_buffer = new byte[bytes];
            serial_port.Read(data_buffer, 0, bytes);
            lock (rx_buffer)
            {
                rx_buffer.AddRange(data_buffer);
            }
        }

        volatile bool run_task = false;
        Thread monitoring_thread = null;

        public bool StartMonitoring()
        {
            if(!DeviceMutex.WaitOne(DeviceMutexTimeout))
            {
                return false;
            }

            try
            {
                serial_port.RtsEnable = false;
                serial_port.Open();
                serial_port.DiscardInBuffer();

            }
            catch
            {
                DeviceMutex.ReleaseMutex();
                return false;
            }

            run_task = true;
            monitoring_thread = new Thread(new ThreadStart(update_task));
            monitoring_thread.IsBackground = true;
            monitoring_thread.Start();

            DeviceMutex.ReleaseMutex();

            return true;
        }

        public bool StopMonitoring()
        {
            if(!DeviceMutex.WaitOne(DeviceMutexTimeout))
            {
                return false;
            }

            run_task = false;
            if (monitoring_thread != null)
            {
                monitoring_thread.Join(MonitoringInterval * 2);
                monitoring_thread = null;
            }

            try
            {
                serial_port.Close();
            }
            catch
            {
                DeviceMutex.ReleaseMutex();
                return false;
            }

            DeviceMutex.ReleaseMutex();
            return true;
        }


        private void update_task()
        {

            while (run_task)
            {
                // Get sensor values
                int sensor_struct_size = Marshal.SizeOf(typeof(SensorStruct));
                bool result = PMD2_SendCmd((byte)USB_CMD.CMD_READ_SENSOR_VALUES, sensor_struct_size);

                if (result)
                {

                    byte[] buffer = null;
                    lock (rx_buffer)
                    {
                        buffer = rx_buffer.ToArray();
                    }

                    try
                    {
                        GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                        SensorStruct sensor_struct = (SensorStruct)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(SensorStruct));
                        handle.Free();

                        Sensors[0].Value = sensor_struct.TotalPower;
                        Sensors[1].Value = sensor_struct.EpsPower;
                        Sensors[2].Value = sensor_struct.PciePower;
                        Sensors[3].Value = sensor_struct.MbPower;

                        // ATX 12V
                        Sensors[4].Value = sensor_struct.PowerReadings[0].Voltage / 1000.0f;
                        Sensors[5].Value = sensor_struct.PowerReadings[0].Current / 1000.0f;
                        Sensors[6].Value = sensor_struct.PowerReadings[0].Power / 1000.0f;

                        // ATX 5V
                        Sensors[7].Value = sensor_struct.PowerReadings[1].Voltage / 1000.0f;
                        Sensors[8].Value = sensor_struct.PowerReadings[1].Current / 1000.0f;
                        Sensors[9].Value = sensor_struct.PowerReadings[1].Power / 1000.0f;

                        // ATX 5VSB
                        Sensors[10].Value = sensor_struct.PowerReadings[2].Voltage / 1000.0f;
                        Sensors[11].Value = sensor_struct.PowerReadings[2].Current / 1000.0f;
                        Sensors[12].Value = sensor_struct.PowerReadings[2].Power / 1000.0f;

                        // ATX 3V3
                        Sensors[13].Value = sensor_struct.PowerReadings[3].Voltage / 1000.0f;
                        Sensors[14].Value = sensor_struct.PowerReadings[3].Current / 1000.0f;
                        Sensors[15].Value = sensor_struct.PowerReadings[3].Power / 1000.0f;

                        // HPWR
                        Sensors[16].Value = sensor_struct.PowerReadings[4].Voltage / 1000.0f;
                        Sensors[17].Value = sensor_struct.PowerReadings[4].Current / 1000.0f;
                        Sensors[18].Value = sensor_struct.PowerReadings[4].Power / 1000.0f;

                        // EPS1
                        Sensors[19].Value = sensor_struct.PowerReadings[5].Voltage / 1000.0f;
                        Sensors[20].Value = sensor_struct.PowerReadings[5].Current / 1000.0f;
                        Sensors[21].Value = sensor_struct.PowerReadings[5].Power / 1000.0f;

                        // EPS2
                        Sensors[22].Value = sensor_struct.PowerReadings[6].Voltage / 1000.0f;
                        Sensors[23].Value = sensor_struct.PowerReadings[6].Current / 1000.0f;
                        Sensors[24].Value = sensor_struct.PowerReadings[6].Power / 1000.0f;

                        // PCIE1
                        Sensors[25].Value = sensor_struct.PowerReadings[7].Voltage / 1000.0f;
                        Sensors[26].Value = sensor_struct.PowerReadings[7].Current / 1000.0f;
                        Sensors[27].Value = sensor_struct.PowerReadings[7].Power / 1000.0f;

                        // PCIE2
                        Sensors[28].Value = sensor_struct.PowerReadings[8].Voltage / 1000.0f;
                        Sensors[29].Value = sensor_struct.PowerReadings[8].Current / 1000.0f;
                        Sensors[30].Value = sensor_struct.PowerReadings[8].Power / 1000.0f;

                        // PCIE3
                        Sensors[31].Value = sensor_struct.PowerReadings[9].Voltage / 1000.0f;
                        Sensors[32].Value = sensor_struct.PowerReadings[9].Current / 1000.0f;
                        Sensors[33].Value = sensor_struct.PowerReadings[9].Power / 1000.0f;

                        // HPWR W1
                        Sensors[34].Value = sensor_struct.PowerReadings[10].Voltage / 1000.0f;
                        Sensors[35].Value = sensor_struct.PowerReadings[10].Current / 1000.0f;
                        Sensors[36].Value = sensor_struct.PowerReadings[10].Power / 1000.0f;

                        // HPWR W2
                        Sensors[37].Value = sensor_struct.PowerReadings[11].Voltage / 1000.0f;
                        Sensors[38].Value = sensor_struct.PowerReadings[11].Current / 1000.0f;
                        Sensors[39].Value = sensor_struct.PowerReadings[11].Power / 1000.0f;

                        // HPWR W3
                        Sensors[40].Value = sensor_struct.PowerReadings[12].Voltage / 1000.0f;
                        Sensors[41].Value = sensor_struct.PowerReadings[12].Current / 1000.0f;
                        Sensors[42].Value = sensor_struct.PowerReadings[12].Power / 1000.0f;

                        // HPWR W4
                        Sensors[43].Value = sensor_struct.PowerReadings[13].Voltage / 1000.0f;
                        Sensors[44].Value = sensor_struct.PowerReadings[13].Current / 1000.0f;
                        Sensors[45].Value = sensor_struct.PowerReadings[13].Power / 1000.0f;

                        // HPWR W5
                        Sensors[46].Value = sensor_struct.PowerReadings[14].Voltage / 1000.0f;
                        Sensors[47].Value = sensor_struct.PowerReadings[14].Current / 1000.0f;
                        Sensors[48].Value = sensor_struct.PowerReadings[14].Power / 1000.0f;

                        // HPWR W6
                        Sensors[49].Value = sensor_struct.PowerReadings[15].Voltage / 1000.0f;
                        Sensors[50].Value = sensor_struct.PowerReadings[15].Current / 1000.0f;
                        Sensors[51].Value = sensor_struct.PowerReadings[15].Power / 1000.0f;

                        // HPWR Temp In
                        Sensors[52].Value = sensor_struct.Ts[0] / 10.0f;

                        // HPWR Temp Out
                        Sensors[53].Value = sensor_struct.Ts[1] / 10.0f;


                        AllSensorsUpdated?.Invoke();
                    }
                    catch { }

                }

                if (MonitoringInterval > 0) Thread.Sleep(MonitoringInterval);

            }
        }
    }
}