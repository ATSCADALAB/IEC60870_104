using System;
using System.Linq;
using System.Windows.Forms;

namespace IEC60870Driver
{
    public partial class ctlDeviceDesign : UserControl
    {
        private readonly ATDriver driver;

        #region PROPERTIES

        public string DeviceName
        {
            get => txtDeviceName.Text.Trim();
            set => txtDeviceName.Text = value;
        }

        public string DeviceID
        {
            get => GenerateDeviceID();
            set => ParseDeviceID(value);
        }

        public string Description
        {
            get => txtDescription.Text.Trim();
            set => txtDescription.Text = value;
        }

        public bool IsValid { get; private set; }

        #endregion

        public ctlDeviceDesign(ATDriver driver)
        {
            InitializeComponent();
            this.driver = driver;

            InitializeControls();
            Load += (sender, e) => Init();
            KeyPress += (sender, e) => CheckKey(e.KeyChar);

            btnOk.Click += (sender, e) => UpdateDevice();
            btnTest.Click += (sender, e) => TestConnection();

            // Add validation events
            txtDeviceName.TextChanged += ValidateInputs;
            txtIpAddress.TextChanged += ValidateInputs;
            nudPort.ValueChanged += ValidateInputs;
            nudCommonAddress.ValueChanged += ValidateInputs;
        }

        private void InitializeControls()
        {
            // Initialize default values
            txtIpAddress.Text = "192.168.1.100";
            nudPort.Value = 2404;
            nudCommonAddress.Minimum = 0;
            nudCommonAddress.Maximum = 65535;
            nudCommonAddress.Value = 1;

            nudOriginatorAddress.Minimum = 0;
            nudOriginatorAddress.Maximum = 255;
            nudOriginatorAddress.Value = 0;

            // COT Field Length dropdown
            cbxCotFieldLength.Items.Clear();
            cbxCotFieldLength.Items.AddRange(new object[] { 1, 2 });
            cbxCotFieldLength.SelectedItem = 1;

            // Common Address Field Length dropdown
            cbxCaFieldLength.Items.Clear();
            cbxCaFieldLength.Items.AddRange(new object[] { 1, 2 });
            cbxCaFieldLength.SelectedItem = 1;

            // IOA Field Length dropdown
            cbxIoaFieldLength.Items.Clear();
            cbxIoaFieldLength.Items.AddRange(new object[] { 1, 2, 3 });
            cbxIoaFieldLength.SelectedItem = 2;

            // Max Read Times
            nudMaxReadTimes.Minimum = 1;
            nudMaxReadTimes.Maximum = 100;
            nudMaxReadTimes.Value = 1;

            // Block settings examples
            cbxBlockSettings.Items.Clear();
            cbxBlockSettings.Items.AddRange(new object[]
            {
                "",
                "M_SP_NA_1-1-100",
                "M_ME_NC_1-1-50",
                "M_SP_NA_1-1-100/M_ME_NC_1-101-200",
                "M_DP_NA_1-1-20/M_ME_NA_1-101-150/C_SC_NA_1-1001-1050",
                "M_SP_NA_1-1-500/M_ME_NC_1-1001-1100/C_SC_NA_1-2001-2100"
            });
            cbxBlockSettings.SelectedIndex = 0;
            cbxBlockSettings.DropDownStyle = ComboBoxStyle.DropDown; // Allow custom input
        }

        private void Init()
        {
            DeviceName = driver.DeviceNameDesignMode;

            if (!string.IsNullOrEmpty(driver.DeviceIDDesignMode))
            {
                DeviceID = driver.DeviceIDDesignMode;
            }

            ValidateInputs(null, null);
        }

        private void ValidateInputs(object sender, EventArgs e)
        {
            try
            {
                IsValid = !string.IsNullOrWhiteSpace(DeviceName) &&
                         !string.IsNullOrWhiteSpace(txtIpAddress.Text) &&
                         System.Net.IPAddress.TryParse(txtIpAddress.Text, out _);

                btnOk.Enabled = IsValid;
                btnTest.Enabled = IsValid;

                if (IsValid)
                {
                    var deviceSettings = DeviceSettings.Initialize(DeviceID);
                    Description = deviceSettings != null ?
                        deviceSettings.GetDetailedDescription() :
                        "Invalid configuration";
                }
                else
                {
                    Description = "Please fill in all required fields correctly.";
                }
            }
            catch (Exception ex)
            {
                Description = $"Validation error: {ex.Message}";
                IsValid = false;
                btnOk.Enabled = false;
                btnTest.Enabled = false;
            }
        }

        private string GenerateDeviceID()
        {
            try
            {
                var parts = new[]
                {
                    txtIpAddress.Text.Trim(),
                    nudPort.Value.ToString(),
                    nudCommonAddress.Value.ToString(),
                    nudOriginatorAddress.Value.ToString(),
                    cbxCotFieldLength.SelectedItem?.ToString() ?? "1",
                    cbxCaFieldLength.SelectedItem?.ToString() ?? "1",
                    cbxIoaFieldLength.SelectedItem?.ToString() ?? "2",
                    nudMaxReadTimes.Value.ToString(),
                    cbxBlockSettings.Text.Trim()
                };

                return string.Join("|", parts.Take(string.IsNullOrWhiteSpace(parts[8]) ? 8 : 9));
            }
            catch
            {
                return string.Empty;
            }
        }

        private void ParseDeviceID(string deviceID)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceID)) return;

                var parts = deviceID.Split('|');
                if (parts.Length < 7) return;

                txtIpAddress.Text = parts[0];
                nudPort.Value = int.Parse(parts[1]);
                nudCommonAddress.Value = int.Parse(parts[2]);
                nudOriginatorAddress.Value = int.Parse(parts[3]);
                cbxCotFieldLength.SelectedItem = int.Parse(parts[4]);
                cbxCaFieldLength.SelectedItem = int.Parse(parts[5]);
                cbxIoaFieldLength.SelectedItem = int.Parse(parts[6]);

                if (parts.Length > 7)
                    nudMaxReadTimes.Value = int.Parse(parts[7]);

                if (parts.Length > 8)
                    cbxBlockSettings.Text = parts[8];
            }
            catch { }
        }

        private void CheckKey(char keyChar)
        {
            if (keyChar == (char)13) // Enter
            {
                UpdateDevice();
            }
            else if (keyChar == (char)27) // Escape
            {
                Parent?.Dispose();
            }
        }

        private void UpdateDevice()
        {
            if (!ValidateDevice()) return;

            try
            {
                driver.DeviceNameDesignMode = DeviceName;
                driver.DeviceIDDesignMode = DeviceID;

                MessageBox.Show("Device configuration saved successfully!",
                    "IEC60870 Driver", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Parent?.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving device configuration: {ex.Message}",
                    "IEC60870 Driver", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TestConnection()
        {
            if (!ValidateDevice()) return;

            try
            {
                var deviceSettings = DeviceSettings.Initialize(DeviceID);
                if (deviceSettings == null)
                {
                    MessageBox.Show("Invalid device configuration!",
                        "IEC60870 Driver", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                btnTest.Enabled = false;
                btnTest.Text = "Testing...";
                Application.DoEvents();

                // Test connection logic here
                using (var client = new IEC60870Client())
                {
                    client.IpAddress = deviceSettings.IpAddress;
                    client.Port = deviceSettings.Port;
                    client.CommonAddress = deviceSettings.CommonAddress;
                    client.OriginatorAddress = deviceSettings.OriginatorAddress;

                    if (client.Connect())
                    {
                        MessageBox.Show("Connection test successful!",
                            "IEC60870 Driver", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        client.Disconnect();
                    }
                    else
                    {
                        MessageBox.Show("Connection test failed!\nPlease check IP address and port.",
                            "IEC60870 Driver", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection test error: {ex.Message}",
                    "IEC60870 Driver", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnTest.Enabled = true;
                btnTest.Text = "Test";
            }
        }

        private bool ValidateDevice()
        {
            if (string.IsNullOrWhiteSpace(DeviceName))
            {
                MessageBox.Show("Device name cannot be empty.",
                    "IEC60870 Driver", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDeviceName.Focus();
                return false;
            }

            if (!System.Net.IPAddress.TryParse(txtIpAddress.Text, out _))
            {
                MessageBox.Show("Please enter a valid IP address.",
                    "IEC60870 Driver", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtIpAddress.Focus();
                return false;
            }

            // Validate block settings if provided
            if (!string.IsNullOrWhiteSpace(cbxBlockSettings.Text))
            {
                var validationError = DeviceSettings.ValidateBlockSettings(cbxBlockSettings.Text);
                if (validationError != null)
                {
                    MessageBox.Show($"Block settings error: {validationError}",
                        "IEC60870 Driver", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cbxBlockSettings.Focus();
                    return false;
                }
            }

            return true;
        }
    }
}