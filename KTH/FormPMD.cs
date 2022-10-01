using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KTH {
    public partial class FormPMD : Form {

        //private const string EVC2_PNPID = "USB\\VID_0483&PID_5740&MI_01";
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


        public static byte[] KTH_SendCmd(SerialPort serial_port, byte[] tx_buffer, int rx_len, bool delay) {

            if(serial_port == null) {
                return null;
            }

            byte[] rx_buffer = new byte[rx_len];
            try {
                serial_port.Write(tx_buffer, 0, tx_buffer.Length);
                if(delay) Thread.Sleep(1);
                serial_port.Read(rx_buffer, 0, rx_buffer.Length);
            } catch(Exception ex) {
                return null;
            }

            return rx_buffer;
        }

        List<MonitorGraph> graphList;
        ThreadStart thread_start;
        Thread task_thread;

        List<string> serial_ports;

        SerialPort serial_port = null;
        Mutex serial_port_mutex = new Mutex();

        DataLogger data_logger;

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
            int graph_offset_y = 10;
            int i = 0;

            // Add graphs
            graphList = new List<MonitorGraph>();

            //MonitorGraph monitor_graph = new MonitorGraph(0, "TC1", "°C", "F1", 0, 10, graph_width, graph_height, true, "");
            MonitorGraph monitor_graph = new MonitorGraph("PCIE1 Voltage", 1, "V", graph_width, graph_height);
            //monitor_graph.Location = new Point(0, graph_offset_y + (graph_height + graph_offset_y) * i++);
            //monitor_graph.MouseEnter += Monitor_graph_MouseEnter;
            //monitor_graph.MouseLeave += Monitor_graph_MouseLeave;
            //monitor_graph.MouseMove += Monitor_graph_MouseMove;
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);

            monitor_graph = new MonitorGraph("PCIE1 Current", 1, "A", graph_width, graph_height);
            //monitor_graph.Location = new Point(0, graph_offset_y + (graph_height + graph_offset_y) * i++);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);

            monitor_graph = new MonitorGraph("PCIE1 Power", 0, "W", graph_width, graph_height);
            //monitor_graph.Location = new Point(0, graph_offset_y + (graph_height + graph_offset_y) * i++);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);

            monitor_graph = new MonitorGraph("PCIE2 Voltage", 1, "V", graph_width, graph_height);
            //monitor_graph.Location = new Point(0, graph_offset_y + (graph_height + graph_offset_y)*i++);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);
            monitor_graph = new MonitorGraph("PCIE2 Current", 1, "A", graph_width, graph_height);
            //monitor_graph.Location = new Point(0, graph_offset_y + (graph_height + graph_offset_y) * i++);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);
            monitor_graph = new MonitorGraph("PCIE2 Power", 0, "W", graph_width, graph_height);
            //monitor_graph.Location = new Point(0, graph_offset_y + (graph_height + graph_offset_y) * i++);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);

            monitor_graph = new MonitorGraph("EPS1 Voltage", 1, "V", graph_width, graph_height);
            //monitor_graph.Location = new Point(0, graph_offset_y + (graph_height + graph_offset_y) * i++);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);
            monitor_graph = new MonitorGraph("EPS1 Current", 1, "A", graph_width, graph_height);
            //monitor_graph.Location = new Point(0, graph_offset_y + (graph_height + graph_offset_y) * i++);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);
            monitor_graph = new MonitorGraph("EPS1 Power", 0, "W", graph_width, graph_height);
            //monitor_graph.Location = new Point(0, graph_offset_y + (graph_height + graph_offset_y) * i++);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);

            monitor_graph = new MonitorGraph("EPS2 Voltage", 1, "V", graph_width, graph_height);
            //monitor_graph.Location = new Point(0, graph_offset_y + (graph_height + graph_offset_y) * i++);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);
            monitor_graph = new MonitorGraph("EPS2 Current", 1, "A", graph_width, graph_height);
            //monitor_graph.Location = new Point(0, graph_offset_y + (graph_height + graph_offset_y) * i++);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);
            monitor_graph = new MonitorGraph("EPS2 Power", 0, "W", graph_width, graph_height);
            //monitor_graph.Location = new Point(0, graph_offset_y + (graph_height + graph_offset_y) * i++);
            graphList.Add(monitor_graph);
            panelMonitoring.Controls.Add(monitor_graph);

            data_logger = new DataLogger();

            //UpdateConfigValues();
            //UpdateCalValues();

            thread_start = new ThreadStart(update_task);

            UpdateSerialPorts();

        }

        private void StartMonitoring() {
            run_task = true;
            task_thread = new Thread(thread_start);
            task_thread.IsBackground = true;
            task_thread.Start();
        }

        private void StopMonitoring() {
            run_task = false;
            task_thread.Join(500);
        }

        private void UpdateSerialPorts() {
            serial_ports = SerialPort.GetPortNames().ToList();

            comboBoxPorts.Items.Clear();

            // https://stackoverflow.com/questions/2837985/getting-serial-port-information
            using(var searcher = new ManagementObjectSearcher("SELECT * FROM WIN32_SerialPort")) {
                foreach(string port in serial_ports) {
                    bool found = false;
                    foreach(ManagementObject queryObj in searcher.Get()) {
                        if(queryObj["DeviceID"].ToString().Equals(port)) {
                            string pnp_dev_id = queryObj["PNPDeviceID"].ToString();
                            if(pnp_dev_id.StartsWith(CH340_PNPID)) {
                                comboBoxPorts.Items.Add(port + ": EVC2 Serial Port");
                                comboBoxPorts.SelectedIndex = comboBoxPorts.Items.Count - 1;
                            } else {
                                comboBoxPorts.Items.Add(port + ": " + queryObj["Description"].ToString());
                            }
                            found = true;
                        }
                    }
                    if(!found) {
                        comboBoxPorts.Items.Add(port + ": Unknown Serial Port");
                    }
                }
            }

            if(comboBoxPorts.SelectedIndex == -1) {
                comboBoxPorts.SelectedIndex = comboBoxPorts.Items.Count - 1;
            }
        }
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct ConfigStruct {
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

        private void UpdateConfigValues() {

            if(!serial_port_mutex.WaitOne(1000)) {
                MessageBox.Show("Couldn't get serial port mutex");
                return;
            }

            serial_port.DiscardInBuffer();

            // ID
            byte[] rx_buffer = KTH_SendCmd(serial_port, new byte[] { (byte)UART_CMD.UART_CMD_READ_ID }, 3, true);
            if(rx_buffer != null) {

                labelFwVerValue.Text = (rx_buffer[2]).ToString("X2");

                // Config
                rx_buffer = KTH_SendCmd(serial_port, new byte[] { (byte)UART_CMD.UART_CMD_READ_CONFIG }, Marshal.SizeOf(typeof(ConfigStruct)), true);
                if(rx_buffer != null) {
                    // Get struct
                    IntPtr ptr = Marshal.AllocHGlobal(rx_buffer.Length);
                    Marshal.Copy(rx_buffer, 0, ptr, rx_buffer.Length);

                    ConfigStruct config_struct = (ConfigStruct)Marshal.PtrToStructure(ptr, typeof(ConfigStruct));

                    Marshal.FreeHGlobal(ptr);

                    int adc_offset = (int)Marshal.OffsetOf(typeof(ConfigStruct), "AdcOffset");
                    byte[] crc_buf = new byte[rx_buffer.Length - adc_offset];
                    Array.Copy(rx_buffer, adc_offset, crc_buf, 0, rx_buffer.Length - adc_offset);

                    int crc16 = CRC16_Calc(crc_buf, crc_buf.Length);

                    //MessageBox.Show(crc16.ToString("X4") + " " + config_struct.Crc.ToString("X4"));

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
            }

            serial_port_mutex.ReleaseMutex();

        }


        private void WriteConfigValues(bool nvm) {

            return;

            ConfigStruct config_struct = new ConfigStruct();
            config_struct.Version = 4;

            config_struct.TimeoutAction = (byte) comboBoxTimeoutAction.SelectedIndex;

            UInt16 timeout_val;
            if(UInt16.TryParse(textBoxTimeoutDelay.Text, out timeout_val)) {
                config_struct.TimeoutCount = timeout_val;
            }

            config_struct.OledDisable = (byte) comboBoxOled.SelectedIndex;
            config_struct.OledRotation = (byte) comboBoxOledRotation.SelectedIndex;
            config_struct.OledSpeed = (byte) comboBoxDisplaySpeed.SelectedIndex;
            config_struct.Averaging = (byte) comboBoxAveraging.SelectedIndex;

            config_struct.AdcOffset = new sbyte[8];
            sbyte offset;
            if(sbyte.TryParse(textBoxPcie1Voffset.Text, out offset)) {
                config_struct.AdcOffset[0] = offset;
            }
            if(sbyte.TryParse(textBoxPcie1Ioffset.Text, out offset)) {
                config_struct.AdcOffset[1] = offset;
            }
            if(sbyte.TryParse(textBoxPcie2Voffset.Text, out offset)) {
                config_struct.AdcOffset[2] = offset;
            }
            if(sbyte.TryParse(textBoxPcie2Ioffset.Text, out offset)) {
                config_struct.AdcOffset[3] = offset;
            }
            if(sbyte.TryParse(textBoxEps1Voffset.Text, out offset)) {
                config_struct.AdcOffset[4] = offset;
            }
            if(sbyte.TryParse(textBoxEps1Ioffset.Text, out offset)) {
                config_struct.AdcOffset[5] = offset;
            }
            if(sbyte.TryParse(textBoxEps2Voffset.Text, out offset)) {
                config_struct.AdcOffset[6] = offset;
            }
            if(sbyte.TryParse(textBoxEps2Ioffset.Text, out offset)) {
                config_struct.AdcOffset[7] = offset;
            }

            //config_struct.UpdateConfigFlag = (byte) ( nvm ? 1 : 3);

            // Get struct
            int size = Marshal.SizeOf(config_struct);
            byte[] tx_buffer = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(config_struct, ptr, true);
            Marshal.Copy(ptr, tx_buffer, 0, size);
            Marshal.FreeHGlobal(ptr);

            int adc_offset = (int)Marshal.OffsetOf(typeof(ConfigStruct), "AdcOffset");
            byte[] crc_buf = new byte[size - adc_offset];
            Array.Copy(tx_buffer, adc_offset, crc_buf, 0, size - adc_offset);

            int crc16 = CRC16_Calc(crc_buf, crc_buf.Length);

            int crc_offset = (int)Marshal.OffsetOf(typeof(ConfigStruct), "Crc");
            tx_buffer[crc_offset] = (byte)crc16;
            tx_buffer[crc_offset+1] = (byte)(crc16>>8);

            if(!serial_port_mutex.WaitOne(1000)) {
                MessageBox.Show("Couldn't get serial port mutex");
                return;
            }

            serial_port.DiscardInBuffer();
            serial_port.DiscardOutBuffer();

            // Write config
            KTH_SendCmd(serial_port, new byte[] { (byte)UART_CMD.UART_CMD_WRITE_CONFIG }, 0, true);

            Thread.Sleep(10);

            KTH_SendCmd(serial_port, tx_buffer, 0, true);

            Thread.Sleep(10);

            serial_port_mutex.ReleaseMutex();

        }

        bool track = false;
        private void Monitor_graph_MouseMove(object sender, MouseEventArgs e) {
            if(track) {
                MonitorGraph monitor_graph = (MonitorGraph)sender;
                //monitor_graph.SetTrackX(e.X);
            }
        }

        private void Monitor_graph_MouseLeave(object sender, EventArgs e) {
            if(track) {
                MonitorGraph monitor_graph = (MonitorGraph)sender;
                //monitor_graph.SetTrackX(-1);
                track = false;
            }
        }

        private void Monitor_graph_MouseEnter(object sender, EventArgs e) {
            track = true;
        }

        static bool run_task;
        private void update_task() {
            //float[] temp = new float[2];

            byte[] rx_buffer;

            // Warmup
            if(serial_port_mutex.WaitOne(1000)) {
                for(int i = 0; i<2; i++) {
                    rx_buffer = KTH_SendCmd(serial_port, new byte[] { (byte)UART_CMD.UART_CMD_READ_SENSOR_VALUES }, 4 * 2 * 2, false);
                }
                serial_port_mutex.ReleaseMutex();

                Thread.Sleep(100);

                serial_port.DiscardInBuffer();
            }

            while(run_task) {

                rx_buffer = null;
                // Get sensor values
                if(serial_port_mutex.WaitOne(1000)) {
                    //serial_port.DiscardInBuffer();
                    rx_buffer = KTH_SendCmd(serial_port, new byte[] { (byte)UART_CMD.UART_CMD_READ_SENSOR_VALUES }, 4*2*2, false);
                    serial_port_mutex.ReleaseMutex();
                }

                if(rx_buffer != null) {
                    for(int i = 0; i < 4; i++) {
                        double voltage = ((Int16)(rx_buffer[i * 4 + 1] << 8 | rx_buffer[i * 4 + 0])) / 100.0;
                        double current = ((Int16)(rx_buffer[i * 4 + 2 + 1] << 8 | rx_buffer[i * 4 + 2 + 0])) / 10.0;
                        double power = voltage * current;
                        graphList[i * 3].Invoke((MethodInvoker)delegate {
                            graphList[i * 3].addValue(voltage);
                            graphList[i * 3 + 1].addValue(current);
                            graphList[i * 3 + 2].addValue(power);
                        });

                        if(csv_logging) {
                            data_logger.UpdateValue(i * 3 + 0, voltage);
                            data_logger.UpdateValue(i * 3 + 1, current);
                            data_logger.UpdateValue(i * 3 + 2, power);
                        }
                    }

                    if(csv_logging) {
                        data_logger.WriteLine();
                    }
                }

                Thread.Sleep(100);

/*
                if(WriteToFileName.Length > 0) {
                    try {
                        string text = "";
                        if(checkBoxTc1.Checked) {
                            text += temp[0].ToString("F1") + "°C " + (temp[0]*9/5f + 32).ToString("F1") + "°F" + Environment.NewLine;
                        }
                        if(checkBoxTc2.Checked) {
                            text += temp[1].ToString("F1") + "°C " + (temp[1] * 9 / 5f + 32).ToString("F1") + "°F" + Environment.NewLine;
                        }
                        System.IO.File.WriteAllText(WriteToFileName, text);
                    } catch(Exception ex) {

                    }
                }*/
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
            KTH_SendCmd(serial_port, new byte[] { (byte) UART_CMD.UART_CMD_RESET }, 0, true);
            serial_port_mutex.ReleaseMutex();
        }

        private void buttonBootloader_Click(object sender, EventArgs e) {
            if(!serial_port_mutex.WaitOne(1000)) {
                MessageBox.Show("Couldn't get serial port mutex");
                return;
            }
            KTH_SendCmd(serial_port, new byte[] { (byte)UART_CMD.UART_CMD_BOOTLOADER }, 0, true);
            serial_port_mutex.ReleaseMutex();
        }

        private void buttonStorecfg_Click(object sender, EventArgs e) {
            if(!serial_port_mutex.WaitOne(1000)) {
                MessageBox.Show("Couldn't get serial port mutex");
                return;
            }
            serial_port_mutex.ReleaseMutex();
        }

        private void buttonApply_Click(object sender, EventArgs e) {

            WriteConfigValues(false);
            UpdateConfigValues();

        }

        private void buttonChangeCalProfile_Click(object sender, EventArgs e) {
            if(!serial_port_mutex.WaitOne(1000)) {
                MessageBox.Show("Couldn't get serial port mutex");
                return;
            }
            serial_port_mutex.ReleaseMutex();

        }

        private void buttonCalOffset_Click(object sender, EventArgs e) {

            if(!serial_port_mutex.WaitOne(1000)) {
                MessageBox.Show("Couldn't get serial port mutex");
                return;
            }

            serial_port_mutex.ReleaseMutex();

        }

        private void buttonCalGain_Click(object sender, EventArgs e) {
            MessageBox.Show("Not yet implemented");
        }

        private void buttonApplyCal_Click(object sender, EventArgs e) {


        }

        private void buttonOpenPort_Click(object sender, EventArgs e) {
            if(serial_port == null || !serial_port.IsOpen) {
                if(comboBoxPorts.SelectedIndex >= serial_ports.Count) {
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
                byte[] rx_buffer = KTH_SendCmd(serial_port, new byte[] { (byte)UART_CMD.UART_CMD_READ_ID }, 3, true);

                serial_port_mutex.ReleaseMutex();

                if(rx_buffer == null || rx_buffer[0] != 0xEE || rx_buffer[1] != 0x0A) {
                    MessageBox.Show("Error communicating with PMD-USB"); 
                    try {
                        serial_port.Close();
                        serial_port.Dispose();
                        serial_port = null;
                    } catch(Exception ex) { }
                    return;
                }

                labelFwVerValue.Text = (rx_buffer[2]).ToString("X2");

                buttonOpenPort.Text = "Close";

                UpdateConfigValues();

                // Patch initial values
                StartMonitoring();

            } else {

                StopMonitoring();

                // Close serial port
                try {
                    serial_port.Close();
                } catch(Exception ex) {
                    MessageBox.Show("Error closing port: " + ex.Message);
                }

                try {
                    serial_port.Dispose();
                } catch(Exception ex) {

                }

                serial_port = null;

                buttonOpenPort.Text = "Open";
            }
        }

        private void buttonRefreshPorts_Click(object sender, EventArgs e) {
            UpdateSerialPorts();
        }

        bool csv_logging = false;
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

                        data_logger.WriteHeader();
                        csv_logging = true;
                        buttonLog.Text = "Stop logging";

                    } catch(Exception ex) {
                        MessageBox.Show(ex.Message, "Error");
                    }
                }
            } else {
                csv_logging = false;
                buttonLog.Text = "Log to CSV";
            }
        }

        private string WriteToFileName = "";

        private void buttonWriteToFile_Click(object sender, EventArgs e) {
            if(WriteToFileName.Length < 1) {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "txt files (*.txt)|*.txt";
                sfd.RestoreDirectory = true;
                if(sfd.ShowDialog() == DialogResult.OK) {
                    // Open and clear file
                    try {
                        System.IO.File.WriteAllText(sfd.FileName, "");
                        WriteToFileName = sfd.FileName;
                        buttonWriteToFile.Text = "Stop writing file";
                    } catch(Exception ex) {
                        MessageBox.Show(ex.Message, "Error");
                    }
                }
            } else {
                WriteToFileName = "";
                buttonWriteToFile.Text = "Write to file";

            }
        }

        private void FormPMD_Load(object sender, EventArgs e) {

        }

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
    }
}
