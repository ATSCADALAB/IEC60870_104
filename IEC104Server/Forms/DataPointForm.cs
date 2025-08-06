// File: Forms/DataPointForm.cs - Sửa để sử dụng DataType và TypeId
using System;
using System.Linq;
using System.Windows.Forms;
using IEC60870.Enum;
using IEC60870ServerWinForm.Models;

namespace IEC60870ServerWinForm.Forms
{
    public partial class DataPointForm : Form
    {
        public DataPoint DataPoint { get; private set; }
        private bool _isEditMode;

        public DataPointForm(DataPoint dataPoint = null)
        {
            InitializeComponent();

            _isEditMode = dataPoint != null;
            DataPoint = dataPoint ?? new DataPoint();

            InitializeForm();
        }

        private void InitializeForm()
        {
            // ✅ Setup ComboBox cho DataType
            cmbDataType.Items.Clear();
            cmbDataType.Items.AddRange(Enum.GetValues(typeof(DataType)).Cast<object>().ToArray());
            cmbDataType.DropDownStyle = ComboBoxStyle.DropDownList;

            // ✅ Setup ComboBox cho TypeId (optional - nếu muốn cho user chọn trực tiếp)
            cmbTypeId.Items.Clear();
            var commonTypeIds = new[]
            {
                TypeId.M_SP_NA_1,  // Single point
                TypeId.M_DP_NA_1,  // Double point  
                TypeId.M_ME_NC_1,  // Float
                TypeId.M_ME_NB_1,  // Scaled value
                TypeId.M_ME_NA_1,  // Normalized value
                TypeId.M_IT_NA_1,  // Counter
                TypeId.M_BO_NA_1   // Bitstring
            };
            cmbTypeId.Items.AddRange(commonTypeIds.Cast<object>().ToArray());
            cmbTypeId.DropDownStyle = ComboBoxStyle.DropDownList;

            // Load data if editing
            if (_isEditMode)
            {
                LoadDataPoint();
            }
            else
            {
                // Set defaults for new data point
                cmbDataType.SelectedItem = DataType.Bool;
                DataPoint.SetDataType(DataType.Bool); // Auto set TypeId
            }

            // ✅ Event handlers cho auto-sync
            cmbDataType.SelectedIndexChanged += CmbDataType_SelectedIndexChanged;
            cmbTypeId.SelectedIndexChanged += CmbTypeId_SelectedIndexChanged;

            this.Text = _isEditMode ? "Edit Data Point" : "Add Data Point";
        }

        /// <summary>
        /// ✅ Khi user chọn DataType, tự động set TypeId tương ứng
        /// </summary>
        private void CmbDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDataType.SelectedItem is DataType selectedDataType)
            {
                // Auto set TypeId based on DataType
                var correspondingTypeId = DataPoint.GetTypeIdFromDataType(selectedDataType);
                cmbTypeId.SelectedItem = correspondingTypeId;

                // Update example
                UpdateExample(selectedDataType);
            }
        }

        /// <summary>
        /// ✅ Khi user chọn TypeId, tự động set DataType tương ứng
        /// </summary>
        private void CmbTypeId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTypeId.SelectedItem is TypeId selectedTypeId)
            {
                // Auto set DataType based on TypeId
                var correspondingDataType = DataPoint.GetDataTypeFromTypeId(selectedTypeId);

                // Prevent infinite loop
                cmbDataType.SelectedIndexChanged -= CmbDataType_SelectedIndexChanged;
                cmbDataType.SelectedItem = correspondingDataType;
                cmbDataType.SelectedIndexChanged += CmbDataType_SelectedIndexChanged;

                UpdateExample(correspondingDataType);
            }
        }

        /// <summary>
        /// ✅ Hiển thị ví dụ value cho từng DataType
        /// </summary>
        private void UpdateExample(DataType dataType)
        {
            string example = "";
            switch (dataType)
            {
                case DataType.Bool:
                    example = "Examples: 1, 0, true, false, on, off";
                    break;
                case DataType.Int:
                    example = "Examples: 123, -456, 0";
                    break;
                case DataType.Float:
                    example = "Examples: 25.5, -10.2, 100.0";
                    break;
                case DataType.Double:
                    example = "Examples: 123.456789, -987.654321";
                    break;
                case DataType.Counter:
                    example = "Examples: 12345, 0 (unsigned integer)";
                    break;
                case DataType.String:
                    example = "Examples: any text value";
                    break;
            }

            if (lblExample != null)
                lblExample.Text = example;
        }

        private void LoadDataPoint()
        {
            txtIOA.Text = DataPoint.IOA.ToString();
            txtName.Text = DataPoint.Name;
            txtDescription.Text = DataPoint.Description;
            txtTagPath.Text = DataPoint.DataTagName;

            // ✅ Load DataType và TypeId
            cmbDataType.SelectedItem = DataPoint.DataType;
            cmbTypeId.SelectedItem = DataPoint.Type;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                SaveDataPoint();
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private bool ValidateInput()
        {
            // Validate IOA
            if (!int.TryParse(txtIOA.Text, out int ioa) || ioa < 0)
            {
                MessageBox.Show("IOA must be a valid positive integer.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtIOA.Focus();
                return false;
            }

            // Validate Name
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Name is required.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            // Validate Tag Path
            if (string.IsNullOrWhiteSpace(txtTagPath.Text))
            {
                MessageBox.Show("Tag Path is required.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTagPath.Focus();
                return false;
            }

            // Validate DataType selection
            if (cmbDataType.SelectedItem == null)
            {
                MessageBox.Show("Please select a Data Type.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbDataType.Focus();
                return false;
            }

            return true;
        }

        private void SaveDataPoint()
        {
            DataPoint.IOA = int.Parse(txtIOA.Text);
            DataPoint.Name = txtName.Text.Trim();
            DataPoint.Description = txtDescription.Text.Trim();
            DataPoint.DataTagName = txtTagPath.Text.Trim();

            // ✅ QUAN TRỌNG: Sử dụng SetDataType thay vì gán trực tiếp
            if (cmbDataType.SelectedItem is DataType selectedDataType)
            {
                DataPoint.SetDataType(selectedDataType); // Tự động set cả DataType và TypeId
            }

            // ✅ Hoặc nếu user chọn TypeId trực tiếp:
            // if (cmbTypeId.SelectedItem is TypeId selectedTypeId)
            // {
            //     DataPoint.SetTypeId(selectedTypeId); // Tự động set cả TypeId và DataType
            // }

            DataPoint.LastUpdated = DateTime.Now;
        }

        /// <summary>
        /// ✅ HELPER: Kiểm tra tag path format
        /// </summary>
        private void btnTestTag_Click(object sender, EventArgs e)
        {
            string tagPath = txtTagPath.Text.Trim();
            if (string.IsNullOrEmpty(tagPath))
            {
                MessageBox.Show("Please enter a tag path first.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string format = GetTagPathFormat(tagPath);
            MessageBox.Show($"Tag Path: {tagPath}\nFormat: {format}", "Tag Path Analysis",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string GetTagPathFormat(string tagPath)
        {
            if (string.IsNullOrEmpty(tagPath))
                return "Empty";

            if (tagPath.Contains("."))
            {
                var parts = tagPath.Split('.');
                if (parts.Length == 2)
                    return $"Task.Tag format (Task: '{parts[0]}', Tag: '{parts[1]}')";
                else
                    return $"Complex format ({parts.Length} parts)";
            }
            else
            {
                return "Tag only (requires default task to be set)";
            }
        }

        #region Form Events

        /// <summary>
        /// Form Load event handler
        /// </summary>
        private void DataPointForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Set form title based on mode
                if (_isEditMode)
                {
                    this.Text = $"Edit Data Point - IOA {DataPoint.IOA}";
                }
                else
                {
                    this.Text = "Add New Data Point";
                }

                // Focus on first input
                txtIOA.Focus();
                txtIOA.SelectAll();

                // Additional initialization if needed
                UpdateTypeIdBasedOnDataType();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading form: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}