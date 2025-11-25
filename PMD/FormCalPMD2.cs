using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PMD.PMD2_Device;

namespace PMD
{
    using DeviceConfigStruct = DeviceConfigStructV1; // Use V1 as the current version

    public partial class FormCalPMD2 : Form
    {

        PMD2_Device pmd2_device;
        DeviceConfigStruct deviceConfig;

        public FormCalPMD2(PMD2_Device pmd2_device)
        {
            InitializeComponent();
            this.pmd2_device = pmd2_device;
        }

        private void buttonRead_Click(object sender, EventArgs e)
        {
            if(!pmd2_device.ReadConfig(out deviceConfig))
            {
                MessageBox.Show("Failed to read device configuration");
                return;
            }

            textBoxAtx12Vgain.Text = deviceConfig.Calibration.PowerReadingVoltage[0].GainOffset.ToString();
            textBoxAtx12Voffset.Text = deviceConfig.Calibration.PowerReadingVoltage[0].Offset.ToString();
            textBoxAtx12Igain.Text = deviceConfig.Calibration.PowerReadingCurrent[0].GainOffset.ToString();
            textBoxAtx12Ioffset.Text = deviceConfig.Calibration.PowerReadingCurrent[0].Offset.ToString();

            textBoxAtx5Vgain.Text = deviceConfig.Calibration.PowerReadingVoltage[1].GainOffset.ToString();
            textBoxAtx5Voffset.Text = deviceConfig.Calibration.PowerReadingVoltage[1].Offset.ToString();
            textBoxAtx5Igain.Text = deviceConfig.Calibration.PowerReadingCurrent[1].GainOffset.ToString();
            textBoxAtx5Ioffset.Text = deviceConfig.Calibration.PowerReadingCurrent[1].Offset.ToString();

            textBoxAtx5sbVgain.Text = deviceConfig.Calibration.PowerReadingVoltage[2].GainOffset.ToString();
            textBoxAtx5sbVoffset.Text = deviceConfig.Calibration.PowerReadingVoltage[2].Offset.ToString();
            textBoxAtx5sbIgain.Text = deviceConfig.Calibration.PowerReadingCurrent[2].GainOffset.ToString();
            textBoxAtx5sbIoffset.Text = deviceConfig.Calibration.PowerReadingCurrent[2].Offset.ToString();

            textBoxAtx3Vgain.Text = deviceConfig.Calibration.PowerReadingVoltage[3].GainOffset.ToString();
            textBoxAtx3Voffset.Text = deviceConfig.Calibration.PowerReadingVoltage[3].Offset.ToString();
            textBoxAtx3Igain.Text = deviceConfig.Calibration.PowerReadingCurrent[3].GainOffset.ToString();
            textBoxAtx3Ioffset.Text = deviceConfig.Calibration.PowerReadingCurrent[3].Offset.ToString();

            textBoxHpwrVgain.Text = deviceConfig.Calibration.PowerReadingVoltage[4].GainOffset.ToString();
            textBoxHpwrVoffset.Text = deviceConfig.Calibration.PowerReadingVoltage[4].Offset.ToString();
            textBoxHpwrIgain.Text = deviceConfig.Calibration.PowerReadingCurrent[4].GainOffset.ToString();
            textBoxHpwrIoffset.Text = deviceConfig.Calibration.PowerReadingCurrent[4].Offset.ToString();

            textBoxEps1Vgain.Text = deviceConfig.Calibration.PowerReadingVoltage[5].GainOffset.ToString();
            textBoxEps1Voffset.Text = deviceConfig.Calibration.PowerReadingVoltage[5].Offset.ToString();
            textBoxEps1Igain.Text = deviceConfig.Calibration.PowerReadingCurrent[5].GainOffset.ToString();
            textBoxEps1Ioffset.Text = deviceConfig.Calibration.PowerReadingCurrent[5].Offset.ToString();

            textBoxEps2Vgain.Text = deviceConfig.Calibration.PowerReadingVoltage[6].GainOffset.ToString();
            textBoxEps2Voffset.Text = deviceConfig.Calibration.PowerReadingVoltage[6].Offset.ToString();
            textBoxEps2Igain.Text = deviceConfig.Calibration.PowerReadingCurrent[6].GainOffset.ToString();
            textBoxEps2Ioffset.Text = deviceConfig.Calibration.PowerReadingCurrent[6].Offset.ToString();

            textBoxPcie1Vgain.Text = deviceConfig.Calibration.PowerReadingVoltage[7].GainOffset.ToString();
            textBoxPcie1Voffset.Text = deviceConfig.Calibration.PowerReadingVoltage[7].Offset.ToString();
            textBoxPcie1Igain.Text = deviceConfig.Calibration.PowerReadingCurrent[7].GainOffset.ToString();
            textBoxPcie1Ioffset.Text = deviceConfig.Calibration.PowerReadingCurrent[7].Offset.ToString();

            textBoxPcie2Vgain.Text = deviceConfig.Calibration.PowerReadingVoltage[8].GainOffset.ToString();
            textBoxPcie2Voffset.Text = deviceConfig.Calibration.PowerReadingVoltage[8].Offset.ToString();
            textBoxPcie2Igain.Text = deviceConfig.Calibration.PowerReadingCurrent[8].GainOffset.ToString();
            textBoxPcie2Ioffset.Text = deviceConfig.Calibration.PowerReadingCurrent[8].Offset.ToString();

            textBoxPcie3Vgain.Text = deviceConfig.Calibration.PowerReadingVoltage[9].GainOffset.ToString();
            textBoxPcie3Voffset.Text = deviceConfig.Calibration.PowerReadingVoltage[9].Offset.ToString();
            textBoxPcie3Igain.Text = deviceConfig.Calibration.PowerReadingCurrent[9].GainOffset.ToString();
            textBoxPcie3Ioffset.Text = deviceConfig.Calibration.PowerReadingCurrent[9].Offset.ToString();

        }

        private void buttonWrite_Click(object sender, EventArgs e)
        {
            // Parse all the values
            if(!int.TryParse(textBoxAtx12Vgain.Text, out int atx12Vgain) ||
                !int.TryParse(textBoxAtx12Voffset.Text, out int atx12Voffset) ||
                !int.TryParse(textBoxAtx12Igain.Text, out int atx12Igain) ||
                !int.TryParse(textBoxAtx12Ioffset.Text, out int atx12Ioffset) ||

                !int.TryParse(textBoxAtx5Vgain.Text, out int atx5Vgain) ||
                !int.TryParse(textBoxAtx5Voffset.Text, out int atx5Voffset) ||
                !int.TryParse(textBoxAtx5Igain.Text, out int atx5Igain) ||
                !int.TryParse(textBoxAtx5Ioffset.Text, out int atx5Ioffset) ||

                !int.TryParse(textBoxAtx5sbVgain.Text, out int atx5sbVgain) ||
                !int.TryParse(textBoxAtx5sbVoffset.Text, out int atx5sbVoffset) ||
                !int.TryParse(textBoxAtx5sbIgain.Text, out int atx5sbIgain) ||
                !int.TryParse(textBoxAtx5sbIoffset.Text, out int atx5sbIoffset) ||

                !int.TryParse(textBoxAtx3Vgain.Text, out int atx3Vgain) ||
                !int.TryParse(textBoxAtx3Voffset.Text, out int atx3Voffset) ||
                !int.TryParse(textBoxAtx3Igain.Text, out int atx3Igain) ||
                !int.TryParse(textBoxAtx3Ioffset.Text, out int atx3Ioffset) ||

                !int.TryParse(textBoxHpwrVgain.Text, out int hpwrVgain) ||
                !int.TryParse(textBoxHpwrVoffset.Text, out int hpwrVoffset) ||
                !int.TryParse(textBoxHpwrIgain.Text, out int hpwrIgain) ||
                !int.TryParse(textBoxHpwrIoffset.Text, out int hpwrIoffset) ||

                !int.TryParse(textBoxEps1Vgain.Text, out int eps1Vgain) ||
                !int.TryParse(textBoxEps1Voffset.Text, out int eps1Voffset) ||
                !int.TryParse(textBoxEps1Igain.Text, out int eps1Igain) ||
                !int.TryParse(textBoxEps1Ioffset.Text, out int eps1Ioffset) ||

                !int.TryParse(textBoxEps2Vgain.Text, out int eps2Vgain) ||
                !int.TryParse(textBoxEps2Voffset.Text, out int eps2Voffset) ||
                !int.TryParse(textBoxEps2Igain.Text, out int eps2Igain) ||
                !int.TryParse(textBoxEps2Ioffset.Text, out int eps2Ioffset) ||

                !int.TryParse(textBoxPcie1Vgain.Text, out int pcie1Vgain) ||
                !int.TryParse(textBoxPcie1Voffset.Text, out int pcie1Voffset) ||
                !int.TryParse(textBoxPcie1Igain.Text, out int pcie1Igain) ||
                !int.TryParse(textBoxPcie1Ioffset.Text, out int pcie1Ioffset) ||

                !int.TryParse(textBoxPcie2Vgain.Text, out int pcie2Vgain) ||
                !int.TryParse(textBoxPcie2Voffset.Text, out int pcie2Voffset) ||
                !int.TryParse(textBoxPcie2Igain.Text, out int pcie2Igain) ||
                !int.TryParse(textBoxPcie2Ioffset.Text, out int pcie2Ioffset) ||
                
                !int.TryParse(textBoxPcie3Vgain.Text, out int pcie3Vgain) ||
                !int.TryParse(textBoxPcie3Voffset.Text, out int pcie3Voffset) ||
                !int.TryParse(textBoxPcie3Igain.Text, out int pcie3Igain) ||
                !int.TryParse(textBoxPcie3Ioffset.Text, out int pcie3Ioffset))
            {
                MessageBox.Show("Failed to parse values");
                return;
            }
           
            // Assing all values to the struct

            deviceConfig.Calibration.PowerReadingVoltage[0].GainOffset = (Int16)atx12Vgain;
            deviceConfig.Calibration.PowerReadingVoltage[0].Offset = (Int16)atx12Voffset;
            deviceConfig.Calibration.PowerReadingCurrent[0].GainOffset = (Int16)atx12Igain;
            deviceConfig.Calibration.PowerReadingCurrent[0].Offset = (Int16)atx12Ioffset;

            deviceConfig.Calibration.PowerReadingVoltage[1].GainOffset = (Int16)atx5Vgain;
            deviceConfig.Calibration.PowerReadingVoltage[1].Offset = (Int16)atx5Voffset;
            deviceConfig.Calibration.PowerReadingCurrent[1].GainOffset = (Int16)atx5Igain;
            deviceConfig.Calibration.PowerReadingCurrent[1].Offset = (Int16)atx5Ioffset;

            deviceConfig.Calibration.PowerReadingVoltage[2].GainOffset = (Int16)atx5sbVgain;
            deviceConfig.Calibration.PowerReadingVoltage[2].Offset = (Int16)atx5sbVoffset;
            deviceConfig.Calibration.PowerReadingCurrent[2].GainOffset = (Int16)atx5sbIgain;
            deviceConfig.Calibration.PowerReadingCurrent[2].Offset = (Int16)atx5sbIoffset;

            deviceConfig.Calibration.PowerReadingVoltage[3].GainOffset = (Int16)atx3Vgain;
            deviceConfig.Calibration.PowerReadingVoltage[3].Offset = (Int16)atx3Voffset;
            deviceConfig.Calibration.PowerReadingCurrent[3].GainOffset = (Int16)atx3Igain;
            deviceConfig.Calibration.PowerReadingCurrent[3].Offset = (Int16)atx3Ioffset;

            deviceConfig.Calibration.PowerReadingVoltage[4].GainOffset = (Int16)hpwrVgain;
            deviceConfig.Calibration.PowerReadingVoltage[4].Offset = (Int16)hpwrVoffset;
            deviceConfig.Calibration.PowerReadingCurrent[4].GainOffset = (Int16)hpwrIgain;
            deviceConfig.Calibration.PowerReadingCurrent[4].Offset = (Int16)hpwrIoffset;

            deviceConfig.Calibration.PowerReadingVoltage[5].GainOffset = (Int16)eps1Vgain;
            deviceConfig.Calibration.PowerReadingVoltage[5].Offset = (Int16)eps1Voffset;
            deviceConfig.Calibration.PowerReadingCurrent[5].GainOffset = (Int16)eps1Igain;
            deviceConfig.Calibration.PowerReadingCurrent[5].Offset = (Int16)eps1Ioffset;

            deviceConfig.Calibration.PowerReadingVoltage[6].GainOffset = (Int16)eps2Vgain;
            deviceConfig.Calibration.PowerReadingVoltage[6].Offset = (Int16)eps2Voffset;
            deviceConfig.Calibration.PowerReadingCurrent[6].GainOffset = (Int16)eps2Igain;
            deviceConfig.Calibration.PowerReadingCurrent[6].Offset = (Int16)eps2Ioffset;

            deviceConfig.Calibration.PowerReadingVoltage[7].GainOffset = (Int16)pcie1Vgain;
            deviceConfig.Calibration.PowerReadingVoltage[7].Offset = (Int16)pcie1Voffset;
            deviceConfig.Calibration.PowerReadingCurrent[7].GainOffset = (Int16)pcie1Igain;
            deviceConfig.Calibration.PowerReadingCurrent[7].Offset = (Int16)pcie1Ioffset;

            deviceConfig.Calibration.PowerReadingVoltage[8].GainOffset = (Int16)pcie2Vgain;
            deviceConfig.Calibration.PowerReadingVoltage[8].Offset = (Int16)pcie2Voffset;
            deviceConfig.Calibration.PowerReadingCurrent[8].GainOffset = (Int16)pcie2Igain;
            deviceConfig.Calibration.PowerReadingCurrent[8].Offset = (Int16)pcie2Ioffset;

            deviceConfig.Calibration.PowerReadingVoltage[9].GainOffset = (Int16)pcie3Vgain;
            deviceConfig.Calibration.PowerReadingVoltage[9].Offset = (Int16)pcie3Voffset;
            deviceConfig.Calibration.PowerReadingCurrent[9].GainOffset = (Int16)pcie3Igain;
            deviceConfig.Calibration.PowerReadingCurrent[9].Offset = (Int16)pcie3Ioffset;

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
