using System;
using System.Windows.Forms;

namespace IEC60870Driver
{
    public partial class ctlChannelAddress : UserControl
    {
        private readonly ATDriver driver;

        #region PROPERTIES

        public uint LifeTime
        {
            get
            {
                if (rbPermanent.Checked) return 0;
                return (uint)nudLifetime.Value;
            }
            set
            {
                if (value == 0)
                {
                    rbPermanent.Checked = true;
                }
                else
                {
                    rbCustom.Checked = true;
                    nudLifetime.Value = Math.Min(value, nudLifetime.Maximum);
                }
            }
        }

        #endregion

        public ctlChannelAddress(ATDriver driver)
        {
            InitializeComponent();
            this.driver = driver;

            // Initialize controls
            nudLifetime.Minimum = 60;  // 1 minute minimum
            nudLifetime.Maximum = 86400; // 24 hours maximum
            nudLifetime.Value = 3600; // 1 hour default

            // Event handlers
            KeyPress += (sender, e) => CheckKey(e.KeyChar);
            btnOK.Click += (sender, e) => UpdateChannel();
            rbPermanent.CheckedChanged += RadioButton_CheckedChanged;
            rbCustom.CheckedChanged += RadioButton_CheckedChanged;

            // Initialize with current value
            if (!string.IsNullOrEmpty(driver.ChannelAddress))
            {
                if (uint.TryParse(driver.ChannelAddress, out uint current))
                {
                    LifeTime = current;
                }
            }

            UpdateControls();
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void UpdateControls()
        {
            nudLifetime.Enabled = rbCustom.Checked;
            lblSeconds.Enabled = rbCustom.Checked;

            if (rbPermanent.Checked)
            {
                lblDescription.Text = "Connection will be kept alive permanently until manually disconnected.";
            }
            else
            {
                lblDescription.Text = $"Connection will be refreshed every {nudLifetime.Value} seconds to ensure reliability.";
            }
        }

        private void CheckKey(char keyChar)
        {
            if (keyChar == (char)13) // Enter
            {
                UpdateChannel();
            }
            else if (keyChar == (char)27) // Escape
            {
                Parent?.Dispose();
            }
        }

        private void UpdateChannel()
        {
            try
            {
                driver.ChannelAddress = LifeTime.ToString();
                MessageBox.Show($"Channel settings updated successfully.\nConnection lifetime: {(LifeTime == 0 ? "Permanent" : $"{LifeTime} seconds")}",
                    "IEC60870 Driver", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Parent?.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating channel settings: {ex.Message}",
                    "IEC60870 Driver", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}