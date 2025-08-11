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
            //  BASIC CONNECTION
            txtIpAddress.Text = "192.168.1.63";
            nudPort.Value = 2404;

            //  PROTOCOL SETTINGS
            nudCommonAddress.Minimum = 0;
            nudCommonAddress.Maximum = 65535;
            nudCommonAddress.Value = 1;

            nudOriginatorAddress.Minimum = 0;
            nudOriginatorAddress.Maximum = 255;
            nudOriginatorAddress.Value = 0;

            //  COT Field Length - COMBOBOX
            cbxCotFieldLength.Items.Clear();
            cbxCotFieldLength.Items.AddRange(new object[] {
                new { Text = "1 byte", Value = 1 },
                new { Text = "2 bytes", Value = 2 }
            });
            cbxCotFieldLength.DisplayMember = "Text";
            cbxCotFieldLength.ValueMember = "Value";
            cbxCotFieldLength.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxCotFieldLength.SelectedIndex = 1; // Default: 2 bytes

            //  CA Field Length - COMBOBOX  
            cbxCaFieldLength.Items.Clear();
            cbxCaFieldLength.Items.AddRange(new object[] {
                new { Text = "1 byte", Value = 1 },
                new { Text = "2 bytes", Value = 2 }
            });
            cbxCaFieldLength.DisplayMember = "Text";
            cbxCaFieldLength.ValueMember = "Value";
            cbxCaFieldLength.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxCaFieldLength.SelectedIndex = 1; // Default: 2 bytes

            //  IOA Field Length - COMBOBOX
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

                if (IsValid)
                {
                }
                else
                {
                    
                }
            }
            catch (Exception ex)
            {
                IsValid = false;
                btnOk.Enabled = false;
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
                // Tạo DeviceSettings object với tất cả thông tin
                var settings = new DeviceSettings
                {
                    IpAddress = txtIpAddress.Text.Trim(),
                    Port = (int)nudPort.Value,
                    CommonAddress = (int)nudCommonAddress.Value,
                    OriginatorAddress = (int)nudOriginatorAddress.Value,
                    CotFieldLength = GetSelectedValue(cbxCotFieldLength),
                    CommonAddressFieldLength = GetSelectedValue(cbxCaFieldLength),
                    IoaFieldLength = GetSelectedValue(cbxIoaFieldLength),
                    MaxReadTimes = 1,
                    BlockSettings = "",

                    // THÊM: Timeout values từ UI (nếu có controls)
                    ConnectionTimeout = GetTimeoutValue("Connection", 10000),
                    // ReadTimeout hidden in UI and not included in DeviceID anymore
                    InterrogationTimeout = GetTimeoutValue("Interrogation", 8000),
                    PingTimeout = GetTimeoutValue("Ping", 3000),
                    RetryDelay = GetTimeoutValue("RetryDelay", 500),

                    // THÊM: Missing tag settings từ UI (nếu có controls)
                    SkipMissingTags = GetBooleanValue("SkipMissingTags", true),
                    MissingTagValue = GetStringValue("MissingTagValue", "BAD"),
                   
                };

                return settings.GenerateDeviceID();
            }
            catch
            {
                return string.Empty;
            }
        }

        // Helper methods để lấy giá trị từ UI controls
        private int GetTimeoutValue(string controlName, int defaultValue)
        {
            try
            {
                switch (controlName)
                {
                    case "Connection": return (int)nudConnectionTimeout.Value;
                    case "Interrogation": return 8000; // Default if hidden
                    case "Ping": return 3000; // Default ping timeout
                    case "RetryDelay": return 500; // Default retry delay
                    default: return defaultValue;
                }
            }
            catch
            {
                return defaultValue;
            }
        }

        private bool GetBooleanValue(string controlName, bool defaultValue)
        {
            // Không còn boolean controls nào
            return defaultValue;
        }

        private string GetStringValue(string controlName, string defaultValue)
        {
            // Không có string controls nào hiện tại
            return defaultValue;
        }

        // Helper methods để set giá trị vào UI controls
        private void SetTimeoutValue(string controlName, int value)
        {
            try
            {
                switch (controlName)
                {
                    case "Connection": nudConnectionTimeout.Value = Math.Max(nudConnectionTimeout.Minimum, Math.Min(nudConnectionTimeout.Maximum, value)); break;
                    
                }
            }
            catch { }
        }

        private void SetBooleanValue(string controlName, bool value)
        {
            // Không còn boolean controls nào
        }

        private void SetStringValue(string controlName, string value)
        {
            // Không có string controls nào hiện tại
        }

        private void ParseDeviceID(string deviceID)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceID)) return;

                // Sử dụng DeviceSettings.Initialize để parse
                var settings = DeviceSettings.Initialize(deviceID);
                if (settings == null) return;

                // Basic settings
                txtIpAddress.Text = settings.IpAddress;
                nudPort.Value = settings.Port;
                nudCommonAddress.Value = settings.CommonAddress;
                nudOriginatorAddress.Value = settings.OriginatorAddress;

                // Set combobox selections
                SetComboBoxValue(cbxCotFieldLength, settings.CotFieldLength);
                SetComboBoxValue(cbxCaFieldLength, settings.CommonAddressFieldLength);
                SetComboBoxValue(cbxIoaFieldLength, settings.IoaFieldLength);

                // Set timeout và missing tag values từ settings
                SetTimeoutValue("Connection", settings.ConnectionTimeout);
                SetTimeoutValue("Read", settings.ReadTimeout);
                SetTimeoutValue("MaxRetry", settings.MaxRetryCount);
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