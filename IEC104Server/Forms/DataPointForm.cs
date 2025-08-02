// File: Forms/DataPointForm.cs
using IEC60870ServerWinForm.Models;
using System;
using System.Windows.Forms;

namespace IEC60870ServerWinForm.Forms
{
    public partial class DataPointForm : Form
    {
        // Thuộc tính để MainForm có thể lấy được DataPoint sau khi form này đóng
        public DataPoint DataPoint { get; private set; }

        // Constructor: nhận một DataPoint có sẵn để vào chế độ "Edit"
        // Nếu không có DataPoint nào được truyền vào, form sẽ ở chế độ "Add"
        public DataPointForm(DataPoint dataPoint = null)
        {
            InitializeComponent();
            this.DataPoint = dataPoint ?? new DataPoint();
        }

        private void DataPointForm_Load(object sender, EventArgs e)
        {
            // Nạp danh sách các kiểu dữ liệu từ Enum vào ComboBox
            cmbDataType.DataSource = Enum.GetValues(typeof(DataType));

            // Nếu là chế độ "Edit", điền thông tin có sẵn vào các ô
            if (DataPoint.IOA > 0)
            {
                this.Text = "Edit Data Point";
                txtIOA.Text = DataPoint.IOA.ToString();
                txtName.Text = DataPoint.Name;
                cmbDataType.SelectedItem = DataPoint.Type;
                txtValue.Text = DataPoint.Value?.ToString() ?? "";
            }
            else
            {
                this.Text = "Add New Data Point";
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // --- Kiểm tra và xác thực dữ liệu đầu vào (Validation) ---
            if (!int.TryParse(txtIOA.Text, out int ioa) || ioa <= 0)
            {
                MessageBox.Show("IOA must be a positive number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Name cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selectedType = (DataType)cmbDataType.SelectedItem;
            object convertedValue = null;

            // Chuyển đổi giá trị người dùng nhập sang đúng kiểu dữ liệu
            try
            {
                switch (selectedType)
                {
                    case DataType.Bool:
                        convertedValue = bool.Parse(txtValue.Text);
                        break;
                    case DataType.Float:
                        convertedValue = float.Parse(txtValue.Text);
                        break;
                    case DataType.Int:
                    case DataType.Counter:
                        convertedValue = int.Parse(txtValue.Text);
                        break;
                }
            }
            catch (Exception)
            {
                MessageBox.Show($"Invalid value for type '{selectedType}'. Please enter a correct value (e.g., true/false for Bool, 25.5 for Float, 100 for Int).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // --- Cập nhật thông tin vào đối tượng DataPoint ---
            DataPoint.IOA = ioa;
            DataPoint.Name = txtName.Text;
            DataPoint.Type = selectedType;
            DataPoint.Value = convertedValue;

            // Đóng form và trả về kết quả OK
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}