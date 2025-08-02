// File: Forms/MainForm.cs (Phiên bản cuối cùng - Quay về kiến trúc đơn)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using IEC60870.Enum;
using IEC60870.IE;
using IEC60870.IE.Base;
using IEC60870.Object;
using IEC60870ServerWinForm.Models;
using IEC60870ServerWinForm.Services;

namespace IEC60870ServerWinForm.Forms
{
    public partial class MainForm : Form
    {
        // --- Services ---
        private readonly IEC60870ServerService _serverService;
        private readonly ConfigManager _configManager;

        // --- Data Storage ---
        private ServerConfig _serverConfig;
        private List<DataPoint> _dataPoints;
        private BindingSource _dataPointsBindingSource;

        public MainForm()
        {
            InitializeComponent();

            _serverService = new IEC60870ServerService();
            _configManager = new ConfigManager();

            // Khôi phục lại các sự kiện như ban đầu
            _serverService.OnLogMessage += LogMessage;
            _serverService.OnAsduReceived += HandleReceivedAsdu;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _serverConfig = _configManager.LoadServerConfig();
            _dataPoints = _configManager.LoadDataPoints();

            _dataPointsBindingSource = new BindingSource
            {
                DataSource = new SortableBindingList<DataPoint>(_dataPoints)
            };
            dgvDataPoints.DataSource = _dataPointsBindingSource;

            // Cấu hình các cột
            dgvDataPoints.Columns["IOA"].Width = 60;
            dgvDataPoints.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvDataPoints.Columns["Type"].Width = 70;
            dgvDataPoints.Columns["Value"].Width = 80;
            if (dgvDataPoints.Columns["Description"] != null)
            {
                dgvDataPoints.Columns["Description"].Visible = false;
            }

            UpdateServerStatusUI();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Đảm bảo server được tắt khi đóng ứng dụng
            if (_serverService.IsRunning)
            {
                _serverService.Stop();
            }
            _configManager.SaveDataPoints(_dataPoints);
        }

        #region Button Event Handlers

        private void btnStart_Click(object sender, EventArgs e)
        {
            _serverService.Start(_serverConfig);
            UpdateServerStatusUI();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = false;
            lblServerStatus.Text = "Status: Stopping...";
            lblServerStatus.ForeColor = System.Drawing.Color.OrangeRed;

            Application.DoEvents();

            _serverService.Stop();

            UpdateServerStatusUI();
        }

        private void btnAddPoint_Click(object sender, EventArgs e)
        {
            using (var form = new DataPointForm())
            {
                if (form.ShowDialog() != DialogResult.OK) return;

                if (_dataPoints.Any(dp => dp.IOA == form.DataPoint.IOA))
                {
                    MessageBox.Show($"IOA '{form.DataPoint.IOA}' already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                _dataPointsBindingSource.Add(form.DataPoint);
                _configManager.SaveDataPoints(_dataPoints);
                LogMessage($"Added data point: IOA {form.DataPoint.IOA}, Name {form.DataPoint.Name}");
            }
        }

        private void btnEditPoint_Click(object sender, EventArgs e)
        {
            if (dgvDataPoints.CurrentRow?.DataBoundItem is DataPoint selectedPoint)
            {
                using (var form = new DataPointForm(selectedPoint))
                {
                    if (form.ShowDialog() != DialogResult.OK) return;

                    _dataPointsBindingSource.ResetBindings(false);
                    _configManager.SaveDataPoints(_dataPoints);
                    LogMessage($"Edited data point: IOA {selectedPoint.IOA}");
                }
            }
            else
            {
                MessageBox.Show("Please select a data point to edit.", "Information");
            }
        }

        private void btnDeletePoint_Click(object sender, EventArgs e)
        {
            if (dgvDataPoints.CurrentRow?.DataBoundItem is DataPoint selectedPoint)
            {
                if (MessageBox.Show($"Are you sure you want to delete '{selectedPoint.Name}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    _dataPointsBindingSource.Remove(selectedPoint);
                    _configManager.SaveDataPoints(_dataPoints);
                    LogMessage($"Deleted data point: IOA {selectedPoint.IOA}");
                }
            }
            else
            {
                MessageBox.Show("Please select a data point to delete.", "Information");
            }
        }

        private void btnSendSelected_Click(object sender, EventArgs e)
        {
            if (!_serverService.IsRunning)
            {
                MessageBox.Show("Server is not running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selectedPoints = dgvDataPoints.SelectedRows
                .Cast<DataGridViewRow>()
                .Select(row => row.DataBoundItem as DataPoint)
                .Where(dp => dp != null)
                .ToList();

            if (selectedPoints.Count == 0)
            {
                MessageBox.Show("Please select at least one data point to send.", "Information");
                return;
            }

            foreach (var point in selectedPoints)
            {
                // Khôi phục lại logic tạo ASDU trực tiếp
                ASdu asdu = CreateAsduFromDataPoint(point);
                if (asdu != null)
                {
                    _serverService.BroadcastAsdu(asdu);
                    LogMessage($"Sent data for IOA {point.IOA} with value {point.Value}");
                }
            }
        }

        private void btnConfigure_Click(object sender, EventArgs e)
        {
            if (_serverService.IsRunning)
            {
                MessageBox.Show("Please stop the server before changing the configuration.", "Server Running", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var form = new ServerConfigForm(_serverConfig))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    _serverConfig = form.ServerConfiguration;
                    _configManager.SaveServerConfig(_serverConfig);
                    LogMessage("Server configuration updated. Settings will be applied on next start.");
                }
            }
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Khôi phục lại hàm này: Chuyển đổi một DataPoint thành một bản tin ASDU.
        /// </summary>
        private ASdu CreateAsduFromDataPoint(DataPoint point)
        {
            InformationObject infoObject;
            var commonAddress = _serverConfig.CommonAddress;

            try
            {
                switch (point.Type)
                {
                    case DataType.Bool:
                        var sp = new IeSinglePointWithQuality((bool)point.Value, false, false, false, false);
                        infoObject = new InformationObject(point.IOA, new[] { new InformationElement[] { sp } });
                        return new ASdu(TypeId.M_SP_NA_1, false, CauseOfTransmission.SPONTANEOUS, false, false, 0, commonAddress, new[] { infoObject });

                    case DataType.Float:
                        var sf = new IeShortFloat(Convert.ToSingle(point.Value));
                        var quality = new IeQuality(false, false, false, false, false);
                        infoObject = new InformationObject(point.IOA, new[] { new InformationElement[] { sf, quality } });
                        return new ASdu(TypeId.M_ME_NC_1, false, CauseOfTransmission.SPONTANEOUS, false, false, 0, commonAddress, new[] { infoObject });

                    case DataType.Int:
                        var sv = new IeScaledValue(Convert.ToInt16(point.Value));
                        var qds = new IeQuality(false, false, false, false, false);
                        infoObject = new InformationObject(point.IOA, new[] { new InformationElement[] { sv, qds } });
                        return new ASdu(TypeId.M_ME_NB_1, false, CauseOfTransmission.SPONTANEOUS, false, false, 0, commonAddress, new[] { infoObject });

                    case DataType.Counter:
                        var bcr = new IeBinaryCounterReading(Convert.ToInt32(point.Value), 0, false, false, false);
                        infoObject = new InformationObject(point.IOA, new[] { new InformationElement[] { bcr } });
                        return new ASdu(TypeId.M_IT_NA_1, false, CauseOfTransmission.SPONTANEOUS, false, false, 0, commonAddress, new[] { infoObject });

                    default:
                        LogMessage($"Error: Data type '{point.Type}' is not supported for sending.");
                        return null;
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error creating ASDU for IOA {point.IOA}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Khôi phục lại hàm này: Xử lý các lệnh nhận được từ client.
        /// </summary>
        private void HandleReceivedAsdu(ASdu asdu)
        {
            if (asdu.GetTypeIdentification() == TypeId.C_IC_NA_1)
            {
                LogMessage("General Interrogation command received. Sending all data points...");
                foreach (DataPoint dp in _dataPoints)
                {
                    var originalAsdu = CreateAsduFromDataPoint(dp);
                    if (originalAsdu == null) continue;

                    // Tạo ASDU mới với COT đã thay đổi
                    var responseAsdu = new ASdu(
                        originalAsdu.GetTypeIdentification(),
                        originalAsdu.IsSequenceOfElements,
                        CauseOfTransmission.INTERROGATED_BY_STATION,
                        originalAsdu.IsTestFrame(),
                        originalAsdu.IsNegativeConfirm(),
                        originalAsdu.GetOriginatorAddress(),
                        originalAsdu.GetCommonAddress(),
                        originalAsdu.GetInformationObjects()
                    );
                    _serverService.BroadcastAsdu(responseAsdu);
                }
                LogMessage("Finished sending all data points for interrogation.");
            }
            // Thêm logic xử lý các lệnh khác ở đây nếu cần
        }

        private void UpdateServerStatusUI()
        {
            bool isRunning = _serverService.IsRunning;
            lblServerStatus.Text = isRunning ? "Status: Running" : "Status: Stopped";
            lblServerStatus.ForeColor = isRunning ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            btnStart.Enabled = !isRunning;
            btnStop.Enabled = isRunning;
        }

        private void LogMessage(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(LogMessage), message);
                return;
            }
            if (txtLogs.Text.Length > 30000)
            {
                txtLogs.Text = txtLogs.Text.Substring(10000);
            }
            txtLogs.AppendText(message + Environment.NewLine);
        }

        #endregion
    }

    public class SortableBindingList<T> : BindingList<T>
    {
        public SortableBindingList(IList<T> list) : base(list) { }
    }
}