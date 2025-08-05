// File: Forms/DataPointForm.cs - Simple and Effective
using IEC60870ServerWinForm.Models;
using System;
using System.Windows.Forms;

namespace IEC60870ServerWinForm.Forms
{
    public partial class DataPointForm : Form
    {
        public DataPoint DataPoint { get; private set; }

        // Simple property để get/set tag name từ SmartTagComboBox
        public string DataTagName
        {
            get => TagName.TagName?.Trim() ?? "";
            set => TagName.TagName = value ?? "";
        }

        public DataPointForm(DataPoint existingPoint = null)
        {
            InitializeComponent();

            DataPoint = existingPoint ?? new DataPoint();

            // Setup DataType combo
            cmbDataType.DataSource = Enum.GetValues(typeof(DataType));
            cmbDataType.SelectedItem = DataType.Float;
        }

        private void DataPointForm_Load(object sender, EventArgs e)
        {
            if (DataPoint.IOA > 0) // Edit mode
            {
                txtIOA.Text = DataPoint.IOA.ToString();
                txtName.Text = DataPoint.Name;
                cmbDataType.SelectedItem = DataPoint.Type;
                DataTagName = DataPoint.DataTagName;
                Text = "Edit Data Point";
            }
            else // Add mode
            {
                txtIOA.Text = "1";
                txtName.Text = "";
                cmbDataType.SelectedItem = DataType.Float;
                DataTagName = "";
                Text = "Add Data Point";
            }

            txtIOA.Focus();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Simple validation
            if (!int.TryParse(txtIOA.Text, out int ioa) || ioa <= 0)
            {
                MessageBox.Show("IOA must be a positive number!");
                txtIOA.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Name cannot be empty!");
                txtName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(DataTagName))
            {
                MessageBox.Show("Please select a Tag!");
                TagName.Focus();
                return;
            }

            // Save data
            DataPoint.IOA = ioa;
            DataPoint.Name = txtName.Text.Trim();
            DataPoint.Type = (DataType)cmbDataType.SelectedItem;
            DataPoint.DataTagName = DataTagName.Trim();
            DataPoint.Description = $"IOA {DataPoint.IOA} - {DataPoint.Name}";

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // Handle Enter/Escape keys
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter && btnOK.Enabled)
            {
                btnOK.PerformClick();
                return true;
            }
            if (keyData == Keys.Escape)
            {
                btnCancel.PerformClick();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}