using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace PMD {
    public partial class FormPMD : Form {

        List<MonitorGraph> graphList = new List<MonitorGraph>();
        DataLogger data_logger = null;

        List<IPMD_Device> deviceList = new List<IPMD_Device>();
        IPMD_Device selectedDevice = null;

        public FormPMD() {

            InitializeComponent();

            // Update title
            this.Text = "PMD " + Application.ProductVersion;

        }

        private void StartMonitoring() {

            if (selectedDevice != null)
            {
                UpdateGraphTiming();
                selectedDevice.AllSensorsUpdated += SelectedDevice_AllSensorsUpdated;
                selectedDevice.StartMonitoring();
            }
        }
        DateTime lastMonitorGraphInvalidation = DateTime.Now;
        private void SelectedDevice_AllSensorsUpdated()
        {
            if (data_logger != null && data_logger.IsLogging)
            {
                data_logger.WriteEntry();
            }
            /*foreach (MonitorGraph monitor_graph in graphList)
            {
                monitor_graph.Redraw();
            }*/
            /*DateTime timeNow = DateTime.Now;
            if (timeNow - lastMonitorGraphInvalidation > TimeSpan.FromMilliseconds(99))
            {
                panelMonitoring.Invoke((MethodInvoker)delegate
                {
                    panelMonitoring.Invalidate(true);
                });
                lastMonitorGraphInvalidation = timeNow;
            }*/
            panelMonitoring.Invoke((MethodInvoker)delegate
            {
                panelMonitoring.Invalidate(true);
            });
        }

        private void StopMonitoring()
        {
            if (selectedDevice != null)
            {
                selectedDevice.StopMonitoring();
                selectedDevice.AllSensorsUpdated -= SelectedDevice_AllSensorsUpdated;
                graphList.Clear();
            }
        }


        volatile bool update_device_list_lock = false;
        private void UpdateDeviceList()
        {

            if (update_device_list_lock)
            {
                return;
            }

            update_device_list_lock = true;

            new System.Threading.Thread(() =>
            {
                deviceList.Clear();

                deviceList.AddRange(PMD2_Device.GetAllDevices());
                deviceList.AddRange(PMD_USB_Device.GetAllDevices());

#if DEBUG
                deviceList.AddRange(Virtual_PMD_Device.GetAllDevices());
#endif
                comboBoxPorts.Invoke((MethodInvoker)delegate
                {
                    comboBoxPorts.Items.Clear();

                    foreach (IPMD_Device device in deviceList)
                    {

                        string description = device.Name;

                        if (device is PMD2_Device pmd2_device)
                        {
                            description += $" ({pmd2_device.Port})";
                        }
                        else if (device is PMD_USB_Device pmd_usb_device)
                        {
                            description += $" ({pmd_usb_device.Port})";
                        }

                        comboBoxPorts.Items.Add(description);

                    }

                    if (comboBoxPorts.Items.Count > 0)
                    {
                        comboBoxPorts.SelectedIndex = 0;
                    }
                });

                update_device_list_lock = false;

            }).Start();

        }

        bool track = false;
        private void Monitor_graph_MouseMove(object sender, MouseEventArgs e) {
            if (track) {
                MonitorGraph monitor_graph = (MonitorGraph)sender;
            }
        }

        private void Monitor_graph_MouseLeave(object sender, EventArgs e) {
            if (track) {
                MonitorGraph monitor_graph = (MonitorGraph)sender;
                track = false;
            }
        }

        private void Monitor_graph_MouseEnter(object sender, EventArgs e) {
            track = true;
        }

        private void FormKTH_FormClosing(object sender, FormClosingEventArgs e) {
            
            ClosePort();
        }

        private void buttonOpenPort_Click(object sender, EventArgs e) {

            if (selectedDevice == null && comboBoxPorts.Items.Count > 0 && comboBoxPorts.SelectedIndex >= 0 && comboBoxPorts.SelectedIndex < comboBoxPorts.Items.Count) {

                selectedDevice = deviceList[comboBoxPorts.SelectedIndex];

                labelFwVerValue.Text = (selectedDevice.FirmwareVersion).ToString("X2");

                buttonOpenPort.Text = "Disconnect";

                buttonLog.Enabled = true;
                checkBoxLogCsv.Enabled = true;
                checkBoxLogTextFile.Enabled = true;
                checkBoxLogHwinfo.Enabled = true;
                labelGuidValue.Text = selectedDevice.Guid.ToString().ToUpper();

                data_logger = new DataLogger(selectedDevice.Name);

                int graph_height = 100;
                int graph_width = panelMonitoring.Width / 2 - 20;

                foreach (Sensor sensor in selectedDevice.Sensors)
                {
                    MonitorGraph monitor_graph = new MonitorGraph(sensor, graph_width, graph_height);
                    monitor_graph.MaxPoints = (int)((double)numericUpDownDuration.Value * 1000.0 / monitoring_interval);
                    graphList.Add(monitor_graph);
                    panelMonitoring.Controls.Add(monitor_graph);
                    data_logger.AddLogItem(sensor);
                }

                StartMonitoring();

            } else {
                ClosePort();
            }
        }

        private void ClosePort()
        {
            StopMonitoring();

            if (data_logger != null && data_logger.IsLogging)
            {
                buttonLog_Click(null, null);
            }

            selectedDevice = null;

            graphList.Clear();
            panelMonitoring.Controls.Clear();

            buttonOpenPort.Text = "Connect";
            buttonLog.Enabled = false;
            checkBoxLogCsv.Enabled = false;
            checkBoxLogTextFile.Enabled = false;
            checkBoxLogHwinfo.Enabled = false;

        }

        private void buttonRefreshPorts_Click(object sender, EventArgs e) {

            UpdateDeviceList();

        }

        private void buttonLog_Click(object sender, EventArgs e) {

            if (data_logger == null)
            {
                return;
            }

            if (data_logger.IsLogging)
            {
                data_logger.Stop();
                buttonLog.Text = "Start logging";
                checkBoxLogHwinfo.Enabled = true;
                checkBoxLogTextFile.Enabled = true;
                checkBoxLogCsv.Enabled = true;

                return;

            }

            bool textFile = checkBoxLogTextFile.Checked;
            string textFilePath = string.Empty;

            if (textFile) {
                // Ask for text file path
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = $"{selectedDevice.Name}.txt";
                saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textFile = true;
                    textFilePath = saveFileDialog.FileName;
                }
            }

            bool csv = checkBoxLogCsv.Checked;
            string csvFilePath = string.Empty;

            if (csv)
            {
                // Ask for CSV file path
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = $"{selectedDevice.Name}_{DateTime.Now.ToString("yyyyMMdd_HHmm")}.csv";
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(saveFileDialog.FileName))
                    {
                        DialogResult dr = MessageBox.Show($"Overwrite {saveFileDialog.FileName}?", "File already exists", MessageBoxButtons.OKCancel);
                        if (dr == DialogResult.OK)
                        {
                            csv = true;
                            csvFilePath = saveFileDialog.FileName;
                        }
                    } else
                    {
                        csv = true;
                        csvFilePath = saveFileDialog.FileName;
                    }
                }
            }

            data_logger.SetTextFile(textFilePath, textFile);
            data_logger.SetCsv(csvFilePath, csv);
            data_logger.SetHwinfo(checkBoxLogHwinfo.Checked);

            try
            {
                data_logger.Start();

            }
            catch (Exception ex)
            {
                try { data_logger.Start(); } catch { }
            }

            if(data_logger.IsLogging) {
                buttonLog.Text = "Stop logging";
                checkBoxLogHwinfo.Enabled = false;
                checkBoxLogTextFile.Enabled = false;
                checkBoxLogCsv.Enabled = false;
            }

        }

        int monitoring_interval = 100;

        private void numericUpDownMonitoringInterval_ValueChanged(object sender, EventArgs e)
        {
            UpdateGraphTiming();
        }

        private void numericUpDownDuration_ValueChanged(object sender, EventArgs e)
        {
            UpdateGraphTiming();
        }

        private void UpdateGraphTiming()
        {
            int time = (int)numericUpDownDuration.Value;
            monitoring_interval = (int)numericUpDownMonitoringInterval.Value;
            if (monitoring_interval < 1) monitoring_interval = 1;
            double num_points = time / (monitoring_interval / 1000.0);

            foreach (MonitorGraph monitor_graph in graphList)
            {
                monitor_graph.MaxPoints = (int)num_points;
            }

            if (selectedDevice != null)
            {
                selectedDevice.MonitoringInterval = monitoring_interval;
            }
        }

        private void buttonCal_Click(object sender, EventArgs e)
        {
            if(selectedDevice is PMD2_Device pmd2_device)
            {
                FormCalPMD2 formCalPMD2 = new FormCalPMD2(pmd2_device);
                formCalPMD2.Show();
            }
        }

        private void FormPMD_Shown(object sender, EventArgs e)
        {
            UpdateDeviceList();
        }

        private void FormPMD_Load(object sender, EventArgs e)
        {

        }
    }
}