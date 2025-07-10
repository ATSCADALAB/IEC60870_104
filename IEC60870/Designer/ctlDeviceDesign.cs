using System;
using System.Windows.Forms;

namespace IEC60870Driver
{
    public partial class ctlDeviceDesign : UserControl
    {
        private readonly ATDriver driver;

        #region PROPERTIES

        public string DeviceName
        {
            get => this.txtDeviceName.Text.Trim();
            set => this.txtDeviceName.Text = value;
        }

        public string DeviceID
        {
            get => this.txtDeviceID.Text.Trim();
            set => this.txtDeviceID.Text = value;
        }

        public string Description
        {
            get => this.txtDescription.Text.Trim();
            set => this.txtDescription.Text = value;
        }

        public bool IsValid
        {
            get => this.btnOk.Enabled;
            set => this.btnOk.Enabled = value;
        }

        #endregion

        public ctlDeviceDesign(ATDriver driver)
        {
            InitializeComponent();
            this.driver = driver;

            Load += (sender, e) => Init();
            KeyPress += (sender, e) => CheckKey(e.KeyChar);

            btnOk.Enabled = false;
            btnOk.KeyPress += (sender, e) => CheckKey(e.KeyChar);
            btnOk.Click += (sender, e) => UpdateDevice();

            btnCheck.Click += (sender, e) => CheckDevice();
        }

        private void Init()
        {
            DeviceName = driver.DeviceNameDesignMode;
            DeviceID = driver.DeviceIDDesignMode;

            if (string.IsNullOrEmpty(driver.DeviceIDDesignMode))
            {
                btnOk.Enabled = true;
                return;
            }

            var deviceSettings = DeviceSettings.Initialize(DeviceID);
            IsValid = deviceSettings != null;
            Description = IsValid ? $"IEC60870 Device: {deviceSettings.IpAddress}:{deviceSettings.Port}" : "Invalid device configuration";
        }

        private void CheckKey(char keyChar)
        {
            if (keyChar == (char)13)
            {
                UpdateDevice();
            }
            else if (keyChar == (char)27)
            {
                Parent.Dispose();
            }
        }

        private void UpdateDevice()
        {
            if (!CheckProperties()) return;
            driver.DeviceNameDesignMode = DeviceName;
            driver.DeviceIDDesignMode = DeviceID;
            Parent.Dispose();
        }

        private void CheckDevice()
        {
            var deviceSettings = DeviceSettings.Initialize(DeviceID);
            IsValid = deviceSettings != null;
            Description = IsValid ? $"IEC60870 Device: {deviceSettings.IpAddress}:{deviceSettings.Port}" : "Invalid device configuration";
        }

        private bool CheckProperties()
        {
            if (string.IsNullOrEmpty(DeviceName))
            {
                MessageBox.Show("Device name cannot be empty.", "ATSCADA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(DeviceID))
            {
                MessageBox.Show("Device ID cannot be empty.", "ATSCADA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
    }
}