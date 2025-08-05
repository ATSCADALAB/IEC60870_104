// File: Forms/DataPointForm.cs
using IEC60870ServerWinForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IEC60870ServerWinForm.Forms
{
    public partial class DataPointForm : Form
    {
        public DataPoint DataPoint { get; private set; }

        // Property để lấy/set tên Tag từ SmartTagComboBox
        public string DataTagName
        {
            get => this.TagName.TagName?.Trim() ?? "";
            set => this.TagName.TagName = value ?? "";
        }

        public DataPointForm(DataPoint existingPoint = null)
        {
            InitializeComponent();

            // Khởi tạo DataPoint
            DataPoint = existingPoint ?? new DataPoint();

            // Populate DataType ComboBox
            cmbDataType.DataSource = Enum.GetValues(typeof(DataType));
            cmbDataType.SelectedItem = DataType.Float; // Default selection
        }

        private void DataPointForm_Load(object sender, EventArgs e)
        {
            if (DataPoint != null)
            {
                // Load existing data point values
                txtIOA.Text = DataPoint.IOA.ToString();
                txtName.Text = DataPoint.Name;
                cmbDataType.SelectedItem = DataPoint.Type;
                DataTagName = DataPoint.DataTagName; // Load tên Tag vào SmartTagComboBox

                // Set title based on mode
                this.Text = DataPoint.IOA > 0 ? "Edit Data Point" : "Add Data Point";
            }
            else
            {
                // New data point defaults
                txtIOA.Text = "1";
                txtName.Text = "";
                cmbDataType.SelectedItem = DataType.Float;
                DataTagName = "";
                this.Text = "Add Data Point";
            }

            // Set focus to IOA textbox
            txtIOA.Focus();
            txtIOA.SelectAll();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Validation
            if (!ValidateInput())
                return;

            try
            {
                // Cập nhật DataPoint với thông tin mới
                DataPoint.IOA = int.Parse(txtIOA.Text.Trim());
                DataPoint.Name = txtName.Text.Trim();
                DataPoint.Type = (DataType)cmbDataType.SelectedItem;
                DataPoint.DataTagName = DataTagName; // Lưu tên Tag từ SmartTagComboBox
                DataPoint.Description = $"IOA {DataPoint.IOA} - {DataPoint.Name}"; // Auto generate description

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data point: {ex.Message}",
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            // Validate IOA
            if (!int.TryParse(txtIOA.Text?.Trim(), out int ioa) || ioa <= 0)
            {
                MessageBox.Show("IOA must be a positive integer.",
                               "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtIOA.Focus();
                return false;
            }

            // Validate Name
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Name cannot be empty.",
                               "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            // Validate Tag Name
            if (string.IsNullOrWhiteSpace(DataTagName))
            {
                MessageBox.Show("Please select a Tag Name from the dropdown.",
                               "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TagName.Focus();
                return false;
            }

            // Validate Data Type selection
            if (cmbDataType.SelectedItem == null)
            {
                MessageBox.Show("Please select a Data Type.",
                               "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbDataType.Focus();
                return false;
            }

            // Check for reasonable IOA range (IEC 60870-5-104 standard)
            if (ioa > 16777215) // 2^24 - 1 (3 bytes max for IOA)
            {
                var result = MessageBox.Show(
                    $"IOA {ioa} is very large. IEC 104 standard typically uses IOA up to 16777215.\n\nDo you want to continue?",
                    "IOA Range Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    txtIOA.Focus();
                    return false;
                }
            }

            return true;
        }

        // Helper method to set data type based on tag name pattern (optional)
        private void TagName_TagChanged(object sender, EventArgs e)
        {
            string tagName = DataTagName.ToLower();

            // Auto-suggest data type based on tag name patterns
            if (tagName.Contains("bool") || tagName.Contains("bit") || tagName.Contains("status"))
            {
                cmbDataType.SelectedItem = DataType.Bool;
            }
            else if (tagName.Contains("int") || tagName.Contains("count"))
            {
                cmbDataType.SelectedItem = DataType.Int;
            }
            else if (tagName.Contains("float") || tagName.Contains("analog") || tagName.Contains("value"))
            {
                cmbDataType.SelectedItem = DataType.Float;
            }
            else if (tagName.Contains("counter"))
            {
                cmbDataType.SelectedItem = DataType.Counter;
            }
            // Keep current selection if no pattern matches
        }

        // Optional: Add validation for duplicate IOA
        public static bool IsIOAUnique(int ioa, List<DataPoint> existingPoints, DataPoint currentPoint = null)
        {
            return !existingPoints.Any(dp => dp.IOA == ioa && dp != currentPoint);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Override ProcessCmdKey to handle Enter and Escape
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Enter:
                    if (btnOK.Enabled)
                    {
                        btnOK.PerformClick();
                        return true;
                    }
                    break;

                case Keys.Escape:
                    btnCancel.PerformClick();
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}