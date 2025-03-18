using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Threading;

namespace PMD
{
    public class Virtual_PMD_Device : IPMD_Device
    {

        #region Statics

        public static List<Virtual_PMD_Device> GetAllDevices(int speed = 1)
        {
            DeviceCounter = 0;

            List<Virtual_PMD_Device> device_list = new List<Virtual_PMD_Device> { new Virtual_PMD_Device("") };

            return device_list;

        }

        public static int DeviceCounter = 0;

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
            CMD_NVM_CONFIG = 0xF2,
            CMD_NOP = 0xFF
        }

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

        #endregion

        private const int SENSOR_POWER_NUM = 10;

        #region Structs
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct PowerSensor
        {
            public UInt16 Voltage;
            public UInt32 Current;
            public UInt32 Power;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SensorStruct
        {
            public UInt16 Vdd;
            public UInt16 Tchip;
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
        public struct DeviceConfigStruct
        {
            public byte Version;               // uint8_t corresponds to C# byte
            public UInt16 Crc;                 // uint16_t corresponds to C# ushort
            public AVG Average;
            public OCP_SCALE OcpScale;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SENSOR_POWER_NUM)] public byte[] OcpPerChannel;       // Arrays for SENSOR_POWER_NUM
            public CalibrationStruct Calibration;
        }
        #endregion

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

        public Virtual_PMD_Device(string port)
        {
            this.Port = port;

            Id = DeviceCounter++;
            Name = $"VirtualPMD2-{Id}";
            int i = 0;

            string voltage_format = "F2";
            string current_format = "F1";
            string power_format = "F0";

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
                new Sensor("PCIE3 Voltage", "PCIE3_V", voltage_format, "V"), new Sensor("PCIE3 Current", "PCIE3_I", current_format, "A"), new Sensor("PCIE3 Power", "PCIE3_P", power_format, "W")
            };

        }

        private DeviceConfigStruct _deviceConfig = new DeviceConfigStruct();
        public bool ReadConfig(out DeviceConfigStruct deviceConfig)
        {
            deviceConfig = _deviceConfig;
            return true;
        }

        public bool WriteConfig(DeviceConfigStruct deviceConfig)
        {
            _deviceConfig = deviceConfig;
            return true;
        }

        volatile bool run_task = false;
        Thread monitoring_thread = null;

        public bool StartMonitoring()
        {
           
            run_task = true;
            monitoring_thread = new Thread(new ThreadStart(update_task));
            monitoring_thread.IsBackground = true;
            monitoring_thread.Start();

            return true;
        }

        public bool StopMonitoring()
        {
           
            run_task = false;
            if (monitoring_thread != null)
            {
                monitoring_thread.Join(MonitoringInterval * 2);
                monitoring_thread = null;
            }

            return true;
        }


        Random rnd = new Random();
        private void update_task()
        {

            while (run_task)
            {

                // ATX 12V
                Sensors[4].Value = (12000.0f + rnd.Next(0, 100)) / 1000.0f;
                Sensors[5].Value = (10000.0f + rnd.Next(0, 100)) / 1000.0f;
                Sensors[6].Value = (Sensors[4].Value * Sensors[5].Value) / 1.0f;

                // ATX 5V
                Sensors[7].Value = (5000.0f + rnd.Next(0, 100)) / 1000.0f;
                Sensors[8].Value = (5000.0f + rnd.Next(0, 100)) / 1000.0f;
                Sensors[9].Value = (Sensors[7].Value * Sensors[8].Value) / 1.0f;

                // ATX 5VSB
                Sensors[10].Value = (5100.0f + rnd.Next(0, 100)) / 1000.0f;
                Sensors[11].Value = (500.0f + rnd.Next(0, 100)) / 1000.0f;
                Sensors[12].Value = (Sensors[10].Value * Sensors[11].Value) / 1.0f;

                // ATX 3V3
                Sensors[13].Value = (3300.0f + rnd.Next(0, 100)) / 1000.0f;
                Sensors[14].Value = (3000.0f + rnd.Next(0, 100)) / 1000.0f;
                Sensors[15].Value = (Sensors[13].Value * Sensors[14].Value) / 1.0f;

                // HPWR
                Sensors[16].Value = (12100.0f + rnd.Next(0, 100)) / 1000.0f;
                Sensors[17].Value = (10100.0f + rnd.Next(0, 100)) / 1000.0f;
                Sensors[18].Value = (Sensors[16].Value * Sensors[17].Value) / 1.0f;

                // EPS1
                Sensors[19].Value = (12200.0f + rnd.Next(0, 100)) / 1000.0f;
                Sensors[20].Value = (10200.0f + rnd.Next(0, 100)) / 1000.0f;
                Sensors[21].Value = (Sensors[19].Value * Sensors[20].Value) / 1.0f;

                // EPS2
                Sensors[22].Value = (12300.0f + rnd.Next(0, 100)) / 1000.0f;
                Sensors[23].Value = (10300.0f + rnd.Next(0, 100)) / 1000.0f;
                Sensors[24].Value = (Sensors[22].Value * Sensors[23].Value) / 1.0f;

                // PCIE1
                Sensors[25].Value = (12400.0f + rnd.Next(0, 100)) / 1000.0f;
                Sensors[26].Value = (10400.0f + rnd.Next(0, 100)) / 1000.0f;
                Sensors[27].Value = (Sensors[25].Value * Sensors[26].Value) / 1.0f;

                // PCIE2
                Sensors[28].Value = (12500.0f + rnd.Next(0, 100)) / 1000.0f;
                Sensors[29].Value = (10500.0f + rnd.Next(0, 100)) / 1000.0f;
                Sensors[30].Value = (Sensors[28].Value * Sensors[29].Value) / 1.0f;

                // PCIE3
                Sensors[31].Value = (12600.0f + rnd.Next(0, 100)) / 1000.0f;
                Sensors[32].Value = (10600.0f + rnd.Next(0, 100)) / 1000.0f;
                Sensors[33].Value = (Sensors[31].Value * Sensors[32].Value) / 1.0f;

                // MB Power
                Sensors[3].Value = Sensors[6].Value + Sensors[9].Value + Sensors[12].Value + Sensors[15].Value;
                // EPS Power
                Sensors[1].Value = Sensors[21].Value + Sensors[24].Value;
                // PCIE Power
                Sensors[2].Value = Sensors[27].Value + Sensors[30].Value + Sensors[33].Value;
                // Total Power
                Sensors[0].Value = Sensors[3].Value + Sensors[1].Value + Sensors[2].Value;

                AllSensorsUpdated?.Invoke();

                if (MonitoringInterval > 0) Thread.Sleep(MonitoringInterval);

            }
        }
    }
}