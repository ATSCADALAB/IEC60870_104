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
using ATSCADA;
using ATSCADA.ToolExtensions.TagCollection;

namespace IEC60870ServerWinForm.Forms
{
    public partial class MainForm : Form
    {
        // --- Services ---
        private readonly IEC60870ServerService _serverService;
        private readonly ConfigManager _configManager;
        private readonly DriverManager _driverManager;

        // --- Data Storage ---
        private ServerConfig _serverConfig;
        private List<DataPoint> _dataPoints;
        private BindingSource _dataPointsBindingSource;

        // --- Timer để quét dữ liệu ---
        private Timer _tagScanTimer;
        private Timer _dataSendTimer;
        private bool _isAutoSendEnabled = false;

        public MainForm()
        {
            InitializeComponent();

            _serverService = new IEC60870ServerService();
            _configManager = new ConfigManager();
            _driverManager = new DriverManager();

            // Setup driver manager events
            _driverManager.LogMessage += LogMessage;
            _driverManager.DriverReady += OnDriverReady;

            // Khôi phục lại các sự kiện như ban đầu
            _serverService.OnLogMessage += LogMessage;
            _serverService.OnAsduReceived += HandleReceivedAsdu;

            // Khởi tạo timers
            InitializeTimers();
        }

        // Method để set driver từ bên ngoài
        public void SetDriver(iDriver driverInstance)
        {
            LogMessage("Setting driver instance...");
            _driverManager.Initialize(driverInstance);

            // Nếu driver đã sẵn sàng (đã complete construction), trigger ngay
            if (driverInstance != null)
            {
                LogMessage("Driver instance set. Checking construction status...");
                // Force check driver ready trong trường hợp construction đã completed trước khi set
                CheckDriverStatus();
            }
        }

        private void CheckDriverStatus()
        {
            // Kiểm tra nếu driver đã sẵn sàng để sử dụng
            if (_driverManager.Driver != null)
            {
                LogMessage("Driver instance found. Testing basic functionality...");

                // Test driver bằng cách thử lấy một tag bất kỳ (nếu có data points)
                if (_dataPoints?.Count > 0)
                {
                    var firstPointWithTag = _dataPoints.FirstOrDefault(dp => !string.IsNullOrEmpty(dp.DataTagName));
                    if (firstPointWithTag != null)
                    {
                        try
                        {
                            var testTag = _driverManager.GetTag(firstPointWithTag.DataTagName);
                            if (testTag != null)
                            {
                                LogMessage("Driver is functional. Tag reading test successful.");
                                // Manually trigger driver ready if not already triggered
                                if (!_driverManager.IsInitialized)
                                {
                                    LogMessage("Manually triggering driver ready state...");
                                    OnDriverReady();
                                }
                            }
                            else
                            {
                                LogMessage($"Driver test: Tag '{firstPointWithTag.DataTagName}' not found. Driver may not be fully initialized.");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogMessage($"Driver test failed: {ex.Message}");
                        }
                    }
                }
                else
                {
                    LogMessage("No data points configured for driver testing.");
                }
            }
            else
            {
                LogMessage("Driver instance is null.");
            }
        }

        private void OnDriverReady()
        {
            LogMessage("Driver is ready. Starting tag scanning...");
            // Có thể trigger validation tags hoặc các hành động khác
            ValidateDataPointTags();

            // Start tag scanning nếu chưa start
            if (!_tagScanTimer.Enabled && _driverManager.IsInitialized)
            {
                StartTagScanning();
            }
        }

        private void InitializeDriver()
        {
            // Driver manager đã được khởi tạo trong constructor
            // Driver instance sẽ được set từ bên ngoài thông qua SetDriver method
            LogMessage("Driver manager initialized. Waiting for driver instance...");
        }

        private void OnDriverConstructionCompleted()
        {
            LogMessage("Driver construction completed. Ready to read tags.");
        }

        private void InitializeTimers()
        {
            // Timer quét dữ liệu từ Tag (mỗi 1 giây)
            _tagScanTimer = new Timer();
            _tagScanTimer.Interval = 1000; // 1 second
            _tagScanTimer.Tick += TagScanTimer_Tick;

            // Timer tự động gửi dữ liệu (mỗi 5 giây)
            _dataSendTimer = new Timer();
            _dataSendTimer.Interval = 5000; // 5 seconds  
            _dataSendTimer.Tick += DataSendTimer_Tick;
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

            // Thêm cột hiển thị DataTagName
            if (dgvDataPoints.Columns["DataTagName"] != null)
            {
                dgvDataPoints.Columns["DataTagName"].Width = 120;
                dgvDataPoints.Columns["DataTagName"].HeaderText = "Tag Name";
            }

            if (dgvDataPoints.Columns["Description"] != null)
            {
                dgvDataPoints.Columns["Description"].Visible = false;
            }

            UpdateServerStatusUI();

            // Bắt đầu timer quét dữ liệu
            StartTagScanning();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Dừng timers
            StopAllTimers();

            // Đảm bảo server được tắt khi đóng ứng dụng
            if (_serverService.IsRunning)
            {
                _serverService.Stop();
            }
            _configManager.SaveDataPoints(_dataPoints);

            // Dispose driver manager
            _driverManager?.Dispose();
        }

        #region Timer Methods

        private void StartTagScanning()
        {
            if (_driverManager.IsInitialized)
            {
                _tagScanTimer.Start();
                LogMessage("Started tag data scanning.");
            }
            else
            {
                LogMessage("Driver not ready. Tag scanning will start when driver is initialized.");
            }
        }

        private void StopTagScanning()
        {
            _tagScanTimer.Stop();
            LogMessage("Stopped tag data scanning.");
        }

        private void StartAutoSend()
        {
            if (_serverService.IsRunning)
            {
                _isAutoSendEnabled = true;
                _dataSendTimer.Start();
                LogMessage("Started automatic data sending.");
            }
            else
            {
                MessageBox.Show("Please start the server first.", "Server Not Running",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void StopAutoSend()
        {
            _isAutoSendEnabled = false;
            _dataSendTimer.Stop();
            LogMessage("Stopped automatic data sending.");
        }

        private void StopAllTimers()
        {
            _tagScanTimer?.Stop();
            _dataSendTimer?.Stop();
            _tagScanTimer?.Dispose();
            _dataSendTimer?.Dispose();
        }

        private void TagScanTimer_Tick(object sender, EventArgs e)
        {
            UpdateDataPointValuesFromTags();
        }

        private void DataSendTimer_Tick(object sender, EventArgs e)
        {
            if (_isAutoSendEnabled && _serverService.IsRunning)
            {
                SendAllDataPoints();
            }
        }

        #endregion

        #region Data Methods

        private void UpdateDataPointValuesFromTags()
        {
            if (!_driverManager.IsInitialized)
            {
                // Thử kiểm tra lại driver status
                CheckDriverStatus();

                // Nếu vẫn chưa ready thì return
                if (!_driverManager.IsInitialized)
                {
                    LogMessage("Driver not initialized. Skipping tag update.");
                    return;
                }
            }

            bool hasChanges = false;
            foreach (var dataPoint in _dataPoints)
            {
                if (!string.IsNullOrEmpty(dataPoint.DataTagName))
                {
                    try
                    {
                        // Lấy giá trị string từ Tag sử dụng DriverManager
                        var tagValue = _driverManager.GetTagValue(dataPoint.DataTagName);
                        bool isTagGood = _driverManager.IsTagGood(dataPoint.DataTagName);

                        // Chỉ cập nhật nếu giá trị thay đổi
                        if (!string.Equals(dataPoint.Value, tagValue) || dataPoint.IsValid != isTagGood)
                        {
                            dataPoint.Value = tagValue ?? "";
                            dataPoint.LastUpdated = DateTime.Now;
                            dataPoint.IsValid = isTagGood && !string.IsNullOrEmpty(tagValue);
                            hasChanges = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"Error reading tag '{dataPoint.DataTagName}': {ex.Message}");
                        if (!string.IsNullOrEmpty(dataPoint.Value) || dataPoint.IsValid != false)
                        {
                            dataPoint.Value = "";
                            dataPoint.IsValid = false;
                            hasChanges = true;
                        }
                    }
                }
            }

            // Refresh UI nếu có thay đổi
            if (hasChanges)
            {
                _dataPointsBindingSource?.ResetBindings(false);
            }
        }

        private void ValidateDataPointTags()
        {
            if (!_driverManager.IsInitialized || _dataPoints == null) return;

            var tagNames = _dataPoints.Where(dp => !string.IsNullOrEmpty(dp.DataTagName))
                                    .Select(dp => dp.DataTagName)
                                    .Distinct()
                                    .ToList();

            if (tagNames.Count == 0) return;

            var validationResults = _driverManager.ValidateTags(tagNames);

            foreach (var result in validationResults)
            {
                if (!result.Value)
                {
                    LogMessage($"Warning: Tag '{result.Key}' not found in driver.");
                }
            }

            LogMessage($"Validated {tagNames.Count} unique tags. {validationResults.Count(r => r.Value)} found.");
        }

        private void SendAllDataPoints()
        {
            var validPoints = _dataPoints.Where(dp => !string.IsNullOrEmpty(dp.Value) &&
                                                     !string.IsNullOrEmpty(dp.DataTagName) &&
                                                     dp.IsValid).ToList();

            if (validPoints.Count == 0) return;

            foreach (var point in validPoints)
            {
                ASdu asdu = CreateAsduFromDataPoint(point);
                if (asdu != null)
                {
                    _serverService.BroadcastAsdu(asdu);
                }
            }

            LogMessage($"Auto-sent {validPoints.Count} valid data points.");
        }

        #endregion

        private void configureServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_serverService.IsRunning)
            {
                MessageBox.Show("Please stop the server before changing the configuration.",
                               "Server Running", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        #region Button Event Handlers

        private void btnStart_Click(object sender, EventArgs e)
        {
            _serverService.Start(_serverConfig);
            UpdateServerStatusUI();

            // Bắt đầu auto send nếu server khởi động thành công
            if (_serverService.IsRunning)
            {
                StartAutoSend();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = false;
            lblServerStatus.Text = "Status: Stopping...";
            lblServerStatus.ForeColor = System.Drawing.Color.OrangeRed;

            Application.DoEvents();

            // Dừng auto send trước
            StopAutoSend();

            _serverService.Stop();

            UpdateServerStatusUI();
        }

        private void btnAddPoint_Click(object sender, EventArgs e)
        {
            using (var form = new DataPointForm())
            {
                if (form.ShowDialog() != DialogResult.OK) return;

                // Kiểm tra IOA trùng lặp
                if (_dataPoints.Any(dp => dp.IOA == form.DataPoint.IOA))
                {
                    MessageBox.Show($"IOA '{form.DataPoint.IOA}' already exists. Please use a different IOA.",
                                   "Duplicate IOA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kiểm tra Tag name trùng lặp (warning only)
                if (_dataPoints.Any(dp => dp.DataTagName == form.DataPoint.DataTagName && !string.IsNullOrEmpty(form.DataPoint.DataTagName)))
                {
                    var result = MessageBox.Show($"Tag '{form.DataPoint.DataTagName}' is already used by another data point.\n\nDo you want to continue?",
                                               "Duplicate Tag Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.No)
                        return;
                }

                _dataPointsBindingSource.Add(form.DataPoint);
                _configManager.SaveDataPoints(_dataPoints);
                LogMessage($"Added data point: IOA {form.DataPoint.IOA}, Name '{form.DataPoint.Name}', Tag '{form.DataPoint.DataTagName}'");

                // Validate new tag immediately
                if (!string.IsNullOrEmpty(form.DataPoint.DataTagName))
                {
                    bool tagExists = _driverManager.GetTag(form.DataPoint.DataTagName) != null;
                    if (!tagExists)
                    {
                        LogMessage($"Warning: Tag '{form.DataPoint.DataTagName}' not found in driver. Please check tag name.");
                    }
                }
            }
        }

        private void btnEditPoint_Click(object sender, EventArgs e)
        {
            if (dgvDataPoints.CurrentRow?.DataBoundItem is DataPoint selectedPoint)
            {
                using (var form = new DataPointForm(selectedPoint))
                {
                    if (form.ShowDialog() != DialogResult.OK) return;

                    // Kiểm tra IOA trùng lặp (ngoại trừ chính nó)
                    if (_dataPoints.Any(dp => dp.IOA == form.DataPoint.IOA && dp != selectedPoint))
                    {
                        MessageBox.Show($"IOA '{form.DataPoint.IOA}' already exists. Please use a different IOA.",
                                       "Duplicate IOA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    _dataPointsBindingSource.ResetBindings(false);
                    _configManager.SaveDataPoints(_dataPoints);
                    LogMessage($"Edited data point: IOA {selectedPoint.IOA}, Name '{selectedPoint.Name}', Tag '{selectedPoint.DataTagName}'");

                    // Validate updated tag
                    if (!string.IsNullOrEmpty(selectedPoint.DataTagName))
                    {
                        bool tagExists = _driverManager.GetTag(selectedPoint.DataTagName) != null;
                        if (!tagExists)
                        {
                            LogMessage($"Warning: Tag '{selectedPoint.DataTagName}' not found in driver. Please check tag name.");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a data point to edit.", "No Selection",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            // Cập nhật giá trị từ Tag trước khi gửi
            UpdateDataPointValuesFromTags();

            foreach (var point in selectedPoints)
            {
                if (!string.IsNullOrEmpty(point.Value) && !string.IsNullOrEmpty(point.DataTagName))
                {
                    ASdu asdu = CreateAsduFromDataPoint(point);
                    if (asdu != null)
                    {
                        _serverService.BroadcastAsdu(asdu);
                        LogMessage($"Sent data for IOA {point.IOA} from Tag '{point.DataTagName}' with value '{point.Value}' (converted to {point.ConvertedValue})");
                    }
                }
                else
                {
                    LogMessage($"Warning: No value available for Tag '{point.DataTagName}' at IOA {point.IOA}");
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
                // Sử dụng ConvertedValue để lấy giá trị đã convert theo DataType
                var convertedValue = point.ConvertedValue;
                if (convertedValue == null)
                {
                    LogMessage($"Error: Cannot convert value '{point.Value}' to type '{point.Type}' for IOA {point.IOA}");
                    return null;
                }

                switch (point.Type)
                {
                    case DataType.Bool:
                        var sp = new IeSinglePointWithQuality((bool)convertedValue, false, false, false, false);
                        infoObject = new InformationObject(point.IOA, new[] { new InformationElement[] { sp } });
                        return new ASdu(TypeId.M_SP_NA_1, false, CauseOfTransmission.SPONTANEOUS, false, false, 0, commonAddress, new[] { infoObject });

                    case DataType.Float:
                        var sf = new IeShortFloat((float)convertedValue);
                        var quality = new IeQuality(false, false, false, false, false);
                        infoObject = new InformationObject(point.IOA, new[] { new InformationElement[] { sf, quality } });
                        return new ASdu(TypeId.M_ME_NC_1, false, CauseOfTransmission.SPONTANEOUS, false, false, 0, commonAddress, new[] { infoObject });

                    case DataType.Int:
                        var sv = new IeScaledValue((short)(int)convertedValue);
                        var qds = new IeQuality(false, false, false, false, false);
                        infoObject = new InformationObject(point.IOA, new[] { new InformationElement[] { sv, qds } });
                        return new ASdu(TypeId.M_ME_NB_1, false, CauseOfTransmission.SPONTANEOUS, false, false, 0, commonAddress, new[] { infoObject });

                    case DataType.Counter:
                        var bcr = new IeBinaryCounterReading((int)convertedValue, 0, false, false, false);
                        infoObject = new InformationObject(point.IOA, new[] { new InformationElement[] { bcr } });
                        return new ASdu(TypeId.M_IT_NA_1, false, CauseOfTransmission.SPONTANEOUS, false, false, 0, commonAddress, new[] { infoObject });

                    case DataType.Double:
                        var nv = new IeNormalizedValue((int)convertedValue);
                        var qualityNv = new IeQuality(false, false, false, false, false);
                        infoObject = new InformationObject(point.IOA, new[] { new InformationElement[] { nv, qualityNv } });
                        return new ASdu(TypeId.M_ME_NA_1, false, CauseOfTransmission.SPONTANEOUS, false, false, 0, commonAddress, new[] { infoObject });

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

                // Cập nhật giá trị từ Tag trước khi gửi
                UpdateDataPointValuesFromTags();

                foreach (DataPoint dp in _dataPoints)
                {
                    if (!string.IsNullOrEmpty(dp.Value) && !string.IsNullOrEmpty(dp.DataTagName))
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
            string logEntry = $"[{DateTime.Now:HH:mm:ss}] {message}";
            txtLogs.AppendText(logEntry + Environment.NewLine);

            // Debug output
            System.Diagnostics.Debug.WriteLine(logEntry);
        }

        // Debug method để kiểm tra driver status
        public void DebugDriverStatus()
        {
            LogMessage("=== DRIVER DEBUG INFO ===");
            LogMessage($"DriverManager IsInitialized: {_driverManager?.IsInitialized}");
            LogMessage($"Driver Instance: {_driverManager?.Driver != null}");

            if (_driverManager?.Driver != null)
            {
                LogMessage($"Driver Type: {_driverManager.Driver.GetType().Name}");

                // Test với data points
                if (_dataPoints?.Count > 0)
                {
                    var taggedPoints = _dataPoints.Where(dp => !string.IsNullOrEmpty(dp.DataTagName)).ToList();
                    LogMessage($"Data points with tags: {taggedPoints.Count}");

                    foreach (var dp in taggedPoints.Take(3)) // Test first 3 tags
                    {
                        try
                        {
                            var tag = _driverManager.GetTag(dp.DataTagName);
                            LogMessage($"Tag '{dp.DataTagName}': {(tag != null ? "Found" : "Not Found")}");
                            if (tag != null)
                            {
                                LogMessage($"  - Value: {tag.Value}");
                                LogMessage($"  - Status: {tag.Status}");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogMessage($"Error testing tag '{dp.DataTagName}': {ex.Message}");
                        }
                    }
                }
            }
            LogMessage("=== END DEBUG INFO ===");
        }

        #endregion
    }

    public class SortableBindingList<T> : BindingList<T>
    {
        public SortableBindingList(IList<T> list) : base(list) { }
    }
}