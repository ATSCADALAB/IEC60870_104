// File: Forms/MainForm.cs - Hoàn thiện và sửa lỗi
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
using static IEC60870.IE.IeDoublePointWithQuality;

namespace IEC60870ServerWinForm.Forms
{
    public partial class MainForm : Form
    {
        // Services
        private readonly IEC60870ServerService _serverService;
        private readonly ConfigManager _configManager;
        private readonly DriverManager _driverManager;

        // Data
        private ServerConfig _serverConfig;
        private List<DataPoint> _dataPoints;
        private BindingSource _dataPointsBindingSource;

        // Timers
        private Timer _tagScanTimer;
        private Timer _dataSendTimer;

        // ATSCADA Driver - nhận từ form chính
        public iDriver iDriver1 { get; set; }

        public MainForm()
        {
            InitializeComponent();

            _serverService = new IEC60870ServerService();
            _configManager = new ConfigManager();
            _driverManager = new DriverManager();

            // Setup events
            _driverManager.LogMessage += LogMessage;
            _serverService.OnLogMessage += LogMessage;

            // ✅ SỬA LỖI: Event OnAsduReceived nhận ASdu parameter
            _serverService.OnAsduReceived += HandleReceivedAsdu;

            // Setup timers với interval hợp lý
            _tagScanTimer = new Timer { Interval = 1000 }; // Scan mỗi 1 giây
            _tagScanTimer.Tick += (s, e) => UpdateTagValues();

            _dataSendTimer = new Timer { Interval = 3000 }; // Gửi data mỗi 3 giây
            _dataSendTimer.Tick += (s, e) => SendAllValidData();
        }

        /// <summary>
        /// Khởi tạo driver từ iDriver1 trong form chính
        /// </summary>
        public void InitializeDriver(iDriver driver, string defaultTaskName = "")
        {
            try
            {
                iDriver1 = driver;
                _driverManager.Initialize(driver, defaultTaskName);

                LogMessage($"✅ Driver initialized successfully!");
                LogMessage($"   Default Task: '{defaultTaskName}'");

                // Test driver ngay sau khi khởi tạo
                TestDriverConnection();
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error initializing driver: {ex.Message}");
            }
        }

        /// <summary>
        /// Phương thức để set driver - tương thích với code cũ
        /// </summary>
        public void SetDriver(iDriver driver, string defaultTaskName = "")
        {
            InitializeDriver(driver, defaultTaskName);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Load config và data
                _serverConfig = _configManager.LoadServerConfig();

                // Setup data binding
                _dataPointsBindingSource = new BindingSource
                {
                    DataSource = new BindingList<DataPoint>(_dataPoints)
                };
                dgvDataPoints.DataSource = _dataPointsBindingSource;

                // Configure grid
                SetupDataGrid();
                UpdateServerStatusUI();

                LogMessage("📊 Application loaded successfully");
                LogMessage($"   Data Points: {_dataPoints.Count}");

                // Kiểm tra xem có driver chưa
                if (_driverManager.IsInitialized)
                {
                    LogMessage("🔄 Starting tag scanning...");
                    _tagScanTimer.Start();
                }
                else
                {
                    LogMessage("⚠️  Driver not initialized yet. Call SetDriver() first.");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error loading application: {ex.Message}");
            }
        }

        private void SetupDataGrid()
        {
            try
            {
                // Configure column widths
                if (dgvDataPoints.Columns["IOA"] != null)
                    dgvDataPoints.Columns["IOA"].Width = 60;

                if (dgvDataPoints.Columns["Name"] != null)
                    dgvDataPoints.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                if (dgvDataPoints.Columns["Type"] != null)
                    dgvDataPoints.Columns["Type"].Width = 100;

                if (dgvDataPoints.Columns["Value"] != null)
                    dgvDataPoints.Columns["Value"].Width = 100;

                if (dgvDataPoints.Columns["DataTagName"] != null)
                {
                    dgvDataPoints.Columns["DataTagName"].Width = 200;
                    dgvDataPoints.Columns["DataTagName"].HeaderText = "Tag Path (Task.Tag)";
                }

                // Hide unnecessary columns
                var hideColumns = new[] { "Description", "ConvertedValue", "LastUpdated" };
                foreach (var col in hideColumns)
                {
                    if (dgvDataPoints.Columns[col] != null)
                        dgvDataPoints.Columns[col].Visible = false;
                }

                // Add color coding for validity
                dgvDataPoints.CellFormatting += (s, e) =>
                {
                    if (dgvDataPoints.Columns["IsValid"] != null &&
                        e.ColumnIndex == dgvDataPoints.Columns["IsValid"].Index)
                    {
                        if (e.Value is bool isValid)
                        {
                            e.CellStyle.BackColor = isValid ?
                                System.Drawing.Color.LightGreen :
                                System.Drawing.Color.LightPink;
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error setting up data grid: {ex.Message}");
            }
        }

        #region Timer Methods - Cải tiến để đọc từ iDriver1

        /// <summary>
        /// ✅ CẢI TIẾN: Sử dụng GetMultipleTagValues để đọc nhiều tag cùng lúc
        /// </summary>
        private void UpdateTagValues()
        {
            if (!_driverManager.IsInitialized)
            {
                return;
            }

            try
            {
                // Lấy danh sách các tag paths cần đọc
                var tagPaths = _dataPoints
                    .Where(dp => !string.IsNullOrEmpty(dp.DataTagName))
                    .Select(dp => dp.DataTagName)
                    .ToList();

                if (tagPaths.Count == 0) return;

                // ✅ HIỆU QUẢ HỚN: Đọc tất cả tag cùng lúc
                var tagValues = _driverManager.GetMultipleTagValues(tagPaths);

                bool hasChanges = false;
                int successCount = 0;
                int errorCount = 0;

                foreach (var dataPoint in _dataPoints)
                {
                    if (string.IsNullOrEmpty(dataPoint.DataTagName))
                        continue;

                    try
                    {
                        // Lấy giá trị từ kết quả đã đọc
                        var newValue = tagValues.ContainsKey(dataPoint.DataTagName)
                            ? tagValues[dataPoint.DataTagName]
                            : null;

                        var isGood = !string.IsNullOrEmpty(newValue) &&
                                   _driverManager.IsTagGood(dataPoint.DataTagName);

                        // Chỉ update nếu có thay đổi
                        if (dataPoint.Value != newValue || dataPoint.IsValid != isGood)
                        {
                            dataPoint.Value = newValue ?? "null";
                            dataPoint.IsValid = isGood;
                            dataPoint.LastUpdated = DateTime.Now;

                            // ✅ Convert value theo DataType
                            if (isGood && !string.IsNullOrEmpty(newValue))
                            {
                                dataPoint.ConvertedValue = dataPoint.ConvertValueByDataType(newValue);
                            }

                            hasChanges = true;
                        }

                        if (isGood && !string.IsNullOrEmpty(newValue))
                            successCount++;
                        else
                            errorCount++;
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"❌ Error processing tag '{dataPoint.DataTagName}': {ex.Message}");
                        dataPoint.IsValid = false;
                        errorCount++;
                    }
                }

                // Update UI nếu có thay đổi
                if (hasChanges)
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() => _dataPointsBindingSource.ResetBindings(false)));
                    }
                    else
                    {
                        _dataPointsBindingSource.ResetBindings(false);
                    }
                }

                // Log thống kê mỗi 10 giây
                if (DateTime.Now.Second % 10 == 0)
                {
                    LogMessage($"📈 Tag Scan: {successCount} OK, {errorCount} Error, {tagPaths.Count} Total");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error in UpdateTagValues: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ SỬA LỖI: Sử dụng BroadcastAsdu thay vì SendSpontaneousData không tồn tại
        /// </summary>
        private void SendAllValidData()
        {
            if (!_serverService.IsRunning) return;

            try
            {
                var validPoints = _dataPoints
                    .Where(p => p.IsValid && !string.IsNullOrEmpty(p.Value))
                    .ToList();

                if (validPoints.Count == 0)
                {
                    // LogMessage("⚠️  No valid data points to send");
                    return;
                }

                foreach (var point in validPoints)
                {
                    try
                    {
                        // Convert data point thành IEC60870 object
                        var asdu = ConvertToASdu(point);
                        if (asdu != null)
                        {
                            // ✅ SỬA LỖI: Sử dụng BroadcastAsdu thay vì SendSpontaneousData
                            _serverService.BroadcastAsdu(asdu);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"❌ Error sending data point {point.IOA}: {ex.Message}");
                    }
                }

                LogMessage($"📤 Sent {validPoints.Count} data points to IEC104 clients");
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error in SendAllValidData: {ex.Message}");
            }
        }

        #endregion

        #region IEC104 Server Control

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_driverManager.IsInitialized)
                {
                    MessageBox.Show("Driver chưa được khởi tạo! Cần gọi SetDriver() trước.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _serverService.Start(_serverConfig);
                _dataSendTimer.Start();
                _tagScanTimer.Start();

                UpdateServerStatusUI();
                LogMessage("🚀 IEC104 Server started successfully");
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error starting server: {ex.Message}");
                MessageBox.Show($"Error starting server: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                _serverService.Stop();
                _dataSendTimer.Stop();

                UpdateServerStatusUI();
                LogMessage("🛑 IEC104 Server stopped");
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error stopping server: {ex.Message}");
            }
        }

        #endregion

        #region Value Conversion Helpers

        /// <summary>
        /// ✅ HELPER: Convert string thành bool an toàn
        /// </summary>
        private bool ConvertToBoolean(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;

            value = value.Trim().ToLower();
            return value == "1" || value == "true" || value == "on" || value == "yes";
        }

        /// <summary>
        /// ✅ HELPER: Convert string thành int an toàn
        /// </summary>
        private int ConvertToInt32(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;

            if (int.TryParse(value.Trim(), out int result))
                return result;

            // Thử convert từ float rồi làm tròn
            if (float.TryParse(value.Trim(), out float floatResult))
                return (int)Math.Round(floatResult);

            return 0;
        }

        /// <summary>
        /// ✅ HELPER: Convert string thành float an toàn
        /// </summary>
        private float ConvertToSingle(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0f;

            if (float.TryParse(value.Trim(), out float result))
                return result;

            return 0f;
        }

        /// <summary>
        /// ✅ HELPER: Convert string thành uint an toàn (cho counter)
        /// </summary>
        private uint ConvertToUInt32(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;

            if (uint.TryParse(value.Trim(), out uint result))
                return result;

            // Thử convert từ int (nếu âm thì trả về 0)
            if (int.TryParse(value.Trim(), out int intResult))
                return intResult >= 0 ? (uint)intResult : 0;

            return 0;
        }

        #endregion

        /// <summary>
        /// ✅ THÊM MỚI: Convert DataPoint thành ASdu để gửi
        /// </summary>
        private ASdu ConvertToASdu(DataPoint point)
        {
            try
            {
                InformationObject infoObj = ConvertToInformationObject(point);
                if (infoObj == null) return null;

                // Tạo ASdu với Spontaneous cause of transmission
                return new ASdu(
                    point.Type,                           // TypeId
                    false,                               // isSequenceOfElements
                    CauseOfTransmission.SPONTANEOUS,     // causeOfTransmission
                    false,                               // isTestFrame
                    false,                               // isNegativeConfirm
                    0,                                   // originatorAddress
                    1,                                   // commonAddress (có thể config)
                    new[] { infoObj }                    // informationObjects
                );
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error converting to ASdu {point.IOA}: {ex.Message}");
                return null;
            }
        }

        private InformationObject ConvertToInformationObject(DataPoint point)
        {
            try
            {
                switch (point.Type)
                {
                    case TypeId.M_SP_NA_1: // Single point
                        bool boolVal = ConvertToBoolean(point.Value);
                        var singlePoint = new IeSinglePointWithQuality(boolVal, false, false, false, false);
                        return new InformationObject(point.IOA,
                            new[] { new InformationElement[] { singlePoint } });

                    case TypeId.M_DP_NA_1: // Double point
                        int dpVal = ConvertToInt32(point.Value);
                        DoublePointInformation dpState;
                        if (dpVal == 0) dpState = DoublePointInformation.Off;
                        else if (dpVal == 1) dpState = DoublePointInformation.On;
                        else dpState = DoublePointInformation.IndeterminateOrIntermediate;

                        var doublePoint = new IeDoublePointWithQuality(dpState, false, false, false, false);
                        return new InformationObject(point.IOA,
                            new[] { new InformationElement[] { doublePoint } });

                    case TypeId.M_ME_NC_1: // Float with quality
                        float floatVal = ConvertToSingle(point.Value);
                        var shortFloat = new IeShortFloat(floatVal);
                        var qualityFloat = new IeQuality(false, false, false, false, false);
                        return new InformationObject(point.IOA,
                            new[] { new InformationElement[] { shortFloat, qualityFloat } });

                    case TypeId.M_ME_NB_1: // Scaled value with quality
                        int scaledVal = ConvertToInt32(point.Value);
                        var scaledValue = new IeScaledValue(scaledVal);
                        var qualityScaled = new IeQuality(false, false, false, false, false);
                        return new InformationObject(point.IOA,
                            new[] { new InformationElement[] { scaledValue, qualityScaled } });

                    case TypeId.M_ME_NA_1: // Normalized value with quality
                        float normVal = ConvertToSingle(point.Value);
                        // Normalized value: -1.0 to +1.0 mapped to -32768 to +32767
                        int normalizedInt = (int)(Math.Max(-1.0f, Math.Min(1.0f, normVal)) * 32767);
                        var normalizedValue = new IeNormalizedValue(normalizedInt);
                        var qualityNorm = new IeQuality(false, false, false, false, false);
                        return new InformationObject(point.IOA,
                            new[] { new InformationElement[] { normalizedValue, qualityNorm } });

                    case TypeId.M_IT_NA_1: // Integrated totals (Counter)
                        uint counterVal = ConvertToUInt32(point.Value);
                        var binaryCounter = new IeBinaryCounterReading((int)counterVal, 0, false, false, false);
                        return new InformationObject(point.IOA,
                            new[] { new InformationElement[] { binaryCounter } });

                    case TypeId.M_BO_NA_1: // Bitstring 32 bit
                        uint bitstringVal = ConvertToUInt32(point.Value);
                        var binaryState = new IeBinaryStateInformation(int.Parse(bitstringVal.ToString()));
                        return new InformationObject(point.IOA,
                            new[] { new InformationElement[] { binaryState } });

                    default:
                        LogMessage($"⚠️  Unsupported type {point.Type} for IOA {point.IOA}");
                        return null;
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error converting data point {point.IOA}: {ex.Message}");
                return null;
            }
        }

        #region UI Methods

        private void UpdateServerStatusUI()
        {
            try
            {
                bool running = _serverService.IsRunning;

                if (lblServerStatus != null)
                {
                    lblServerStatus.Text = running ? "Status: Running" : "Status: Stopped";
                    lblServerStatus.ForeColor = running ? System.Drawing.Color.Green : System.Drawing.Color.Red;
                }

                if (btnStart != null)
                    btnStart.Enabled = !running;

                if (btnStop != null)
                    btnStop.Enabled = running;
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error updating server status UI: {ex.Message}");
            }
        }

        private void LogMessage(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(LogMessage), message);
                return;
            }

            try
            {
                if (txtLogs != null)
                {
                    if (txtLogs.Text.Length > 15000)
                        txtLogs.Text = txtLogs.Text.Substring(7500);

                    txtLogs.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
                    txtLogs.SelectionStart = txtLogs.Text.Length;
                    txtLogs.ScrollToCaret();
                }
            }
            catch { }
        }

        /// <summary>
        /// ✅ SỬA LỖI: Event handler nhận ASdu parameter
        /// </summary>
        private void HandleReceivedAsdu(ASdu asdu)
        {
            try
            {
                LogMessage($"📨 Received ASDU: Type={asdu.GetTypeIdentification()}, " +
                          $"COT={asdu.GetCauseOfTransmission()}, " +
                          $"CA={asdu.GetCommonAddress()}");

                // Có thể xử lý commands từ client ở đây
                HandleClientCommands(asdu);
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error handling received ASDU: {ex.Message}");
            }
        }

        /// <summary>
        /// Xử lý commands từ IEC104 client
        /// </summary>
        private void HandleClientCommands(ASdu asdu)
        {
            try
            {
                var typeId = asdu.GetTypeIdentification();
                var cot = asdu.GetCauseOfTransmission();

                // Xử lý các loại command khác nhau
                switch (typeId)
                {
                    case TypeId.C_SC_NA_1: // Single command
                        LogMessage($"🎛️  Received Single Command");
                        break;

                    case TypeId.C_IC_NA_1: // Interrogation command
                        LogMessage($"🔍 Received Interrogation Command - sending all data");
                        SendAllValidData(); // Gửi tất cả data hiện tại
                        break;

                    case TypeId.C_SE_NC_1: // Set point command
                        LogMessage($"📊 Received Set Point Command");
                        break;

                    default:
                        LogMessage($"❓ Received unknown command type: {typeId}");
                        break;
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error processing client command: {ex.Message}");
            }
        }

        #endregion

        #region Data Point Management

        /// <summary>
        /// Add new data point
        /// </summary>
        private void btnAddPoint_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new DataPointForm())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        _dataPoints.Add(form.DataPoint);
                        RefreshDataPointsGrid();
                        LogMessage($"✅ Added data point: IOA={form.DataPoint.IOA}, Name={form.DataPoint.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error adding data point: {ex.Message}");
                MessageBox.Show($"Error adding data point: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Edit selected data point
        /// </summary>
        private void btnEditPoint_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvDataPoints.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a data point to edit.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var selectedPoint = dgvDataPoints.SelectedRows[0].DataBoundItem as DataPoint;
                if (selectedPoint != null)
                {
                    using (var form = new DataPointForm(selectedPoint))
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            RefreshDataPointsGrid();
                            LogMessage($"✅ Updated data point: IOA={form.DataPoint.IOA}, Name={form.DataPoint.Name}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error editing data point: {ex.Message}");
                MessageBox.Show($"Error editing data point: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Delete selected data point
        /// </summary>
        private void btnDeletePoint_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvDataPoints.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a data point to delete.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var selectedPoint = dgvDataPoints.SelectedRows[0].DataBoundItem as DataPoint;
                if (selectedPoint != null)
                {
                    var result = MessageBox.Show(
                        $"Are you sure you want to delete data point?\n\nIOA: {selectedPoint.IOA}\nName: {selectedPoint.Name}",
                        "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        _dataPoints.Remove(selectedPoint);
                        RefreshDataPointsGrid();
                        LogMessage($"✅ Deleted data point: IOA={selectedPoint.IOA}, Name={selectedPoint.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error deleting data point: {ex.Message}");
                MessageBox.Show($"Error deleting data point: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Send selected data point immediately
        /// </summary>
        private void btnSendSelected_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvDataPoints.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a data point to send.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var selectedPoint = dgvDataPoints.SelectedRows[0].DataBoundItem as DataPoint;
                if (selectedPoint != null && selectedPoint.IsValid)
                {
                    var asdu = ConvertToASdu(selectedPoint);
                    if (asdu != null)
                    {
                        _serverService.BroadcastAsdu(asdu);
                        LogMessage($"📤 Sent data point: IOA={selectedPoint.IOA}, Value={selectedPoint.Value}");
                    }
                }
                else
                {
                    MessageBox.Show("Selected data point is not valid or has no value.", "Invalid Data",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error sending data point: {ex.Message}");
                MessageBox.Show($"Error sending data point: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Refresh data points grid
        /// </summary>
        private void RefreshDataPointsGrid()
        {
            try
            {
                if (_dataPointsBindingSource != null)
                {
                    _dataPointsBindingSource.ResetBindings(false);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error refreshing grid: {ex.Message}");
            }
        }

        #endregion

        #region Form Events

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                LogMessage("🔄 Shutting down application...");

                _tagScanTimer?.Stop();
                _dataSendTimer?.Stop();

                if (_serverService.IsRunning)
                    _serverService.Stop();

                _configManager.SaveDataPoints(_dataPoints);
                _driverManager.Dispose();

                LogMessage("✅ Application closed successfully");
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error during shutdown: {ex.Message}");
            }
        }

        #endregion

        #region Debug Methods

        /// <summary>
        /// Test kết nối driver và hiển thị thông tin debug
        /// </summary>
        public void TestDriverConnection()
        {
            LogMessage("🔧 === DRIVER CONNECTION TEST ===");
            LogMessage($"   Driver Initialized: {_driverManager.IsInitialized}");
            LogMessage($"   Default Task: '{_driverManager.DefaultTaskName}'");
            LogMessage($"   iDriver1 Object: {(iDriver1 != null ? "✅ Available" : "❌ Null")}");

            if (_dataPoints.Count > 0)
            {
                var testPoints = _dataPoints.Take(3).Where(p => !string.IsNullOrEmpty(p.DataTagName));

                foreach (var testPoint in testPoints)
                {
                    try
                    {
                        var info = _driverManager.GetTagInfo(testPoint.DataTagName);
                        LogMessage($"   Test Tag: {info}");
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"   Test Tag Error: {ex.Message}");
                    }
                }
            }
            else
            {
                LogMessage("   No data points configured for testing");
            }

            LogMessage("🔧 === END CONNECTION TEST ===");
        }

        /// <summary>
        /// Thêm data point mới với TypeId
        /// </summary>
        public void AddDataPoint(int ioa, string name, TypeId type, string tagPath)
        {
            try
            {
                var newPoint = new DataPoint
                {
                    IOA = ioa,
                    Name = name,
                    DataTagName = tagPath,
                    IsValid = false,
                    Value = "",
                    LastUpdated = DateTime.Now
                };

                // ✅ Set TypeId và tự động mapping DataType
                newPoint.SetTypeId(type);

                _dataPoints.Add(newPoint);
                _dataPointsBindingSource.ResetBindings(false);

                LogMessage($"➕ Added data point: IOA={ioa}, Type={newPoint.GetTypeIdDisplayName()}, DataType={newPoint.GetDataTypeDisplayName()}, Tag={tagPath}");
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error adding data point: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ THÊM MỚI: Thêm data point với DataType (tự động mapping TypeId)
        /// </summary>
        public void AddDataPointByDataType(int ioa, string name, DataType dataType, string tagPath)
        {
            try
            {
                var newPoint = new DataPoint
                {
                    IOA = ioa,
                    Name = name,
                    DataTagName = tagPath,
                    IsValid = false,
                    Value = "",
                    LastUpdated = DateTime.Now
                };

                // ✅ Set DataType và tự động mapping TypeId
                newPoint.SetDataType(dataType);

                _dataPoints.Add(newPoint);
                _dataPointsBindingSource.ResetBindings(false);

                LogMessage($"➕ Added data point: IOA={ioa}, DataType={newPoint.GetDataTypeDisplayName()}, Type={newPoint.GetTypeIdDisplayName()}, Tag={tagPath}");
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error adding data point: {ex.Message}");
            }
        }

        /// <summary>
        /// Force scan tất cả tags ngay lập tức
        /// </summary>
        public void ForceScanTags()
        {
            LogMessage("🔄 Force scanning all tags...");
            UpdateTagValues();
        }

        /// <summary>
        /// Force send tất cả data ngay lập tức
        /// </summary>
        public void ForceSendData()
        {
            LogMessage("📤 Force sending all data...");
            SendAllValidData();
        }

        #endregion

        #region Menu Event Handlers

        /// <summary>
        /// Exit application
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (_serverService.IsRunning)
                {
                    var result = MessageBox.Show(
                        "Server is still running. Do you want to stop it and exit?",
                        "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        _serverService.Stop();
                        _dataSendTimer.Stop();
                        _tagScanTimer.Stop();
                        Application.Exit();
                    }
                }
                else
                {
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error during exit: {ex.Message}");
                Application.Exit();
            }
        }

        /// <summary>
        /// Configure server settings
        /// </summary>
        private void configureServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new ServerConfigForm(_serverConfig))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        _serverConfig = form.ServerConfig;
                        LogMessage("✅ Server configuration updated");
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error opening server configuration: {ex.Message}");
                MessageBox.Show($"Error opening server configuration: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Show about dialog
        /// </summary>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(
                    "IEC 60870-5-104 Server\n\n" +
                    "Version: 1.0\n" +
                    "Built with .NET Framework\n\n" +
                    "Features:\n" +
                    "• SCADA Integration\n" +
                    "• IEC104 Protocol Support\n" +
                    "• Real-time Data Transmission\n" +
                    "• Command Reception\n\n" +
                    "© 2025 IEC104 Server Project",
                    "About IEC104 Server",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error showing about dialog: {ex.Message}");
            }
        }

        #endregion
    }
}