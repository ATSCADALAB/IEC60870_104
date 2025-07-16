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
            btnCancel.Click += (sender, e) => Parent?.Dispose();

            // Add validation events
            txtDeviceName.TextChanged += ValidateInputs;
            txtIpAddress.TextChanged += ValidateInputs;
            nudPort.ValueChanged += ValidateInputs;
            nudCommonAddress.ValueChanged += ValidateInputs;
            nudOriginatorAddress.ValueChanged += ValidateInputs;
            cbxCotFieldLength.SelectedIndexChanged += ValidateInputs;
            cbxCaFieldLength.SelectedIndexChanged += ValidateInputs;
            cbxIoaFieldLength.SelectedIndexChanged += ValidateInputs;
        }

        private void InitializeControls()
        {
            // ✅ BASIC CONNECTION
            txtIpAddress.Text = "192.168.1.63";
            nudPort.Value = 2404;

            // ✅ PROTOCOL SETTINGS
            nudCommonAddress.Minimum = 0;
            nudCommonAddress.Maximum = 65535;
            nudCommonAddress.Value = 1;

            nudOriginatorAddress.Minimum = 0;
            nudOriginatorAddress.Maximum = 255;
            nudOriginatorAddress.Value = 0;

            // ✅ COT Field Length - COMBOBOX
            cbxCotFieldLength.Items.Clear();
            cbxCotFieldLength.Items.AddRange(new object[] {
                new { Text = "1 byte", Value = 1 },
                new { Text = "2 bytes", Value = 2 }
            });
            cbxCotFieldLength.DisplayMember = "Text";
            cbxCotFieldLength.ValueMember = "Value";
            cbxCotFieldLength.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxCotFieldLength.SelectedIndex = 1; // Default: 2 bytes

            // ✅ CA Field Length - COMBOBOX  
            cbxCaFieldLength.Items.Clear();
            cbxCaFieldLength.Items.AddRange(new object[] {
                new { Text = "1 byte", Value = 1 },
                new { Text = "2 bytes", Value = 2 }
            });
            cbxCaFieldLength.DisplayMember = "Text";
            cbxCaFieldLength.ValueMember = "Value";
            cbxCaFieldLength.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxCaFieldLength.SelectedIndex = 1; // Default: 2 bytes

            // ✅ IOA Field Length - COMBOBOX
            cbxIoaFieldLength.Items.Clear();
            cbxIoaFieldLength.Items.AddRange(new object[] {
                new { Text = "1 byte (1-255)", Value = 1 },
                new { Text = "2 bytes (1-65535)", Value = 2 },
                new { Text = "3 bytes (1-16777215)", Value = 3 }
            });
            cbxIoaFieldLength.DisplayMember = "Text";
            cbxIoaFieldLength.ValueMember = "Value";
            cbxIoaFieldLength.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxIoaFieldLength.SelectedIndex = 2; // Default: 3 bytes
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
                         System.Net.IPAddress.TryParse(txtIpAddress.Text, out _) &&
                         cbxCotFieldLength.SelectedItem != null &&
                         cbxCaFieldLength.SelectedItem != null &&
                         cbxIoaFieldLength.SelectedItem != null;

                btnOk.Enabled = IsValid;
                btnTest.Enabled = IsValid;

                if (IsValid)
                {
                    Description = $"IEC60870-5-104 Device:\n" +
                                 $"• IP: {txtIpAddress.Text}:{nudPort.Value}\n" +
                                 $"• Common Address: {nudCommonAddress.Value}\n" +
                                 $"• Originator Address: {nudOriginatorAddress.Value}\n" +
                                 $"• COT Length: {GetSelectedValue(cbxCotFieldLength)} bytes\n" +
                                 $"• CA Length: {GetSelectedValue(cbxCaFieldLength)} bytes\n" +
                                 $"• IOA Length: {GetSelectedValue(cbxIoaFieldLength)} bytes\n" +
                                 $"• Max IOA Range: 1 - {GetMaxIOARange()}";
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

        private int GetSelectedValue(ComboBox comboBox)
        {
            if (comboBox.SelectedItem != null)
            {
                var item = comboBox.SelectedItem;
                var valueProperty = item.GetType().GetProperty("Value");
                return (int)valueProperty.GetValue(item);
            }
            return 1;
        }

        private int GetMaxIOARange()
        {
            int ioaLength = GetSelectedValue(cbxIoaFieldLength);
            switch (ioaLength)
            {
                case 1: return 255;
                case 2: return 65535;
                case 3: return 16777215;
                default: return 255;
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
                    GetSelectedValue(cbxCotFieldLength).ToString(),
                    GetSelectedValue(cbxCaFieldLength).ToString(),
                    GetSelectedValue(cbxIoaFieldLength).ToString()
                };

                return string.Join("|", parts);
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

                // Set combobox selections
                SetComboBoxValue(cbxCotFieldLength, int.Parse(parts[4]));
                SetComboBoxValue(cbxCaFieldLength, int.Parse(parts[5]));
                SetComboBoxValue(cbxIoaFieldLength, int.Parse(parts[6]));
            }
            catch { }
        }

        private void SetComboBoxValue(ComboBox comboBox, int value)
        {
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                var item = comboBox.Items[i];
                var valueProperty = item.GetType().GetProperty("Value");
                if ((int)valueProperty.GetValue(item) == value)
                {
                    comboBox.SelectedIndex = i;
                    break;
                }
            }
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

            return true;
        }
    }
}