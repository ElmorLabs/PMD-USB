namespace PMD {
    partial class FormPMD {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPMD));
            this.labelFwVer = new System.Windows.Forms.Label();
            this.labelFwVerValue = new System.Windows.Forms.Label();
            this.comboBoxPorts = new System.Windows.Forms.ComboBox();
            this.buttonOpenPort = new System.Windows.Forms.Button();
            this.buttonRefreshPorts = new System.Windows.Forms.Button();
            this.buttonLog = new System.Windows.Forms.Button();
            this.panelMonitoring = new System.Windows.Forms.FlowLayoutPanel();
            this.numericUpDownMonitoringInterval = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.labelDuration = new System.Windows.Forms.Label();
            this.numericUpDownDuration = new System.Windows.Forms.NumericUpDown();
            this.buttonDevice = new System.Windows.Forms.Button();
            this.checkBoxLogHwinfo = new System.Windows.Forms.CheckBox();
            this.checkBoxLogTextFile = new System.Windows.Forms.CheckBox();
            this.checkBoxLogCsv = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelGuidValue = new System.Windows.Forms.Label();
            this.labelGuidText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMonitoringInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDuration)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelFwVer
            // 
            this.labelFwVer.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelFwVer.AutoSize = true;
            this.labelFwVer.Location = new System.Drawing.Point(227, 98);
            this.labelFwVer.Name = "labelFwVer";
            this.labelFwVer.Size = new System.Drawing.Size(46, 13);
            this.labelFwVer.TabIndex = 4;
            this.labelFwVer.Text = "FW Ver.";
            this.labelFwVer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelFwVerValue
            // 
            this.labelFwVerValue.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelFwVerValue.AutoSize = true;
            this.labelFwVerValue.Location = new System.Drawing.Point(350, 98);
            this.labelFwVerValue.Name = "labelFwVerValue";
            this.labelFwVerValue.Size = new System.Drawing.Size(0, 13);
            this.labelFwVerValue.TabIndex = 5;
            this.labelFwVerValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxPorts
            // 
            this.comboBoxPorts.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel2.SetColumnSpan(this.comboBoxPorts, 2);
            this.comboBoxPorts.FormattingEnabled = true;
            this.comboBoxPorts.Location = new System.Drawing.Point(131, 4);
            this.comboBoxPorts.Name = "comboBoxPorts";
            this.comboBoxPorts.Size = new System.Drawing.Size(137, 21);
            this.comboBoxPorts.TabIndex = 12;
            // 
            // buttonOpenPort
            // 
            this.buttonOpenPort.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonOpenPort.Location = new System.Drawing.Point(312, 3);
            this.buttonOpenPort.Name = "buttonOpenPort";
            this.buttonOpenPort.Size = new System.Drawing.Size(75, 23);
            this.buttonOpenPort.TabIndex = 13;
            this.buttonOpenPort.Text = "Connect";
            this.buttonOpenPort.UseVisualStyleBackColor = true;
            this.buttonOpenPort.Click += new System.EventHandler(this.buttonOpenPort_Click);
            // 
            // buttonRefreshPorts
            // 
            this.buttonRefreshPorts.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonRefreshPorts.Location = new System.Drawing.Point(15, 3);
            this.buttonRefreshPorts.Name = "buttonRefreshPorts";
            this.buttonRefreshPorts.Size = new System.Drawing.Size(70, 23);
            this.buttonRefreshPorts.TabIndex = 14;
            this.buttonRefreshPorts.Text = "Refresh";
            this.buttonRefreshPorts.UseVisualStyleBackColor = true;
            this.buttonRefreshPorts.Click += new System.EventHandler(this.buttonRefreshPorts_Click);
            // 
            // buttonLog
            // 
            this.buttonLog.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonLog.Enabled = false;
            this.buttonLog.Location = new System.Drawing.Point(312, 33);
            this.buttonLog.Name = "buttonLog";
            this.buttonLog.Size = new System.Drawing.Size(75, 23);
            this.buttonLog.TabIndex = 15;
            this.buttonLog.Text = "Start logging";
            this.buttonLog.UseVisualStyleBackColor = true;
            this.buttonLog.Click += new System.EventHandler(this.buttonLog_Click);
            // 
            // panelMonitoring
            // 
            this.panelMonitoring.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMonitoring.AutoScroll = true;
            this.panelMonitoring.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelMonitoring.Location = new System.Drawing.Point(9, 138);
            this.panelMonitoring.Name = "panelMonitoring";
            this.panelMonitoring.Size = new System.Drawing.Size(795, 472);
            this.panelMonitoring.TabIndex = 18;
            // 
            // numericUpDownMonitoringInterval
            // 
            this.numericUpDownMonitoringInterval.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.numericUpDownMonitoringInterval.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownMonitoringInterval.Location = new System.Drawing.Point(119, 65);
            this.numericUpDownMonitoringInterval.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericUpDownMonitoringInterval.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownMonitoringInterval.Name = "numericUpDownMonitoringInterval";
            this.numericUpDownMonitoringInterval.Size = new System.Drawing.Size(61, 20);
            this.numericUpDownMonitoringInterval.TabIndex = 22;
            this.numericUpDownMonitoringInterval.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownMonitoringInterval.ValueChanged += new System.EventHandler(this.numericUpDownMonitoringInterval_ValueChanged);
            // 
            // label16
            // 
            this.label16.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(18, 68);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(64, 13);
            this.label16.TabIndex = 23;
            this.label16.Text = "Interval (ms)";
            // 
            // labelDuration
            // 
            this.labelDuration.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelDuration.AutoSize = true;
            this.labelDuration.Location = new System.Drawing.Point(219, 68);
            this.labelDuration.Name = "labelDuration";
            this.labelDuration.Size = new System.Drawing.Size(61, 13);
            this.labelDuration.TabIndex = 25;
            this.labelDuration.Text = "Duration (s)";
            // 
            // numericUpDownDuration
            // 
            this.numericUpDownDuration.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.numericUpDownDuration.Location = new System.Drawing.Point(319, 65);
            this.numericUpDownDuration.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numericUpDownDuration.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownDuration.Name = "numericUpDownDuration";
            this.numericUpDownDuration.Size = new System.Drawing.Size(61, 20);
            this.numericUpDownDuration.TabIndex = 24;
            this.numericUpDownDuration.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDownDuration.ValueChanged += new System.EventHandler(this.numericUpDownDuration_ValueChanged);
            // 
            // buttonDevice
            // 
            this.buttonDevice.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonDevice.Location = new System.Drawing.Point(112, 93);
            this.buttonDevice.Name = "buttonDevice";
            this.buttonDevice.Size = new System.Drawing.Size(75, 23);
            this.buttonDevice.TabIndex = 26;
            this.buttonDevice.Text = "CAL";
            this.buttonDevice.UseVisualStyleBackColor = true;
            this.buttonDevice.Click += new System.EventHandler(this.buttonCal_Click);
            // 
            // checkBoxLogHwinfo
            // 
            this.checkBoxLogHwinfo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkBoxLogHwinfo.AutoSize = true;
            this.checkBoxLogHwinfo.Checked = true;
            this.checkBoxLogHwinfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLogHwinfo.Enabled = false;
            this.checkBoxLogHwinfo.Location = new System.Drawing.Point(15, 36);
            this.checkBoxLogHwinfo.Name = "checkBoxLogHwinfo";
            this.checkBoxLogHwinfo.Size = new System.Drawing.Size(69, 17);
            this.checkBoxLogHwinfo.TabIndex = 27;
            this.checkBoxLogHwinfo.Text = "HWiNFO";
            this.checkBoxLogHwinfo.UseVisualStyleBackColor = true;
            // 
            // checkBoxLogTextFile
            // 
            this.checkBoxLogTextFile.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkBoxLogTextFile.AutoSize = true;
            this.checkBoxLogTextFile.Enabled = false;
            this.checkBoxLogTextFile.Location = new System.Drawing.Point(118, 36);
            this.checkBoxLogTextFile.Name = "checkBoxLogTextFile";
            this.checkBoxLogTextFile.Size = new System.Drawing.Size(63, 17);
            this.checkBoxLogTextFile.TabIndex = 28;
            this.checkBoxLogTextFile.Text = "Text file";
            this.checkBoxLogTextFile.UseVisualStyleBackColor = true;
            // 
            // checkBoxLogCsv
            // 
            this.checkBoxLogCsv.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkBoxLogCsv.AutoSize = true;
            this.checkBoxLogCsv.Enabled = false;
            this.checkBoxLogCsv.Location = new System.Drawing.Point(226, 36);
            this.checkBoxLogCsv.Name = "checkBoxLogCsv";
            this.checkBoxLogCsv.Size = new System.Drawing.Size(47, 17);
            this.checkBoxLogCsv.TabIndex = 29;
            this.checkBoxLogCsv.Text = "CSV";
            this.checkBoxLogCsv.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.Controls.Add(this.buttonRefreshPorts, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.checkBoxLogCsv, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.numericUpDownDuration, 3, 2);
            this.tableLayoutPanel2.Controls.Add(this.labelDuration, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.comboBoxPorts, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.checkBoxLogTextFile, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.numericUpDownMonitoringInterval, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.label16, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.buttonOpenPort, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.checkBoxLogHwinfo, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.buttonLog, 3, 1);
            this.tableLayoutPanel2.Controls.Add(this.buttonDevice, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.labelFwVer, 2, 3);
            this.tableLayoutPanel2.Controls.Add(this.labelFwVerValue, 3, 3);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(404, 12);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(400, 120);
            this.tableLayoutPanel2.TabIndex = 30;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.labelGuidValue, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelGuidText, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(9, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(350, 40);
            this.tableLayoutPanel1.TabIndex = 31;
            // 
            // labelGuidValue
            // 
            this.labelGuidValue.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelGuidValue.AutoSize = true;
            this.labelGuidValue.Location = new System.Drawing.Point(200, 3);
            this.labelGuidValue.Name = "labelGuidValue";
            this.labelGuidValue.Size = new System.Drawing.Size(0, 13);
            this.labelGuidValue.TabIndex = 1;
            // 
            // labelGuidText
            // 
            this.labelGuidText.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelGuidText.AutoSize = true;
            this.labelGuidText.Location = new System.Drawing.Point(12, 3);
            this.labelGuidText.Name = "labelGuidText";
            this.labelGuidText.Size = new System.Drawing.Size(26, 13);
            this.labelGuidText.TabIndex = 0;
            this.labelGuidText.Text = "UID";
            // 
            // FormPMD
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(813, 622);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.panelMonitoring);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormPMD";
            this.Text = "PMD-USB";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormKTH_FormClosing);
            this.Load += new System.EventHandler(this.FormPMD_Load);
            this.Shown += new System.EventHandler(this.FormPMD_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMonitoringInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDuration)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label labelFwVer;
        private System.Windows.Forms.ComboBox comboBoxPorts;
        private System.Windows.Forms.Button buttonOpenPort;
        private System.Windows.Forms.Button buttonRefreshPorts;
        private System.Windows.Forms.Button buttonLog;
        private System.Windows.Forms.FlowLayoutPanel panelMonitoring;
        private System.Windows.Forms.Label labelFwVerValue;
        private System.Windows.Forms.NumericUpDown numericUpDownMonitoringInterval;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label labelDuration;
        private System.Windows.Forms.NumericUpDown numericUpDownDuration;
        private System.Windows.Forms.Button buttonDevice;
        private System.Windows.Forms.CheckBox checkBoxLogHwinfo;
        private System.Windows.Forms.CheckBox checkBoxLogTextFile;
        private System.Windows.Forms.CheckBox checkBoxLogCsv;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelGuidValue;
        private System.Windows.Forms.Label labelGuidText;
    }
}