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
            get => txtTagName.Text.Trim();
            set => txtTagName.Text = value.Trim();
        }

        public string TagAddress
        {
            get => nudIOA.Value.ToString(); // ✅ Chỉ trả về IOA number
            set => ParseTagAddress(value);
        }

        public DataType TagType
        {
            get
            {
                if (cbxDataType.SelectedItem is DataType dataType)
                    return dataType;
                return DataType.Default;
            }
            set => cbxDataType.SelectedItem = value;
        }

        public string Description
        {
            get => txtDescription.Text.Trim();
            set => txtDescription.Text = value;
        }

        public bool IsValid { get; private set; }

        #endregion

        public ctlTagDesign(ATDriver driver)
        {
            InitializeComponent();
            this.driver = driver;

            InitializeControls();

            Load += (sender, e) => Init();
            KeyPress += (sender, e) => CheckKey(e.KeyChar);

            btnOk.Click += (sender, e) => UpdateTag();
            btnCheck.Click += (sender, e) => TestTag();
            btnCancel.Click += (sender, e) => Parent?.Dispose();

            // Add validation events
            txtTagName.TextChanged += ValidateInputs;
            nudIOA.ValueChanged += ValidateInputs;
            cbxDataType.SelectedIndexChanged += ValidateInputs;
        }

        private void InitializeControls()
        {
            // ✅ IOA SETTINGS - CHỈ CẦN NHẬP TRỰC TIẾP
            nudIOA.Minimum = 0;
            nudIOA.Maximum = 16777215; // 3 bytes max
            nudIOA.Value = 1;

            // ✅ DATA TYPE - CHỈ CÁC LOẠI CƠ BẢN
            cbxDataType.Items.Clear();
            cbxDataType.Items.AddRange(new object[]
            {
                DataType.Bool,    // Single Point, Double Point
                DataType.Int,     // Normalized, Scaled Value
                DataType.Float,   // Short Float
                DataType.DWord,   // Counter, Binary State
                DataType.String   // Generic
            });
            cbxDataType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxDataType.SelectedIndex = 0; // Default Bool

            // ✅ EXAMPLES - THÔNG TIN THAM KHẢO
            lblExamples.Text = "Common IOA Ranges:\n" +
                              "• Digital Inputs: 1-999\n" +
                              "• Digital Outputs: 1000-1999\n" +
                              "• Analog Inputs: 2000-2999\n" +
                              "• Analog Outputs: 3000-3999\n" +
                              "• Counters: 4000-4999\n" +
                              "• System Commands: 0";
        }

        private void Init()
        {
            TagName = driver.TagNameDesignMode;

            if (!string.IsNullOrEmpty(driver.TagAddressDesignMode))
            {
                TagAddress = driver.TagAddressDesignMode;
            }

            if (!string.IsNullOrEmpty(driver.TagTypeDesignMode))
            {
                TagType = driver.TagTypeDesignMode.GetDataType();
            }

            ValidateInputs(null, null);
        }

        private void ValidateInputs(object sender, EventArgs e)
        {
            try
            {
                IsValid = !string.IsNullOrWhiteSpace(TagName) &&
                         nudIOA.Value >= nudIOA.Minimum &&
                         cbxDataType.SelectedItem != null;

                btnOk.Enabled = IsValid;
                btnCheck.Enabled = IsValid;

                if (IsValid)
                {
                    var ioa = (int)nudIOA.Value;
                    var dataType = (DataType)cbxDataType.SelectedItem;

                    Description = $"Tag Configuration:\n" +
                                 $"• Tag Name: {TagName}\n" +
                                 $"• IOA: {ioa}\n" +
                                 $"• Data Type: {dataType}\n" +
                                 $"• Range: {GetIOARange(ioa)}\n" +
                                 $"• TypeId: Auto-detected when reading\n" +
                                 $"• Access: Read/Write (depends on device)";
                }
                else
                {
                    Description = "Please fill in all required fields.";
                }
            }
            catch (Exception ex)
            {
                Description = $"Validation error: {ex.Message}";
                IsValid = false;
                btnOk.Enabled = false;
                btnCheck.Enabled = false;
            }
        }

        private string GetIOARange(int ioa)
        {
            if (ioa == 0) return "System/Broadcast";
            if (ioa >= 1 && ioa <= 999) return "Digital Inputs";
            if (ioa >= 1000 && ioa <= 1999) return "Digital Outputs";
            if (ioa >= 2000 && ioa <= 2999) return "Analog Inputs";
            if (ioa >= 3000 && ioa <= 3999) return "Analog Outputs";
            if (ioa >= 4000 && ioa <= 4999) return "Counters";
            return "Custom Range";
        }

        private void ParseTagAddress(string tagAddress)
        {
            try
            {
                // ✅ CHỈ PARSE IOA NUMBER
                if (int.TryParse(tagAddress, out int ioa))
                {
                    nudIOA.Value = Math.Max(nudIOA.Minimum, Math.Min(nudIOA.Maximum, ioa));
                }
                else
                {
                    // ✅ FALLBACK: Nếu là format cũ "TypeId:IOA"
                    var parts = tagAddress.Split(':');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int oldIoa))
                    {
                        nudIOA.Value = Math.Max(nudIOA.Minimum, Math.Min(nudIOA.Maximum, oldIoa));
                    }
                }
            }
            catch { }
        }

        private void CheckKey(char keyChar)
        {
            if (keyChar == (char)13) // Enter
            {
                UpdateTag();
            }
            else if (keyChar == (char)27) // Escape
            {
                Parent?.Dispose();
            }
        }

        private void UpdateTag()
        {
            if (!ValidateTag()) return;

            try
            {
                driver.TagNameDesignMode = TagName;
                driver.TagAddressDesignMode = TagAddress; // ✅ Chỉ là IOA number
                driver.TagTypeDesignMode = TagType.ToDisplayName();

                MessageBox.Show("Tag configuration saved successfully!",
                    "IEC60870 Driver", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Parent?.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving tag configuration: {ex.Message}",
                    "IEC60870 Driver", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TestTag()
        {
            if (!ValidateTag()) return;

            try
            {
                btnCheck.Enabled = false;
                btnCheck.Text = "Testing...";
                Application.DoEvents();

                var ioa = (int)nudIOA.Value;
                var dataType = (DataType)cbxDataType.SelectedItem;

                // ✅ SIMPLE VALIDATION TEST
                string validationResult = ValidateIOAAndType(ioa, dataType);

                MessageBox.Show($"Tag Validation Result:\n\n" +
                               $"• IOA: {ioa}\n" +
                               $"• Data Type: {dataType}\n" +
                               $"• Range: {GetIOARange(ioa)}\n" +
                               $"• Status: {validationResult}\n\n" +
                               $"Note: Actual communication test requires device connection.",
                    "IEC60870 Driver", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tag test error: {ex.Message}",
                    "IEC60870 Driver", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCheck.Enabled = true;
                btnCheck.Text = "Check";
            }
        }

        private string ValidateIOAAndType(int ioa, DataType dataType)
        {
            // ✅ BASIC VALIDATION LOGIC
            if (ioa == 0)
            {
                if (dataType == DataType.String)
                    return "✅ Valid - System command with String type";
                else
                    return "⚠️ Warning - System commands typically use String type";
            }

            if (ioa >= 1 && ioa <= 999)
            {
                if (dataType == DataType.Bool)
                    return "✅ Valid - Digital Input with Bool type";
                else
                    return "⚠️ Warning - Digital Inputs typically use Bool type";
            }

            if (ioa >= 1000 && ioa <= 1999)
            {
                if (dataType == DataType.Bool)
                    return "✅ Valid - Digital Output with Bool type";
                else
                    return "⚠️ Warning - Digital Outputs typically use Bool type";
            }

            if (ioa >= 2000 && ioa <= 2999)
            {
                if (dataType == DataType.Float || dataType == DataType.Int)
                    return "✅ Valid - Analog Input with numeric type";
                else
                    return "⚠️ Warning - Analog Inputs typically use Float or Int type";
            }

            if (ioa >= 3000 && ioa <= 3999)
            {
                if (dataType == DataType.Float || dataType == DataType.Int)
                    return "✅ Valid - Analog Output with numeric type";
                else
                    return "⚠️ Warning - Analog Outputs typically use Float or Int type";
            }

            if (ioa >= 4000 && ioa <= 4999)
            {
                if (dataType == DataType.DWord || dataType == DataType.Int)
                    return "✅ Valid - Counter with numeric type";
                else
                    return "⚠️ Warning - Counters typically use DWord or Int type";
            }

            return "✅ Valid - Custom IOA range";
        }

        private bool ValidateTag()
        {
            if (string.IsNullOrWhiteSpace(TagName))
            {
                MessageBox.Show("Tag name cannot be empty.",
                    "IEC60870 Driver", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTagName.Focus();
                return false;
            }

            if (cbxDataType.SelectedItem == null)
            {
                MessageBox.Show("Please select a data type.",
                    "IEC60870 Driver", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbxDataType.Focus();
                return false;
            }

            return true;
        }
    }
}