// File: Forms/ServerConfigForm.cs
using IEC60870ServerWinForm.Models;
using System;
using System.Net;
using System.Windows.Forms;

namespace IEC60870ServerWinForm.Forms
{
    public partial class ServerConfigForm : Form
    {
        // Thuộc tính để MainForm có thể lấy cấu hình mới
        public ServerConfig ServerConfiguration { get; private set; }

        public ServerConfigForm(ServerConfig currentConfig)
        {
            InitializeComponent();
            this.ServerConfiguration = currentConfig;
        }

        private void ServerConfigForm_Load(object sender, EventArgs e)
        {
            // Điền dữ liệu từ cấu hình hiện tại vào các control
            txtIPAddress.Text = ServerConfiguration.IPAddress;
            txtPort.Text = ServerConfiguration.Port.ToString();
            txtCommonAddress.Text = ServerConfiguration.CommonAddress.ToString();
            txtCotLength.Text = ServerConfiguration.CotFieldLength.ToString();
            txtCaLength.Text = ServerConfiguration.CaFieldLength.ToString();
            txtIoaLength.Text = ServerConfiguration.IoaFieldLength.ToString();
            txtTimeoutT0.Text = ServerConfiguration.TimeoutT0.ToString();
            txtTimeoutT1.Text = ServerConfiguration.TimeoutT1.ToString();
            txtTimeoutT2.Text = ServerConfiguration.TimeoutT2.ToString();
            txtTimeoutT3.Text = ServerConfiguration.TimeoutT3.ToString();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Xác thực dữ liệu và lưu lại
            if (!IPAddress.TryParse(txtIPAddress.Text, out _))
            {
                MessageBox.Show("Invalid IP Address format.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtPort.Text, out int port) || port <= 0 || port > 65535)
            {
                MessageBox.Show("Port must be a number between 1 and 65535.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // (Thêm các bước xác thực khác cho các ô còn lại nếu cần)
            try
            {
                ServerConfiguration.IPAddress = txtIPAddress.Text;
                ServerConfiguration.Port = int.Parse(txtPort.Text);
                ServerConfiguration.CommonAddress = int.Parse(txtCommonAddress.Text);
                ServerConfiguration.CotFieldLength = int.Parse(txtCotLength.Text);
                ServerConfiguration.CaFieldLength = int.Parse(txtCaLength.Text);
                ServerConfiguration.IoaFieldLength = int.Parse(txtIoaLength.Text);
                ServerConfiguration.TimeoutT0 = int.Parse(txtTimeoutT0.Text);
                ServerConfiguration.TimeoutT1 = int.Parse(txtTimeoutT1.Text);
                ServerConfiguration.TimeoutT2 = int.Parse(txtTimeoutT2.Text);
                ServerConfiguration.TimeoutT3 = int.Parse(txtTimeoutT3.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("All fields must be valid numbers.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}