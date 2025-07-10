using System;
using System.Windows.Forms;

namespace IEC60870Driver
{
    public partial class ctlTagDesign : UserControl
    {
        private readonly ATDriver driver;

        #region PROPERTIES

        public string TagName
        {
            get => this.txtTagName.Text.Trim();
            set => this.txtTagName.Text = value.Trim();
        }

        public string TagAddress
        {
            get => this.txtTagAddress.Text.Trim();
            set => this.txtTagAddress.Text = value.Trim();
        }

        public DataType TagType
        {
            get
            {
                var item = this.cbxDataType.SelectedItem;
                if (item is null) return DataType.Default;
                return item.ToString().GetDataType();
            }
            set => this.cbxDataType.SelectedItem = value.ToDisplayName();
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

        public ctlTagDesign(ATDriver driver)
        {
            InitializeComponent();
            this.driver = driver;

            Load += (sender, e) => Init();
            KeyPress += (sender, e) => CheckKey(e.KeyChar);

            btnOk.Enabled = false;
            btnOk.KeyPress += (sender, e) => CheckKey(e.KeyChar);
            btnOk.Click += (sender, e) => UpdateTag();

            btnCheck.Click += (sender, e) => CheckTag();
            cbxDataType.DataSource = Enum.GetNames(typeof(DataType));
            cbxDataType.SelectedIndex = 0;
        }

        private void Init()
        {
            TagName = driver.TagNameDesignMode;
            TagAddress = driver.TagAddressDesignMode;
            TagType = driver.TagTypeDesignMode.GetDataType();

            if (string.IsNullOrEmpty(driver.TagAddressDesignMode))
            {
                btnOk.Enabled = true;
                return;
            }

            var tagType = TagType;
            IsValid = TagAddress.GetIOAddress(tagType, out string description) != null;
            TagType = tagType;
            Description = description;
        }

        private void CheckKey(char keyChar)
        {
            if (keyChar == (char)13)
            {
                UpdateTag();
            }
            else if (keyChar == (char)27)
            {
                Parent.Dispose();
            }
        }

        private void UpdateTag()
        {
            if (!CheckProperties()) return;
            driver.TagNameDesignMode = TagName;
            driver.TagAddressDesignMode = TagAddress;
            driver.TagTypeDesignMode = TagType.ToDisplayName();
            Parent.Dispose();
        }

        private void CheckTag()
        {
            var tagType = TagType;
            IsValid = TagAddress.GetIOAddress(tagType, out string description) != null;
            TagType = tagType;
            Description = description;
        }

        private bool CheckProperties()
        {
            if (string.IsNullOrEmpty(TagName))
            {
                MessageBox.Show("Tag name cannot be empty.", "ATSCADA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(TagAddress))
            {
                MessageBox.Show("Tag address cannot be empty.", "ATSCADA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
    }
}