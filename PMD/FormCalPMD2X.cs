using System;
using System.Drawing;
using System.Windows.Forms;
using static PMD.PMD2X_Device;

namespace PMD
{
    using DeviceConfigStruct = DeviceConfigStructV2; // Use V2 as the current version

    public partial class FormCalPMD2X : Form
    {

        PMD2X_Device pmd2_device;
        DeviceConfigStruct deviceConfig;

        private readonly TextBox[] hpwrWireVoltageGainTextBoxes = new TextBox[6];
        private readonly TextBox[] hpwrWireVoltageOffsetTextBoxes = new TextBox[6];
        private readonly TextBox[] hpwrWireCurrentGainTextBoxes = new TextBox[6];
        private readonly TextBox[] hpwrWireCurrentOffsetTextBoxes = new TextBox[6];

        private static readonly int[] EditableRailIndices = { 0, 1, 2, 3, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

        private static readonly string[] RailNames = new[]
        {
            "ATX 12V", "ATX 5V", "ATX 5VSB", "ATX 3V3", "EPS1", "EPS2", "PCIE1", "PCIE2", "PCIE3",
            "HPWR W1", "HPWR W2", "HPWR W3", "HPWR W4", "HPWR W5", "HPWR W6"
        };

        public FormCalPMD2X(PMD2X_Device pmd2_device)
        {
            InitializeComponent();
            this.pmd2_device = pmd2_device;
            HideAggregateHpwrRail();
            AddAdditionalRails();
        }

        private void HideAggregateHpwrRail()
        {
            label17.Visible = false;
            textBoxHpwrVgain.Visible = false;
            textBoxHpwrVoffset.Visible = false;
            textBoxHpwrIgain.Visible = false;
            textBoxHpwrIoffset.Visible = false;

            if (tableLayoutPanel2.RowStyles.Count > 6)
            {
                tableLayoutPanel2.RowStyles[6].Height = 0F;
            }
        }

        private void AddAdditionalRails()
        {
            int firstAdditionalRow = tableLayoutPanel2.RowCount;
            tableLayoutPanel2.RowCount += 6;

            for (int i = 0; i < 6; i++)
            {
                tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));

                int row = firstAdditionalRow + i;

                Label label = new Label
                {
                    Anchor = AnchorStyles.Left,
                    AutoSize = true,
                    Text = $"HPWR W{i + 1}"
                };

                TextBox vGain = new TextBox { Size = new Size(44, 20) };
                TextBox vOffset = new TextBox { Size = new Size(44, 20) };
                TextBox iGain = new TextBox { Size = new Size(44, 20) };
                TextBox iOffset = new TextBox { Size = new Size(46, 20) };

                hpwrWireVoltageGainTextBoxes[i] = vGain;
                hpwrWireVoltageOffsetTextBoxes[i] = vOffset;
                hpwrWireCurrentGainTextBoxes[i] = iGain;
                hpwrWireCurrentOffsetTextBoxes[i] = iOffset;

                tableLayoutPanel2.Controls.Add(label, 0, row);
                tableLayoutPanel2.Controls.Add(vGain, 1, row);
                tableLayoutPanel2.Controls.Add(vOffset, 2, row);
                tableLayoutPanel2.Controls.Add(iGain, 3, row);
                tableLayoutPanel2.Controls.Add(iOffset, 4, row);
            }

            tableLayoutPanel2.Size = new Size(tableLayoutPanel2.Width, 2 + (tableLayoutPanel2.RowCount * 25));

            int buttonsY = tableLayoutPanel2.Bottom + 6;
            buttonRead.Top = buttonsY;
            buttonWrite.Top = buttonsY;
            buttonLoad.Top = buttonsY;
            buttonStore.Top = buttonsY;

            ClientSize = new Size(ClientSize.Width, buttonRead.Bottom + 10);
        }

        private TextBox[] GetVoltageGainTextBoxes()
        {
            return new[]
            {
                textBoxAtx12Vgain, textBoxAtx5Vgain, textBoxAtx5sbVgain, textBoxAtx3Vgain,
                textBoxEps1Vgain, textBoxEps2Vgain, textBoxPcie1Vgain, textBoxPcie2Vgain, textBoxPcie3Vgain,
                hpwrWireVoltageGainTextBoxes[0], hpwrWireVoltageGainTextBoxes[1], hpwrWireVoltageGainTextBoxes[2],
                hpwrWireVoltageGainTextBoxes[3], hpwrWireVoltageGainTextBoxes[4], hpwrWireVoltageGainTextBoxes[5]
            };
        }

        private TextBox[] GetVoltageOffsetTextBoxes()
        {
            return new[]
            {
                textBoxAtx12Voffset, textBoxAtx5Voffset, textBoxAtx5sbVoffset, textBoxAtx3Voffset,
                textBoxEps1Voffset, textBoxEps2Voffset, textBoxPcie1Voffset, textBoxPcie2Voffset, textBoxPcie3Voffset,
                hpwrWireVoltageOffsetTextBoxes[0], hpwrWireVoltageOffsetTextBoxes[1], hpwrWireVoltageOffsetTextBoxes[2],
                hpwrWireVoltageOffsetTextBoxes[3], hpwrWireVoltageOffsetTextBoxes[4], hpwrWireVoltageOffsetTextBoxes[5]
            };
        }

        private TextBox[] GetCurrentGainTextBoxes()
        {
            return new[]
            {
                textBoxAtx12Igain, textBoxAtx5Igain, textBoxAtx5sbIgain, textBoxAtx3Igain,
                textBoxEps1Igain, textBoxEps2Igain, textBoxPcie1Igain, textBoxPcie2Igain, textBoxPcie3Igain,
                hpwrWireCurrentGainTextBoxes[0], hpwrWireCurrentGainTextBoxes[1], hpwrWireCurrentGainTextBoxes[2],
                hpwrWireCurrentGainTextBoxes[3], hpwrWireCurrentGainTextBoxes[4], hpwrWireCurrentGainTextBoxes[5]
            };
        }

        private TextBox[] GetCurrentOffsetTextBoxes()
        {
            return new[]
            {
                textBoxAtx12Ioffset, textBoxAtx5Ioffset, textBoxAtx5sbIoffset, textBoxAtx3Ioffset,
                textBoxEps1Ioffset, textBoxEps2Ioffset, textBoxPcie1Ioffset, textBoxPcie2Ioffset, textBoxPcie3Ioffset,
                hpwrWireCurrentOffsetTextBoxes[0], hpwrWireCurrentOffsetTextBoxes[1], hpwrWireCurrentOffsetTextBoxes[2],
                hpwrWireCurrentOffsetTextBoxes[3], hpwrWireCurrentOffsetTextBoxes[4], hpwrWireCurrentOffsetTextBoxes[5]
            };
        }

        private void buttonRead_Click(object sender, EventArgs e)
        {
            if(!pmd2_device.ReadConfig(out deviceConfig))
            {
                MessageBox.Show("Failed to read device configuration");
                return;
            }

            TextBox[] voltageGainTextBoxes = GetVoltageGainTextBoxes();
            TextBox[] voltageOffsetTextBoxes = GetVoltageOffsetTextBoxes();
            TextBox[] currentGainTextBoxes = GetCurrentGainTextBoxes();
            TextBox[] currentOffsetTextBoxes = GetCurrentOffsetTextBoxes();

            int railCount = Math.Min(Math.Min(deviceConfig.Calibration.PowerReadingVoltage.Length, deviceConfig.Calibration.PowerReadingCurrent.Length), EditableRailIndices.Length);

            for (int i = 0; i < railCount; i++)
            {
                int railIndex = EditableRailIndices[i];
                voltageGainTextBoxes[i].Text = deviceConfig.Calibration.PowerReadingVoltage[railIndex].GainOffset.ToString();
                voltageOffsetTextBoxes[i].Text = deviceConfig.Calibration.PowerReadingVoltage[railIndex].Offset.ToString();
                currentGainTextBoxes[i].Text = deviceConfig.Calibration.PowerReadingCurrent[railIndex].GainOffset.ToString();
                currentOffsetTextBoxes[i].Text = deviceConfig.Calibration.PowerReadingCurrent[railIndex].Offset.ToString();
            }

        }

        private void buttonWrite_Click(object sender, EventArgs e)
        {
            if (deviceConfig.Calibration.PowerReadingVoltage == null || deviceConfig.Calibration.PowerReadingCurrent == null)
            {
                MessageBox.Show("Read configuration first");
                return;
            }

            TextBox[] voltageGainTextBoxes = GetVoltageGainTextBoxes();
            TextBox[] voltageOffsetTextBoxes = GetVoltageOffsetTextBoxes();
            TextBox[] currentGainTextBoxes = GetCurrentGainTextBoxes();
            TextBox[] currentOffsetTextBoxes = GetCurrentOffsetTextBoxes();

            int railCount = Math.Min(Math.Min(deviceConfig.Calibration.PowerReadingVoltage.Length, deviceConfig.Calibration.PowerReadingCurrent.Length), EditableRailIndices.Length);

            for (int i = 0; i < railCount; i++)
            {
                if (!short.TryParse(voltageGainTextBoxes[i].Text, out short vGain) ||
                    !short.TryParse(voltageOffsetTextBoxes[i].Text, out short vOffset) ||
                    !short.TryParse(currentGainTextBoxes[i].Text, out short iGain) ||
                    !short.TryParse(currentOffsetTextBoxes[i].Text, out short iOffset))
                {
                    MessageBox.Show("Failed to parse values for " + RailNames[i]);
                    return;
                }

                int railIndex = EditableRailIndices[i];
                deviceConfig.Calibration.PowerReadingVoltage[railIndex].GainOffset = vGain;
                deviceConfig.Calibration.PowerReadingVoltage[railIndex].Offset = vOffset;
                deviceConfig.Calibration.PowerReadingCurrent[railIndex].GainOffset = iGain;
                deviceConfig.Calibration.PowerReadingCurrent[railIndex].Offset = iOffset;
            }

            if (!pmd2_device.WriteConfig(deviceConfig))
            {
                MessageBox.Show("Failed to write device configuration");
                return;
            }
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            if (!pmd2_device.ConfigLoad())
            {
                MessageBox.Show("Failed to write device configuration");
                return;
            }

            System.Threading.Thread.Sleep(100);

            // Re-read the config
            buttonRead_Click(null, null);
        }

        private void buttonStore_Click(object sender, EventArgs e)
        {
            if(!pmd2_device.ConfigStore())
            {
                MessageBox.Show("Failed to write device configuration");
                return;
            }

            System.Threading.Thread.Sleep(100);

            // Re-load the config
            buttonLoad_Click(null, null);
        }

    }
}
