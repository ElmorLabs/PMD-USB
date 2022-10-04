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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.comboBoxOled = new System.Windows.Forms.ComboBox();
            this.labelDisplayupdate = new System.Windows.Forms.Label();
            this.labelAof = new System.Windows.Forms.Label();
            this.buttonReset = new System.Windows.Forms.Button();
            this.buttonBootloader = new System.Windows.Forms.Button();
            this.buttonApplyConfig = new System.Windows.Forms.Button();
            this.buttonStorecfg = new System.Windows.Forms.Button();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.labelFwVer = new System.Windows.Forms.Label();
            this.labelFwVerValue = new System.Windows.Forms.Label();
            this.comboBoxPorts = new System.Windows.Forms.ComboBox();
            this.buttonOpenPort = new System.Windows.Forms.Button();
            this.buttonRefreshPorts = new System.Windows.Forms.Button();
            this.buttonLog = new System.Windows.Forms.Button();
            this.buttonWriteToFile = new System.Windows.Forms.Button();
            this.panelMonitoring = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxPcie1Ioffset = new System.Windows.Forms.TextBox();
            this.textBoxPcie2Ioffset = new System.Windows.Forms.TextBox();
            this.textBoxEps1Ioffset = new System.Windows.Forms.TextBox();
            this.textBoxEps2Ioffset = new System.Windows.Forms.TextBox();
            this.textBoxPcie1Voffset = new System.Windows.Forms.TextBox();
            this.textBoxPcie2Voffset = new System.Windows.Forms.TextBox();
            this.textBoxEps1Voffset = new System.Windows.Forms.TextBox();
            this.textBoxEps2Voffset = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBoxTimeoutAction = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBoxDisplaySpeed = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBoxAveraging = new System.Windows.Forms.ComboBox();
            this.textBoxTimeoutDelay = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.comboBoxOledRotation = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label10, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBoxTimeoutDelay, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxOled, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelAof, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelDisplayupdate, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxTimeoutAction, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxDisplaySpeed, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label9, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxAveraging, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.buttonApplyConfig, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxOledRotation, 1, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(9, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(176, 178);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // comboBoxOled
            // 
            this.comboBoxOled.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.comboBoxOled.FormattingEnabled = true;
            this.comboBoxOled.Location = new System.Drawing.Point(88, 53);
            this.comboBoxOled.Name = "comboBoxOled";
            this.comboBoxOled.Size = new System.Drawing.Size(85, 21);
            this.comboBoxOled.TabIndex = 8;
            // 
            // labelDisplayupdate
            // 
            this.labelDisplayupdate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelDisplayupdate.AutoSize = true;
            this.labelDisplayupdate.Location = new System.Drawing.Point(3, 106);
            this.labelDisplayupdate.Name = "labelDisplayupdate";
            this.labelDisplayupdate.Size = new System.Drawing.Size(70, 13);
            this.labelDisplayupdate.TabIndex = 7;
            this.labelDisplayupdate.Text = "OLED Speed";
            // 
            // labelAof
            // 
            this.labelAof.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelAof.AutoSize = true;
            this.labelAof.Location = new System.Drawing.Point(3, 31);
            this.labelAof.Name = "labelAof";
            this.labelAof.Size = new System.Drawing.Size(75, 13);
            this.labelAof.TabIndex = 0;
            this.labelAof.Text = "Timeout Delay";
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(658, 75);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(70, 23);
            this.buttonReset.TabIndex = 2;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // buttonBootloader
            // 
            this.buttonBootloader.Location = new System.Drawing.Point(734, 75);
            this.buttonBootloader.Name = "buttonBootloader";
            this.buttonBootloader.Size = new System.Drawing.Size(70, 23);
            this.buttonBootloader.TabIndex = 3;
            this.buttonBootloader.Text = "Bootloader";
            this.buttonBootloader.UseVisualStyleBackColor = true;
            this.buttonBootloader.Click += new System.EventHandler(this.buttonBootloader_Click);
            // 
            // buttonApplyConfig
            // 
            this.buttonApplyConfig.Enabled = false;
            this.buttonApplyConfig.Location = new System.Drawing.Point(88, 153);
            this.buttonApplyConfig.Name = "buttonApplyConfig";
            this.buttonApplyConfig.Size = new System.Drawing.Size(85, 22);
            this.buttonApplyConfig.TabIndex = 6;
            this.buttonApplyConfig.Text = "Apply";
            this.buttonApplyConfig.UseVisualStyleBackColor = true;
            this.buttonApplyConfig.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // buttonStorecfg
            // 
            this.buttonStorecfg.Enabled = false;
            this.buttonStorecfg.Location = new System.Drawing.Point(584, 75);
            this.buttonStorecfg.Name = "buttonStorecfg";
            this.buttonStorecfg.Size = new System.Drawing.Size(70, 23);
            this.buttonStorecfg.TabIndex = 7;
            this.buttonStorecfg.Text = "Store cfg";
            this.buttonStorecfg.UseVisualStyleBackColor = true;
            this.buttonStorecfg.Click += new System.EventHandler(this.buttonStorecfg_Click);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.labelFwVer, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.labelFwVerValue, 1, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(677, 104);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(124, 24);
            this.tableLayoutPanel3.TabIndex = 10;
            // 
            // labelFwVer
            // 
            this.labelFwVer.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelFwVer.AutoSize = true;
            this.labelFwVer.Location = new System.Drawing.Point(3, 5);
            this.labelFwVer.Name = "labelFwVer";
            this.labelFwVer.Size = new System.Drawing.Size(46, 13);
            this.labelFwVer.TabIndex = 4;
            this.labelFwVer.Text = "FW Ver.";
            this.labelFwVer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelFwVerValue
            // 
            this.labelFwVerValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelFwVerValue.AutoSize = true;
            this.labelFwVerValue.Location = new System.Drawing.Point(63, 5);
            this.labelFwVerValue.Name = "labelFwVerValue";
            this.labelFwVerValue.Size = new System.Drawing.Size(0, 13);
            this.labelFwVerValue.TabIndex = 5;
            this.labelFwVerValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxPorts
            // 
            this.comboBoxPorts.FormattingEnabled = true;
            this.comboBoxPorts.Location = new System.Drawing.Point(515, 6);
            this.comboBoxPorts.Name = "comboBoxPorts";
            this.comboBoxPorts.Size = new System.Drawing.Size(137, 21);
            this.comboBoxPorts.TabIndex = 12;
            // 
            // buttonOpenPort
            // 
            this.buttonOpenPort.Location = new System.Drawing.Point(734, 5);
            this.buttonOpenPort.Name = "buttonOpenPort";
            this.buttonOpenPort.Size = new System.Drawing.Size(70, 23);
            this.buttonOpenPort.TabIndex = 13;
            this.buttonOpenPort.Text = "Open";
            this.buttonOpenPort.UseVisualStyleBackColor = true;
            this.buttonOpenPort.Click += new System.EventHandler(this.buttonOpenPort_Click);
            // 
            // buttonRefreshPorts
            // 
            this.buttonRefreshPorts.Location = new System.Drawing.Point(658, 5);
            this.buttonRefreshPorts.Name = "buttonRefreshPorts";
            this.buttonRefreshPorts.Size = new System.Drawing.Size(70, 23);
            this.buttonRefreshPorts.TabIndex = 14;
            this.buttonRefreshPorts.Text = "Refresh";
            this.buttonRefreshPorts.UseVisualStyleBackColor = true;
            this.buttonRefreshPorts.Click += new System.EventHandler(this.buttonRefreshPorts_Click);
            // 
            // buttonLog
            // 
            this.buttonLog.Location = new System.Drawing.Point(734, 40);
            this.buttonLog.Name = "buttonLog";
            this.buttonLog.Size = new System.Drawing.Size(70, 23);
            this.buttonLog.TabIndex = 15;
            this.buttonLog.Text = "Log to CSV";
            this.buttonLog.UseVisualStyleBackColor = true;
            this.buttonLog.Click += new System.EventHandler(this.buttonLog_Click);
            // 
            // buttonWriteToFile
            // 
            this.buttonWriteToFile.Enabled = false;
            this.buttonWriteToFile.Location = new System.Drawing.Point(658, 40);
            this.buttonWriteToFile.Name = "buttonWriteToFile";
            this.buttonWriteToFile.Size = new System.Drawing.Size(70, 23);
            this.buttonWriteToFile.TabIndex = 16;
            this.buttonWriteToFile.Text = "Write to file";
            this.buttonWriteToFile.UseVisualStyleBackColor = true;
            this.buttonWriteToFile.Click += new System.EventHandler(this.buttonWriteToFile_Click);
            // 
            // panelMonitoring
            // 
            this.panelMonitoring.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMonitoring.AutoScroll = true;
            this.panelMonitoring.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelMonitoring.Location = new System.Drawing.Point(9, 250);
            this.panelMonitoring.Name = "panelMonitoring";
            this.panelMonitoring.Size = new System.Drawing.Size(795, 360);
            this.panelMonitoring.TabIndex = 18;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.label5, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.label6, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.textBoxPcie1Ioffset, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.textBoxPcie2Ioffset, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.textBoxEps1Ioffset, 2, 3);
            this.tableLayoutPanel2.Controls.Add(this.textBoxEps2Ioffset, 2, 4);
            this.tableLayoutPanel2.Controls.Add(this.textBoxPcie1Voffset, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.textBoxPcie2Voffset, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.textBoxEps1Voffset, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.textBoxEps2Voffset, 1, 4);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(191, 5);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 6;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(217, 155);
            this.tableLayoutPanel2.TabIndex = 19;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(78, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Voltage";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 106);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "EPS2 Offset";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "PCIE1 Offset";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "PCIE2 Offset";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 81);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "EPS1 Offset";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(149, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 13);
            this.label6.TabIndex = 21;
            this.label6.Text = "Current";
            // 
            // textBoxPcie1Ioffset
            // 
            this.textBoxPcie1Ioffset.Location = new System.Drawing.Point(149, 28);
            this.textBoxPcie1Ioffset.Name = "textBoxPcie1Ioffset";
            this.textBoxPcie1Ioffset.Size = new System.Drawing.Size(65, 20);
            this.textBoxPcie1Ioffset.TabIndex = 8;
            // 
            // textBoxPcie2Ioffset
            // 
            this.textBoxPcie2Ioffset.Location = new System.Drawing.Point(149, 53);
            this.textBoxPcie2Ioffset.Name = "textBoxPcie2Ioffset";
            this.textBoxPcie2Ioffset.Size = new System.Drawing.Size(65, 20);
            this.textBoxPcie2Ioffset.TabIndex = 9;
            // 
            // textBoxEps1Ioffset
            // 
            this.textBoxEps1Ioffset.Location = new System.Drawing.Point(149, 78);
            this.textBoxEps1Ioffset.Name = "textBoxEps1Ioffset";
            this.textBoxEps1Ioffset.Size = new System.Drawing.Size(65, 20);
            this.textBoxEps1Ioffset.TabIndex = 10;
            // 
            // textBoxEps2Ioffset
            // 
            this.textBoxEps2Ioffset.Location = new System.Drawing.Point(149, 103);
            this.textBoxEps2Ioffset.Name = "textBoxEps2Ioffset";
            this.textBoxEps2Ioffset.Size = new System.Drawing.Size(65, 20);
            this.textBoxEps2Ioffset.TabIndex = 11;
            // 
            // textBoxPcie1Voffset
            // 
            this.textBoxPcie1Voffset.Location = new System.Drawing.Point(78, 28);
            this.textBoxPcie1Voffset.Name = "textBoxPcie1Voffset";
            this.textBoxPcie1Voffset.Size = new System.Drawing.Size(65, 20);
            this.textBoxPcie1Voffset.TabIndex = 22;
            // 
            // textBoxPcie2Voffset
            // 
            this.textBoxPcie2Voffset.Location = new System.Drawing.Point(78, 53);
            this.textBoxPcie2Voffset.Name = "textBoxPcie2Voffset";
            this.textBoxPcie2Voffset.Size = new System.Drawing.Size(65, 20);
            this.textBoxPcie2Voffset.TabIndex = 23;
            // 
            // textBoxEps1Voffset
            // 
            this.textBoxEps1Voffset.Location = new System.Drawing.Point(78, 78);
            this.textBoxEps1Voffset.Name = "textBoxEps1Voffset";
            this.textBoxEps1Voffset.Size = new System.Drawing.Size(65, 20);
            this.textBoxEps1Voffset.TabIndex = 24;
            // 
            // textBoxEps2Voffset
            // 
            this.textBoxEps2Voffset.Location = new System.Drawing.Point(78, 103);
            this.textBoxEps2Voffset.Name = "textBoxEps2Voffset";
            this.textBoxEps2Voffset.Size = new System.Drawing.Size(65, 20);
            this.textBoxEps2Voffset.TabIndex = 25;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(78, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Timeout Action";
            // 
            // comboBoxTimeoutAction
            // 
            this.comboBoxTimeoutAction.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.comboBoxTimeoutAction.FormattingEnabled = true;
            this.comboBoxTimeoutAction.Location = new System.Drawing.Point(88, 3);
            this.comboBoxTimeoutAction.Name = "comboBoxTimeoutAction";
            this.comboBoxTimeoutAction.Size = new System.Drawing.Size(85, 21);
            this.comboBoxTimeoutAction.TabIndex = 10;
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 56);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(36, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "OLED";
            // 
            // comboBoxDisplaySpeed
            // 
            this.comboBoxDisplaySpeed.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.comboBoxDisplaySpeed.FormattingEnabled = true;
            this.comboBoxDisplaySpeed.Location = new System.Drawing.Point(88, 103);
            this.comboBoxDisplaySpeed.Name = "comboBoxDisplaySpeed";
            this.comboBoxDisplaySpeed.Size = new System.Drawing.Size(85, 21);
            this.comboBoxDisplaySpeed.TabIndex = 12;
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 131);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(55, 13);
            this.label9.TabIndex = 13;
            this.label9.Text = "Averaging";
            // 
            // comboBoxAveraging
            // 
            this.comboBoxAveraging.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.comboBoxAveraging.FormattingEnabled = true;
            this.comboBoxAveraging.Location = new System.Drawing.Point(88, 128);
            this.comboBoxAveraging.Name = "comboBoxAveraging";
            this.comboBoxAveraging.Size = new System.Drawing.Size(85, 21);
            this.comboBoxAveraging.TabIndex = 14;
            // 
            // textBoxTimeoutDelay
            // 
            this.textBoxTimeoutDelay.Location = new System.Drawing.Point(88, 28);
            this.textBoxTimeoutDelay.Name = "textBoxTimeoutDelay";
            this.textBoxTimeoutDelay.Size = new System.Drawing.Size(85, 20);
            this.textBoxTimeoutDelay.TabIndex = 23;
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 81);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(79, 13);
            this.label10.TabIndex = 24;
            this.label10.Text = "OLED Rotation";
            // 
            // comboBoxOledRotation
            // 
            this.comboBoxOledRotation.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.comboBoxOledRotation.FormattingEnabled = true;
            this.comboBoxOledRotation.Location = new System.Drawing.Point(88, 78);
            this.comboBoxOledRotation.Name = "comboBoxOledRotation";
            this.comboBoxOledRotation.Size = new System.Drawing.Size(85, 21);
            this.comboBoxOledRotation.TabIndex = 25;
            // 
            // FormPMD
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(813, 622);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.panelMonitoring);
            this.Controls.Add(this.buttonWriteToFile);
            this.Controls.Add(this.buttonLog);
            this.Controls.Add(this.buttonRefreshPorts);
            this.Controls.Add(this.buttonOpenPort);
            this.Controls.Add(this.comboBoxPorts);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.buttonStorecfg);
            this.Controls.Add(this.buttonBootloader);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormPMD";
            this.Text = "PMD-USB";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormKTH_FormClosing);
            this.Load += new System.EventHandler(this.FormPMD_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelAof;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.Button buttonBootloader;
        private System.Windows.Forms.Button buttonApplyConfig;
        private System.Windows.Forms.ComboBox comboBoxOled;
        private System.Windows.Forms.Label labelDisplayupdate;
        private System.Windows.Forms.Button buttonStorecfg;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label labelFwVer;
        private System.Windows.Forms.ComboBox comboBoxPorts;
        private System.Windows.Forms.Button buttonOpenPort;
        private System.Windows.Forms.Button buttonRefreshPorts;
        private System.Windows.Forms.Button buttonLog;
        private System.Windows.Forms.Button buttonWriteToFile;
        private System.Windows.Forms.FlowLayoutPanel panelMonitoring;
        private System.Windows.Forms.Label labelFwVerValue;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxEps1Ioffset;
        private System.Windows.Forms.TextBox textBoxPcie2Ioffset;
        private System.Windows.Forms.TextBox textBoxPcie1Ioffset;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxEps2Ioffset;
        private System.Windows.Forms.TextBox textBoxPcie1Voffset;
        private System.Windows.Forms.TextBox textBoxPcie2Voffset;
        private System.Windows.Forms.TextBox textBoxEps1Voffset;
        private System.Windows.Forms.TextBox textBoxEps2Voffset;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBoxTimeoutAction;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBoxDisplaySpeed;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox comboBoxAveraging;
        private System.Windows.Forms.TextBox textBoxTimeoutDelay;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox comboBoxOledRotation;
    }
}