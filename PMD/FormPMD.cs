using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Resources;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace PMD {
    public partial class FormPMD : Form {

        private const string EVC2_PNPID = "USB\\VID_0483&PID_5740&MI_01";
        private const string CH340_PNPID = "USB\\VID_1A86&PID_7523";

        // UART interface commands

        enum UART_CMD : byte {
            UART_CMD_WELCOME,
            UART_CMD_READ_ID,
            UART_CMD_READ_SENSORS,
            UART_CMD_READ_SENSOR_VALUES,
            UART_CMD_READ_CONFIG,
            UART_CMD_WRITE_CONFIG,
            UART_CMD_READ_ADC,
            UART_CMD_WRITE_CONFIG_CONT_TX,
            UART_CMD_WRITE_CONFIG_UART,
            UART_CMD_RESET = 0xF0,
            UART_CMD_BOOTLOADER = 0xF1,
            UART_CMD_NOP = 0xFF
        };

        List<byte> rx_buffer = new List<byte>();

        private bool PMD_USB_SendCmd(UART_CMD cmd, int rx_len)
        {

            if (serial_port == null)
            {
                return false;
            }

            lock (rx_buffer) rx_buffer.Clear();
            serial_port.Write(new byte[] { (byte)cmd }, 0, 1);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (rx_buffer.Count < rx_len && sw.ElapsedMilliseconds < 100)
            {

            }

            return rx_buffer.Count == rx_len;
        }
        private bool PMD_USB_SendBuffer(byte[] tx_buffer, int rx_len)
        {

            if (serial_port == null)
            {
                return false;
            }

            lock (rx_buffer) rx_buffer.Clear();
            serial_port.Write(tx_buffer, 0, tx_buffer.Length);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (rx_buffer.Count < rx_len && sw.ElapsedMilliseconds < 100)
            {

            }

            return rx_buffer.Count == rx_len;
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


        List<MonitorGraph> graphList;
        ThreadStart thread_start;
        Thread task_thread;

        List<string> serial_ports;

        SerialPort serial_port = null;
        Mutex serial_port_mutex = new Mutex();

        DataLogger data_logger;

        int FirmwareVersion;

        public FormPMD() {

            InitializeComponent();

            // Update title
            this.Text = "PMD-USB " + Application.ProductVersion;

            // Populate options
            comboBoxTimeoutAction.Items.Add("Cycle");
            comboBoxTimeoutAction.Items.Add("OLED Off");
            comboBoxTimeoutAction.Items.Add("Disabled");
            comboBoxTimeoutAction.SelectedIndex = 0;

            comboBoxOled.Items.Add("On");
            comboBoxOled.Items.Add("Off");
            comboBoxOled.SelectedIndex = 1;

            comboBoxOledRotation.Items.Add("0 deg");
            comboBoxOledRotation.Items.Add("180 deg");
            comboBoxTimeoutAction.SelectedIndex = 0;

            comboBoxDisplaySpeed.Items.Add("0.0s");
            comboBoxDisplaySpeed.Items.Add("0.2s");
            comboBoxDisplaySpeed.Items.Add("0.4s");
            comboBoxDisplaySpeed.Items.Add("0.6s");
            comboBoxDisplaySpeed.Items.Add("0.8s");
            comboBoxDisplaySpeed.Items.Add("1.0s");
            comboBoxDisplaySpeed.Items.Add("1.2s");
            comboBoxDisplaySpeed.Items.Add("1.4s");
            comboBoxDisplaySpeed.Items.Add("1.4s");

            comboBoxAveraging.Items.Add("29µs (1 sample)");
            comboBoxAveraging.Items.Add("1.87ms (64 samples)");
            comboBoxAveraging.Items.Add("119ms (4096 samples)");
            comboBoxAveraging.Items.Add("0.95s (32768 samples)");

            int graph_height = 100;
            int graph_width = panelMonitoring.Width / 2 - 20;

            // Add graphs
            graphList = new List<MonitorGraph>();

            MonitorGraph monitor_graph = new MonitorGraph("PCIE1 Voltage", 2, "V", graph_width, graph_height);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);

            monitor_graph = new MonitorGraph("PCIE1 Current", 1, "A", graph_width, graph_height);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);

            monitor_graph = new MonitorGraph("PCIE1 Power", 0, "W", graph_width, graph_height);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);

            monitor_graph = new MonitorGraph("PCIE2 Voltage", 2, "V", graph_width, graph_height);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);
            monitor_graph = new MonitorGraph("PCIE2 Current", 1, "A", graph_width, graph_height);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);
            monitor_graph = new MonitorGraph("PCIE2 Power", 0, "W", graph_width, graph_height);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);

            monitor_graph = new MonitorGraph("EPS1 Voltage", 2, "V", graph_width, graph_height);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);
            monitor_graph = new MonitorGraph("EPS1 Current", 1, "A", graph_width, graph_height);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);
            monitor_graph = new MonitorGraph("EPS1 Power", 0, "W", graph_width, graph_height);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);

            monitor_graph = new MonitorGraph("EPS2 Voltage", 2, "V", graph_width, graph_height);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);
            monitor_graph = new MonitorGraph("EPS2 Current", 1, "A", graph_width, graph_height);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);
            monitor_graph = new MonitorGraph("EPS2 Power", 0, "W", graph_width, graph_height);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);

            monitor_graph = new MonitorGraph("GPU Power", 0, "W", graph_width, graph_height);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);

            monitor_graph = new MonitorGraph("CPU Power", 0, "W", graph_width, graph_height);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);

            monitor_graph = new MonitorGraph("Total Power", 0, "W", graph_width, graph_height);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);

            data_logger = new DataLogger();

            thread_start = new ThreadStart(update_task);

            UpdateSerialPorts();

        }

        private void StartMonitoring() {
            run_task = true;
            task_thread = new Thread(thread_start);
            task_thread.IsBackground = true;
            task_thread.Start();
        }

        private void StopMonitoring()
        {
            run_task = false;
            if (task_thread != null) {
                if(!task_thread.Join(500))
                {
                    return;
                }
            }
        }

        private void UpdateSerialPorts()
        {

            serial_ports = SerialPort.GetPortNames().ToList();

            comboBoxPorts.Items.Clear();

            // Open registry to find matching CH340 USB-Serial ports

            List<string> ch340_ports = new List<string>();
            RegistryKey masterRegKey = null;

            try
            {
                masterRegKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Enum\USB\VID_1A86&PID_7523");
            }
            catch
            {
            }

            if (masterRegKey != null)
            {
                foreach (string subKey in masterRegKey.GetSubKeyNames())
                {
                    // Name must contain either VCP or Serial to be valid. Process any entries NOT matching
                    // Compare to subKey (name of RegKey entry)
                    try
                    {
                        RegistryKey subRegKey = masterRegKey.OpenSubKey($"{subKey}\\Device Parameters");
                        if (subRegKey == null) continue;

                        string value = (string)subRegKey.GetValue("PortName");

                        if (subRegKey.GetValueKind("PortName") != RegistryValueKind.String) continue;

                        if (value != null) ch340_ports.Add(value);
                    }
                    catch
                    {
                        continue;
                    }
                }

                masterRegKey.Close();
            }

            // https://stackoverflow.com/questions/2837985/getting-serial-port-information
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM WIN32_SerialPort"))
            {
                foreach (string port in serial_ports)
                {
                    bool found = false;
                    if (ch340_ports.Contains(port))
                    {
                        comboBoxPorts.Items.Add(port + ": CH340 Serial Port");
                        comboBoxPorts.SelectedIndex = comboBoxPorts.Items.Count - 1;
                    }
                    else
                    {
                        foreach (ManagementObject queryObj in searcher.Get())
                        {
                            if (queryObj["DeviceID"].ToString().Equals(port))
                            {
                                string pnp_dev_id = queryObj["PNPDeviceID"].ToString();
                                if (pnp_dev_id.StartsWith(EVC2_PNPID))
                                {
                                    comboBoxPorts.Items.Add(port + ": EVC2 Serial Port");
                                    comboBoxPorts.SelectedIndex = comboBoxPorts.Items.Count - 1;
                                }
                                else
                                {
                                    comboBoxPorts.Items.Add(port + ": " + queryObj["Description"].ToString());
                                }
                                found = true;
                            }
                        }
                        if (!found)
                        {
                            comboBoxPorts.Items.Add(port + ": Unknown Serial Port");
                        }
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct ConfigStruct
        {
            public byte Version;
            public UInt16 Crc;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] public sbyte[] AdcOffset;
            public byte OledDisable;
            public UInt16 TimeoutCount;
            public byte TimeoutAction;
            public byte OledSpeed; //uint8_t rsvd[1]; // Padding on V1
            public byte RestartAdcFlag;
            public byte CalFlag;
            public byte UpdateConfigFlag;
            public byte OledRotation;
            public byte Averaging;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] public byte[] rsvd; // Padding on V2
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct ConfigStructV5
        {
            public byte Version;
            public UInt16 Crc;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] public sbyte[] AdcOffset;
            public byte OledDisable;
            public UInt16 TimeoutCount;
            public byte TimeoutAction;
            public byte OledSpeed; //uint8_t rsvd[1]; // Padding on V1
            public byte RestartAdcFlag;
            public byte CalFlag;
            public byte UpdateConfigFlag;
            public byte OledRotation;
            public byte Averaging;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] public sbyte[] AdcGainOffset;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] public byte[] rsvd; // Padding on V3
        }

        private void UpdateConfigValues() {

            if(!serial_port_mutex.WaitOne(1000)) {
                MessageBox.Show("Couldn't get serial port mutex");
                return;
            }

            serial_port.DiscardInBuffer();

            // ID
            if(PMD_USB_SendCmd(UART_CMD.UART_CMD_READ_ID, 3)) {

                FirmwareVersion = rx_buffer[2];

                labelFwVerValue.Text = (FirmwareVersion).ToString("X2");

                // Config

                if (FirmwareVersion < 6)
                {
                    if (PMD_USB_SendCmd(UART_CMD.UART_CMD_READ_CONFIG, Marshal.SizeOf(typeof(ConfigStruct)))) {

                        // Get struct
                        IntPtr ptr = Marshal.AllocHGlobal(rx_buffer.Count);
                        Marshal.Copy(rx_buffer.ToArray(), 0, ptr, rx_buffer.Count);

                        ConfigStruct config_struct = (ConfigStruct)Marshal.PtrToStructure(ptr, typeof(ConfigStruct));

                        Marshal.FreeHGlobal(ptr);

                        int crc_data_offset = (int)Marshal.OffsetOf(typeof(ConfigStruct), "AdcOffset");
                        int crc_data_length = (int)Marshal.OffsetOf(typeof(ConfigStruct), "rsvd") - crc_data_offset;
                        byte[] crc_buf = new byte[crc_data_length];
                        Array.Copy(rx_buffer.ToArray(), crc_data_offset, crc_buf, 0, crc_data_length);

                        int crc16 = CRC16_Calc(crc_buf, crc_buf.Length);

                        comboBoxTimeoutAction.SelectedIndex = config_struct.TimeoutAction;
                        textBoxTimeoutDelay.Text = config_struct.TimeoutCount.ToString();
                        comboBoxOled.SelectedIndex = config_struct.OledDisable;
                        comboBoxOledRotation.SelectedIndex = config_struct.OledRotation;
                        comboBoxDisplaySpeed.SelectedIndex = config_struct.OledSpeed;
                        comboBoxAveraging.SelectedIndex = config_struct.Averaging;

                        textBoxPcie1Voffset.Text = config_struct.AdcOffset[0].ToString();
                        textBoxPcie1Ioffset.Text = config_struct.AdcOffset[1].ToString();
                        textBoxPcie2Voffset.Text = config_struct.AdcOffset[2].ToString();
                        textBoxPcie2Ioffset.Text = config_struct.AdcOffset[3].ToString();
                        textBoxEps1Voffset.Text = config_struct.AdcOffset[4].ToString();
                        textBoxEps1Ioffset.Text = config_struct.AdcOffset[5].ToString();
                        textBoxEps2Voffset.Text = config_struct.AdcOffset[6].ToString();
                        textBoxEps2Ioffset.Text = config_struct.AdcOffset[7].ToString();

                    }
                } else
                {
                    if (PMD_USB_SendCmd(UART_CMD.UART_CMD_READ_CONFIG, Marshal.SizeOf(typeof(ConfigStructV5))))
                    {

                        // Get struct
                        IntPtr ptr = Marshal.AllocHGlobal(rx_buffer.Count);
                        Marshal.Copy(rx_buffer.ToArray(), 0, ptr, rx_buffer.Count);

                        ConfigStructV5 config_struct = (ConfigStructV5)Marshal.PtrToStructure(ptr, typeof(ConfigStructV5));

                        Marshal.FreeHGlobal(ptr);

                        int crc_data_offset = (int)Marshal.OffsetOf(typeof(ConfigStruct), "AdcOffset");
                        int crc_data_length = (int)Marshal.OffsetOf(typeof(ConfigStruct), "rsvd") - crc_data_offset;
                        byte[] crc_buf = new byte[crc_data_length];
                        Array.Copy(rx_buffer.ToArray(), crc_data_offset, crc_buf, 0, crc_data_length);

                        int crc16 = CRC16_Calc(crc_buf, crc_buf.Length);

                        comboBoxTimeoutAction.SelectedIndex = config_struct.TimeoutAction;
                        textBoxTimeoutDelay.Text = config_struct.TimeoutCount.ToString();
                        comboBoxOled.SelectedIndex = config_struct.OledDisable;
                        comboBoxOledRotation.SelectedIndex = config_struct.OledRotation;
                        comboBoxDisplaySpeed.SelectedIndex = config_struct.OledSpeed;
                        comboBoxAveraging.SelectedIndex = config_struct.Averaging;

                        textBoxPcie1Voffset.Text = config_struct.AdcOffset[0].ToString();
                        textBoxPcie1Ioffset.Text = config_struct.AdcOffset[1].ToString();
                        textBoxPcie2Voffset.Text = config_struct.AdcOffset[2].ToString();
                        textBoxPcie2Ioffset.Text = config_struct.AdcOffset[3].ToString();
                        textBoxEps1Voffset.Text = config_struct.AdcOffset[4].ToString();
                        textBoxEps1Ioffset.Text = config_struct.AdcOffset[5].ToString();
                        textBoxEps2Voffset.Text = config_struct.AdcOffset[6].ToString();
                        textBoxEps2Ioffset.Text = config_struct.AdcOffset[7].ToString();

                        textBoxPcie1Vgain.Text = config_struct.AdcGainOffset[0].ToString();
                        textBoxPcie1Igain.Text = config_struct.AdcGainOffset[1].ToString();
                        textBoxPcie2Vgain.Text = config_struct.AdcGainOffset[2].ToString();
                        textBoxPcie2Igain.Text = config_struct.AdcGainOffset[3].ToString();
                        textBoxEps1Vgain.Text = config_struct.AdcGainOffset[4].ToString();
                        textBoxEps1Igain.Text = config_struct.AdcGainOffset[5].ToString();
                        textBoxEps2Vgain.Text = config_struct.AdcGainOffset[6].ToString();
                        textBoxEps2Igain.Text = config_struct.AdcGainOffset[7].ToString();

                    }
                }
            }

            serial_port_mutex.ReleaseMutex();

        }

        private void WriteConfigValues(bool nvm) {

            if (FirmwareVersion < 05)
            {
                return;
            }

            if(serial_port == null)
            {
                return;
            }

            if (!serial_port_mutex.WaitOne(1000))
            {
                MessageBox.Show("Couldn't get serial port mutex");
                return;
            }

            //StopMonitoring();

            byte[] tx_buffer;

            if (FirmwareVersion < 6)
            {
                ConfigStruct config_struct = new ConfigStruct();
                config_struct.Version = 4;

                config_struct.TimeoutAction = (byte)comboBoxTimeoutAction.SelectedIndex;

                UInt16 timeout_val;
                if (UInt16.TryParse(textBoxTimeoutDelay.Text, out timeout_val))
                {
                    config_struct.TimeoutCount = timeout_val;
                }

                config_struct.OledDisable = (byte)comboBoxOled.SelectedIndex;
                config_struct.OledRotation = (byte)comboBoxOledRotation.SelectedIndex;
                config_struct.OledSpeed = (byte)comboBoxDisplaySpeed.SelectedIndex;
                config_struct.Averaging = (byte)comboBoxAveraging.SelectedIndex;

                config_struct.AdcOffset = new sbyte[8];
                sbyte offset;
                if (sbyte.TryParse(textBoxPcie1Voffset.Text, out offset))
                {
                    config_struct.AdcOffset[0] = offset;
                }
                if (sbyte.TryParse(textBoxPcie1Ioffset.Text, out offset))
                {
                    config_struct.AdcOffset[1] = offset;
                }
                if (sbyte.TryParse(textBoxPcie2Voffset.Text, out offset))
                {
                    config_struct.AdcOffset[2] = offset;
                }
                if (sbyte.TryParse(textBoxPcie2Ioffset.Text, out offset))
                {
                    config_struct.AdcOffset[3] = offset;
                }
                if (sbyte.TryParse(textBoxEps1Voffset.Text, out offset))
                {
                    config_struct.AdcOffset[4] = offset;
                }
                if (sbyte.TryParse(textBoxEps1Ioffset.Text, out offset))
                {
                    config_struct.AdcOffset[5] = offset;
                }
                if (sbyte.TryParse(textBoxEps2Voffset.Text, out offset))
                {
                    config_struct.AdcOffset[6] = offset;
                }
                if (sbyte.TryParse(textBoxEps2Ioffset.Text, out offset))
                {
                    config_struct.AdcOffset[7] = offset;
                }

                config_struct.UpdateConfigFlag = (byte)(nvm ? 1 : 3);

                // Get struct
                int size = Marshal.SizeOf(config_struct);
                tx_buffer = new byte[size];
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(config_struct, ptr, true);
                Marshal.Copy(ptr, tx_buffer, 0, size);
                Marshal.FreeHGlobal(ptr);

                int crc_data_offset = (int)Marshal.OffsetOf(typeof(ConfigStruct), "AdcOffset");
                int crc_data_length = (int)Marshal.OffsetOf(typeof(ConfigStruct), "rsvd") - crc_data_offset;
                byte[] crc_buf = new byte[crc_data_length];
                Array.Copy(tx_buffer, crc_data_offset, crc_buf, 0, crc_data_length);

                int crc16 = CRC16_Calc(crc_buf, crc_buf.Length);

                int crc_offset = (int)Marshal.OffsetOf(typeof(ConfigStruct), "Crc");
                tx_buffer[crc_offset] = (byte)crc16;
                tx_buffer[crc_offset + 1] = (byte)(crc16 >> 8);
            } else
            {
                ConfigStructV5 config_struct = new ConfigStructV5();
                config_struct.Version = 5;

                config_struct.TimeoutAction = (byte)comboBoxTimeoutAction.SelectedIndex;

                UInt16 timeout_val;
                if (UInt16.TryParse(textBoxTimeoutDelay.Text, out timeout_val))
                {
                    config_struct.TimeoutCount = timeout_val;
                }

                config_struct.OledDisable = (byte)comboBoxOled.SelectedIndex;
                config_struct.OledRotation = (byte)comboBoxOledRotation.SelectedIndex;
                config_struct.OledSpeed = (byte)comboBoxDisplaySpeed.SelectedIndex;
                config_struct.Averaging = (byte)comboBoxAveraging.SelectedIndex;

                config_struct.AdcOffset = new sbyte[8];
                config_struct.AdcGainOffset = new sbyte[8];

                if (sbyte.TryParse(textBoxPcie1Voffset.Text, out sbyte offset))
                {
                    config_struct.AdcOffset[0] = offset;
                }
                if (sbyte.TryParse(textBoxPcie1Ioffset.Text, out offset))
                {
                    config_struct.AdcOffset[1] = offset;
                }
                if (sbyte.TryParse(textBoxPcie2Voffset.Text, out offset))
                {
                    config_struct.AdcOffset[2] = offset;
                }
                if (sbyte.TryParse(textBoxPcie2Ioffset.Text, out offset))
                {
                    config_struct.AdcOffset[3] = offset;
                }
                if (sbyte.TryParse(textBoxEps1Voffset.Text, out offset))
                {
                    config_struct.AdcOffset[4] = offset;
                }
                if (sbyte.TryParse(textBoxEps1Ioffset.Text, out offset))
                {
                    config_struct.AdcOffset[5] = offset;
                }
                if (sbyte.TryParse(textBoxEps2Voffset.Text, out offset))
                {
                    config_struct.AdcOffset[6] = offset;
                }
                if (sbyte.TryParse(textBoxEps2Ioffset.Text, out offset))
                {
                    config_struct.AdcOffset[7] = offset;
                }

                if (sbyte.TryParse(textBoxPcie1Vgain.Text, out sbyte gain))
                {
                    config_struct.AdcGainOffset[0] = gain;
                }
                if (sbyte.TryParse(textBoxPcie1Igain.Text, out gain))
                {
                    config_struct.AdcGainOffset[1] = gain;
                }
                if (sbyte.TryParse(textBoxPcie2Vgain.Text, out gain))
                {
                    config_struct.AdcGainOffset[2] = gain;
                }
                if (sbyte.TryParse(textBoxPcie2Igain.Text, out gain))
                {
                    config_struct.AdcGainOffset[3] = gain;
                }
                if (sbyte.TryParse(textBoxEps1Vgain.Text, out gain))
                {
                    config_struct.AdcGainOffset[4] = gain;
                }
                if (sbyte.TryParse(textBoxEps1Igain.Text, out gain))
                {
                    config_struct.AdcGainOffset[5] = gain;
                }
                if (sbyte.TryParse(textBoxEps2Vgain.Text, out gain))
                {
                    config_struct.AdcGainOffset[6] = gain;
                }
                if (sbyte.TryParse(textBoxEps2Igain.Text, out gain))
                {
                    config_struct.AdcGainOffset[7] = gain;
                }

                config_struct.UpdateConfigFlag = (byte)(nvm ? 1 : 3);

                // Get struct
                int size = Marshal.SizeOf(config_struct);
                tx_buffer = new byte[size];
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(config_struct, ptr, true);
                Marshal.Copy(ptr, tx_buffer, 0, size);
                Marshal.FreeHGlobal(ptr);

                int crc_data_offset = (int)Marshal.OffsetOf(typeof(ConfigStructV5), "AdcOffset");
                int crc_data_length = (int)Marshal.OffsetOf(typeof(ConfigStructV5), "rsvd") - crc_data_offset;
                byte[] crc_buf = new byte[crc_data_length];
                Array.Copy(tx_buffer, crc_data_offset, crc_buf, 0, crc_data_length);

                int crc16 = CRC16_Calc(crc_buf, crc_buf.Length);

                int crc_offset = (int)Marshal.OffsetOf(typeof(ConfigStruct), "Crc");
                tx_buffer[crc_offset] = (byte)crc16;
                tx_buffer[crc_offset + 1] = (byte)(crc16 >> 8);
            }

            //Thread.Sleep(500);


            serial_port.DiscardInBuffer();
            serial_port.DiscardOutBuffer();

            // Write config
            PMD_USB_SendCmd(UART_CMD.UART_CMD_WRITE_CONFIG, 0);

            Thread.Sleep(10);

            PMD_USB_SendBuffer(tx_buffer, 0);

            Thread.Sleep(10);

            serial_port_mutex.ReleaseMutex();

            //StartMonitoring();

        }

        bool track = false;
        private void Monitor_graph_MouseMove(object sender, MouseEventArgs e) {
            if(track) {
                MonitorGraph monitor_graph = (MonitorGraph)sender;
            }
        }

        private void Monitor_graph_MouseLeave(object sender, EventArgs e) {
            if(track) {
                MonitorGraph monitor_graph = (MonitorGraph)sender;
                track = false;
            }
        }

        private void Monitor_graph_MouseEnter(object sender, EventArgs e) {
            track = true;
        }

        volatile bool run_task;
        private void update_task() {

            while (run_task) {

                bool result;

                // Get sensor values
                result = serial_port_mutex.WaitOne(100);

                if(result) 
                {
                    result = PMD_USB_SendCmd(UART_CMD.UART_CMD_READ_SENSOR_VALUES, 4 * 2 * 2);
                    byte[] local_rx_buffer = rx_buffer.ToArray();
                    serial_port_mutex.ReleaseMutex();

                    if(result)
                    {

                        double gpu_power = 0;
                        double cpu_power = 0;

                        for (int i = 0; i < 4; i++)
                        {
                            double voltage = ((Int16)(local_rx_buffer[i * 4 + 1] << 8 | local_rx_buffer[i * 4 + 0])) / 100.0;
                            double current = ((Int16)(local_rx_buffer[i * 4 + 2 + 1] << 8 | local_rx_buffer[i * 4 + 2 + 0])) / 10.0;
                            double power = voltage * current;

                            if (i < 2)
                            {
                                gpu_power += power;
                            }
                            else
                            {
                                cpu_power += power;
                            }

                            graphList[i * 3].Invoke((MethodInvoker)delegate
                            {
                                graphList[i * 3].addValue(voltage);
                                graphList[i * 3 + 1].addValue(current);
                                graphList[i * 3 + 2].addValue(power);
                            });

                            if (csv_logging)
                            {
                                data_logger.UpdateValue(i * 3 + 0, voltage);
                                data_logger.UpdateValue(i * 3 + 1, current);
                                data_logger.UpdateValue(i * 3 + 2, power);
                            }

                        }

                        double total_power = gpu_power + cpu_power;

                        graphList[4 * 3].Invoke((MethodInvoker)delegate
                        {
                            graphList[4 * 3].addValue(gpu_power);
                            graphList[4 * 3 + 1].addValue(cpu_power);
                            graphList[4 * 3 + 2].addValue(total_power);
                        });

                        if (csv_logging)
                        {
                            data_logger.UpdateValue(4 * 3 + 0, gpu_power);
                            data_logger.UpdateValue(4 * 3 + 1, cpu_power);
                            data_logger.UpdateValue(4 * 3 + 2, total_power);
                            data_logger.WriteLine();
                        }

                        if (csv_logging)
                        {
                            data_logger.WriteLine();
                        }

                        if (!string.IsNullOrEmpty(WriteToFileName))
                        {
                            try
                            {
                                File.WriteAllText(WriteToFileName, total_power.ToString("F0") + "W");
                            }
                            catch { }
                        }

                        Thread.Sleep(100);
                    }

                }

            }
        }


        private void FormKTH_FormClosing(object sender, FormClosingEventArgs e) {
            run_task = false;
        }

        private void buttonReset_Click(object sender, EventArgs e) {
            if(!serial_port_mutex.WaitOne(1000)) {
                MessageBox.Show("Couldn't get serial port mutex");
                return;
            }
            PMD_USB_SendCmd(UART_CMD.UART_CMD_RESET, 0);
            serial_port_mutex.ReleaseMutex();
        }

        private void buttonBootloader_Click(object sender, EventArgs e) {
            if(!serial_port_mutex.WaitOne(1000)) {
                MessageBox.Show("Couldn't get serial port mutex");
                return;
            }
            PMD_USB_SendCmd(UART_CMD.UART_CMD_BOOTLOADER, 0);
            serial_port_mutex.ReleaseMutex();
        }

        private void buttonStorecfg_Click(object sender, EventArgs e) {
            if(!serial_port_mutex.WaitOne(1000)) {
                MessageBox.Show("Couldn't get serial port mutex");
                return;
            }
            WriteConfigValues(true);
            UpdateConfigValues();
            serial_port_mutex.ReleaseMutex();
        }

        private void buttonApply_Click(object sender, EventArgs e) {

            WriteConfigValues(false);
            UpdateConfigValues();

        }

        private void buttonOpenPort_Click(object sender, EventArgs e) {
            if(serial_port == null || !serial_port.IsOpen) {
                if(comboBoxPorts.SelectedIndex >= serial_ports.Count || comboBoxPorts.SelectedIndex < 0) {
                    return;
                }

                try {

                    serial_port = new SerialPort(serial_ports[comboBoxPorts.SelectedIndex]);

                    serial_port.BaudRate = 115200;
                    serial_port.Parity = Parity.None;
                    serial_port.StopBits = StopBits.One;
                    serial_port.DataBits = 8;

                    serial_port.Handshake = Handshake.None;
                    serial_port.ReadTimeout = 100;
                    serial_port.WriteTimeout = 100;

                    serial_port.RtsEnable = true;
                    serial_port.DtrEnable = true;

                    serial_port.DataReceived += Serial_port_DataReceived;

                } catch(Exception ex) {
                    MessageBox.Show("Error initializing serial port: " + ex.Message);
                    serial_port = null;
                    buttonOpenPort.Text = "Open";
                    return;
                }

                try {
                    serial_port.Open();
                } catch(Exception ex) {
                    MessageBox.Show("Error opening port: " + ex.Message);
                    return;
                }

                // Check communication
                if(!serial_port_mutex.WaitOne(1000)) {
                    MessageBox.Show("Couldn't get serial port mutex");
                    return;
                }

                // Clear buffer
                serial_port.DiscardInBuffer();

                // ID
                bool result  = PMD_USB_SendCmd(UART_CMD.UART_CMD_READ_ID, 3);

                serial_port_mutex.ReleaseMutex();

                if(!result || rx_buffer[0] != 0xEE || rx_buffer[1] != 0x0A) {
                    MessageBox.Show("Error communicating with PMD-USB"); 
                    try {
                        ClosePort();
                    } catch(Exception ex) { }
                    return;
                }

                FirmwareVersion = rx_buffer[2];

                labelFwVerValue.Text = (FirmwareVersion).ToString("X2");

                buttonOpenPort.Text = "Close";

                if(FirmwareVersion != 06)
                {
                    if(MessageBox.Show("New firmware version 06 available, would you like to update?", "Notice", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        UpdateFirmware();

                        // Re-init device
                        ClosePort();
                        buttonOpenPort_Click(null, null);
                        return;
                    }
                } else
                {
                    buttonApplyConfig.Enabled = true;
                    buttonStorecfg.Enabled = true;
                }

                buttonWriteToFile.Enabled = true;

                UpdateConfigValues();

                // Patch initial values
                StartMonitoring();

                buttonFwu.Enabled = true;

            } else {
                ClosePort();
            }
        }

        private void ClosePort()
        {

            StopMonitoring();

            // Close serial port
            try
            {
                serial_port.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error closing port: " + ex.Message);
            }

            try
            {
                serial_port.Dispose();
            }
            catch (Exception ex)
            {

            }

            serial_port = null;

            buttonOpenPort.Text = "Open";
        }

        private void buttonRefreshPorts_Click(object sender, EventArgs e) {
            UpdateSerialPorts();
        }

        bool csv_logging = false;
        bool log_to_file = false;
        int tc1_log_id = -1;
        int tc2_log_id = -1;
        
        private void buttonLog_Click(object sender, EventArgs e) {
            if(!csv_logging) {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "csv files (*.csv)|*.csv";
                sfd.RestoreDirectory = true;
                if(sfd.ShowDialog() == DialogResult.OK) {
                    // Open and clear file
                    try {

                        data_logger = new DataLogger();

                        // Try to set file path
                        if(!data_logger.SetFilePath(sfd.FileName, false)) {
                            DialogResult dr = MessageBox.Show($"Overwrite {sfd.FileName}?", "File already exists", MessageBoxButtons.OKCancel);
                            if(dr != DialogResult.OK) {
                                // Cancel
                                return;
                            } else {
                                // Try again with overwrite
                                if(!data_logger.SetFilePath(sfd.FileName, true)) {
                                    MessageBox.Show("Error setting file path");
                                    return;
                                }
                            }
                        }

                        data_logger.AddLogItem("PCIE1 Voltage", "V");
                        data_logger.AddLogItem("PCIE1 Current", "A");
                        data_logger.AddLogItem("PCIE1 Power", "W");

                        data_logger.AddLogItem("PCIE2 Voltage", "V");
                        data_logger.AddLogItem("PCIE2 Current", "A");
                        data_logger.AddLogItem("PCIE2 Power", "W");

                        data_logger.AddLogItem("EPS1 Voltage", "V");
                        data_logger.AddLogItem("EPS1 Current", "A");
                        data_logger.AddLogItem("EPS1 Power", "W");

                        data_logger.AddLogItem("EPS2 Voltage", "V");
                        data_logger.AddLogItem("EPS2 Current", "A");
                        data_logger.AddLogItem("EPS2 Power", "W");

                        data_logger.AddLogItem("GPU Power", "W");
                        data_logger.AddLogItem("CPU Power", "W");
                        data_logger.AddLogItem("Total Power", "W");

                        data_logger.WriteHeader();
                        csv_logging = true;
                        buttonLog.Text = "Stop logging";

                    } catch(Exception ex) {
                        MessageBox.Show(ex.Message, "Error");
                    }
                }
            } else {
                data_logger.Commit();
                csv_logging = false;
                buttonLog.Text = "Log to CSV";
            }
        }

        private string WriteToFileName = "";

        private void buttonWriteToFile_Click(object sender, EventArgs e)
        {
            if (WriteToFileName.Length < 1)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "txt files (*.txt)|*.txt";
                sfd.RestoreDirectory = true;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    // Open and clear file
                    try
                    {
                        File.WriteAllText(sfd.FileName, "");
                        WriteToFileName = sfd.FileName;
                        buttonWriteToFile.Text = "Stop writing file";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error");
                    }
                }
            }
            else
            {
                WriteToFileName = "";
                buttonWriteToFile.Text = "Write to file";

            }
        }

        #region Firmware update

        // https://github.com/Microchip-MPLAB-Harmony/bootloader/blob/master/release_notes.md

        private const int BL_CMD_UNLOCK = 0xa0;
        private const int BL_CMD_DATA = 0xa1;
        private const int BL_CMD_VERIFY = 0xa2;
        private const int BL_CMD_RESET = 0xa3;
        private const int BL_CMD_BKSWAP_RESET = 0xa4;

        private const int BL_RESP_OK = 0x50;
        private const int BL_RESP_ERROR = 0x51;
        private const int BL_RESP_INVALID = 0x52;
        private const int BL_RESP_CRC_OK = 0x53;
        private const int BL_RESP_CRC_FAIL = 0x54;

        private const int BL_GUARD = 0x5048434D;

        private void UpdateFirmware()
        {
            if (!serial_port_mutex.WaitOne(1000))
            {
                MessageBox.Show("Couldn't get serial port mutex");
                return;
            }

            bool result = true;

            // Check firmware data
            int fw_max_size = (16 * 1024) - 2048 - 256;

            byte[] fw_data = new byte[fw_max_size];
            try
            {
                byte[] fw_data_temp = File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "PMD-06.bin"));
                if (fw_data_temp.Length > fw_data.Length)
                {
                    MessageBox.Show("Firmware data is too long");
                    result = false;
                }
                Array.Copy(fw_data_temp, 0, fw_data, 0, fw_data_temp.Length);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading firmware file: {ex.Message}");
                result = false;
            }

            // Check if we have KTH-USB

            // ID
            result = PMD_USB_SendCmd(UART_CMD.UART_CMD_READ_ID , 3);

            if (!result || rx_buffer[0] != 0xEE || rx_buffer[1] != 0x0A)
            {
                result = MessageBox.Show("Error checking device ID, would you like to continue? If you manually entered bootloader, press OK.", "Error", MessageBoxButtons.OKCancel) == DialogResult.OK;
            }

            if (result)
            {

                // Enter bootloader
                //MessageBox.Show("Entering bootloader...");
                PMD_USB_SendCmd(UART_CMD.UART_CMD_BOOTLOADER, 0);

                Thread.Sleep(1000);

                // Check bootloader

                int addr = 0x800;

                //MessageBox.Show("Unlocking bootloader...");
                byte[] tx_buffer = new byte[8];
                Array.Copy(uint32(addr), 0, tx_buffer, 0, 4);
                Array.Copy(uint32(fw_data.Length), 0, tx_buffer, 4, 4);

                result = send_request(serial_port, BL_CMD_UNLOCK, uint32(tx_buffer.Length), tx_buffer);

                // Send data
                for (int i = 0; i < fw_max_size / 256 && result; i++)
                {
                    //Console.Write($"Writing page {i}... ");
                    tx_buffer = new byte[256 + 4];
                    Array.Copy(uint32(addr + i * 256), 0, tx_buffer, 0, 4);
                    Array.Copy(fw_data, i * 256, tx_buffer, 4, 256);
                    result = send_request(serial_port, BL_CMD_DATA, uint32(tx_buffer.Length), tx_buffer);
                    
                }

                //Console.Write("Resetting device...");
                tx_buffer = new byte[16];
                result = send_request(serial_port, BL_CMD_RESET, uint32(tx_buffer.Length), tx_buffer);

                Thread.Sleep(1000);
                
            }

            if(result)
            {
                MessageBox.Show("Update finished. Please replug your device.");
            } else
            {
                MessageBox.Show("Error occurred during update");
            }

            serial_port_mutex.ReleaseMutex();
        }

        static UInt32[] crc32_tab_gen()
        {
            UInt32[] res = new UInt32[256];

            for (int i = 0; i < res.Length; i++)
            {
                UInt32 value = (UInt32)i;

                for (int j = 0; j < 8; j++)
                {
                    if ((value & 1) != 0)
                    {
                        value = (value >> 1) ^ 0xedb88320;
                    }
                    else
                    {
                        value = value >> 1;
                    }
                }
                res[i] = value;
            }
            return res;
        }

        static UInt32 crc32(UInt32[] tab, byte[] data)
        {
            UInt32 crc = 0xffffffff;

            for (int i = 0; i < data.Length; i++)
            {
                crc = tab[(crc ^ data[i]) & 0xff] ^ (crc >> 8);
            }
            return crc;
        }

        static byte[] uint32(int v)
        {
            return new byte[] { (byte)((v >> 0) & 0xff), (byte)((v >> 8) & 0xff), (byte)((v >> 16) & 0xff), (byte)((v >> 24) & 0xff) };
        }

        bool send_request(SerialPort serial_port, byte cmd, byte[] size, byte[] data)
        {

            byte[] bl_guard = uint32(BL_GUARD);

            byte[] tx_buffer = new byte[bl_guard.Length + 1 + size.Length + data.Length];

            Array.Copy(bl_guard, 0, tx_buffer, 0, bl_guard.Length);
            Array.Copy(size, 0, tx_buffer, bl_guard.Length, size.Length);
            tx_buffer[bl_guard.Length + size.Length] = cmd;
            if (data != null) Array.Copy(data, 0, tx_buffer, bl_guard.Length + size.Length + 1, data.Length);

            bool result = PMD_USB_SendBuffer(tx_buffer, 1);

            if (!result || rx_buffer[0] != BL_RESP_OK)
            {
                return false;
            }

            return true;
        }

        #endregion Firmware update

        private static UInt16 CRC16_Calc(byte[] data, int length) {
            byte x;
            UInt16 crc = 0xFFFF;

            //while(length-- > 0) {
            for(int i = 0; i < length; i++) {
                x = (byte)(crc >> 8 ^ data[i]);
                x ^= (byte)(x >> 4);
                crc = (UInt16)((crc << 8) ^ ((UInt16)(x << 12)) ^ ((UInt16)(x << 5)) ^ ((UInt16)x));
            }

            return crc;
        }

        private void buttonFwu_Click(object sender, EventArgs e)
        {
            if (serial_port != null) {
                if(MessageBox.Show("Are you sure you want to update firwamre?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    StopMonitoring();
                    Thread.Sleep(100);
                    serial_port.DiscardInBuffer();
                    UpdateFirmware();
                    ClosePort();
                    buttonOpenPort_Click(null, null);
                }
            }
        }
    }
}
