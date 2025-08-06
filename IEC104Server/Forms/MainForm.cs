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
using IEC104Server.Services;
using System.IO;
using IEC104Server.Models;

namespace IEC60870ServerWinForm.Forms
{
    public partial class MainForm : Form
    {
        // Services
        private readonly IEC60870ServerService _serverService;
        private readonly ConfigManager _configManager;
        private readonly DriverManager _driverManager;
        private readonly XmlConfigService _xmlConfigService;

        // Data
        private ServerConfig _serverConfig;
        private List<DataPoint> _dataPoints;
        private BindingSource _dataPointsBindingSource;

        // Timers
        private Timer _tagScanTimer;
        private Timer _dataSendTimer;

        //  OPTIMIZATION: UI update control
        private bool _enableGridValueUpdates = false; // Disable by default for performance

        //  File management
        private string _currentConfigFile = null;

        //  Write response optimization
        private bool _enableImmediateReadBack = true; // Enable immediate read back after write

        //  Driver optimization
        private bool _useDirectDriverOnly = true; // Skip DriverManager, use iDriver1 directly

        public MainForm()
        {
            InitializeComponent();

            _serverService = new IEC60870ServerService();
            _configManager = new ConfigManager();
            _driverManager = new DriverManager();
            _xmlConfigService = new XmlConfigService();

            //  Initialize _dataPoints early to prevent null reference
            _dataPoints = new List<DataPoint>();

            // Setup events
            _driverManager.LogMessage += LogMessage;
            _serverService.OnLogMessage += LogMessage;

            //  SỬA LỖI: Event OnAsduReceived nhận ASdu parameter
            _serverService.OnAsduReceived += HandleReceivedAsdu;

            // Setup timers với interval hợp lý -  OPTIMIZATION: Adaptive intervals
            _tagScanTimer = new Timer { Interval = GetOptimalScanInterval() };
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
                LogMessage($"🔧 InitializeDriver called with driver: {(driver != null ? "NOT NULL" : "NULL")}");

                iDriver1 = driver;
                LogMessage($"🔧 iDriver1 set: {(iDriver1 != null ? "NOT NULL" : "NULL")}");

                _driverManager.Initialize(driver, defaultTaskName);
                LogMessage($"🔧 DriverManager.Initialize completed");

                LogMessage($" Driver initialized successfully!");
                LogMessage($"   iDriver1: {(iDriver1 != null ? "Available" : "NULL")}");
                LogMessage($"   DriverManager.IsInitialized: {_driverManager.IsInitialized}");
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

                //  Initialize _dataPoints if null
                //_dataPoints = _configManager.LoadDataPoints() ?? new List<DataPoint>();
                // LogMessage($"🔧 _dataPoints initialized: Count={_dataPoints.Count}");

                // Setup data binding
                _dataPointsBindingSource = new BindingSource
                {
                    DataSource = new BindingList<DataPoint>(_dataPoints)
                };
                dgvDataPoints.DataSource = _dataPointsBindingSource;

                // Configure grid
                SetupDataGrid();
                UpdateServerStatusUI();

                //LogMessage(" Application loaded successfully");
                LogMessage($"Data Points: {_dataPoints.Count}");

                // Kiểm tra xem có driver chưa
                if (_driverManager.IsInitialized)
                {
                    //LogMessage("🔄 Starting tag scanning...");
                    _tagScanTimer.Start();
                }
                else
                {
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

                if (dgvDataPoints.Columns["DataTagName"] != null)
                {
                    dgvDataPoints.Columns["DataTagName"].Width = 200;
                    dgvDataPoints.Columns["DataTagName"].HeaderText = "Tag Name";
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

        #region Performance Optimization

        /// <summary>
        ///  OPTIMIZATION: Calculate optimal scan interval based on data point count
        /// </summary>
        private int GetOptimalScanInterval()
        {
            var pointCount = _dataPoints?.Count ?? 0;

            if (pointCount <= 50)
                return 1000;    // 1 second for small datasets
            else if (pointCount <= 200)
                return 2000;    // 2 seconds for medium datasets
            else if (pointCount <= 500)
                return 3000;    // 3 seconds for large datasets
            else if (pointCount <= 1000)
                return 5000;    // 5 seconds for very large datasets
            else
                return 10000;   // 10 seconds for massive datasets
        }

        /// <summary>
        ///  OPTIMIZATION: Update scan interval when data points change
        /// </summary>
        public void UpdateScanInterval()
        {
            if (_tagScanTimer != null)
            {
                var newInterval = GetOptimalScanInterval();
                if (_tagScanTimer.Interval != newInterval)
                {
                    _tagScanTimer.Interval = newInterval;
                    LogMessage($" Scan interval updated to {newInterval}ms for {_dataPoints.Count} data points");
                }
            }
        }

        #endregion

        #region Timer Methods - Cải tiến để đọc từ iDriver1

        /// <summary>
        ///  OPTIMIZED: Batch reading cho hiệu suất cao với nhiều tags
        /// </summary>
        private void UpdateTagValues()
        {
            if (iDriver1 == null)
            {
                return;
            }

            try
            {
                var startTime = DateTime.Now;

                //  OPTIMIZATION 1: Batch read tất cả tags cùng lúc
                var tagPaths = _dataPoints
                    .Where(dp => !string.IsNullOrEmpty(dp.DataTagName))
                    .Select(dp => dp.DataTagName)
                    .ToList();

                if (tagPaths.Count == 0) return;

                //  OPTIMIZATION 2: Use direct iDriver1 reads (skip DriverManager)
                var tagValues = GetTagValuesDirectly(tagPaths);

                var readTime = DateTime.Now - startTime;

                //  OPTIMIZATION 3: Process results nhanh chóng
                bool hasChanges = false;
                int successCount = 0;
                int errorCount = 0;
                int changedCount = 0;

                foreach (var dataPoint in _dataPoints)
                {
                    if (string.IsNullOrEmpty(dataPoint.DataTagName))
                        continue;

                    try
                    {
                        // Lấy value từ batch result
                        var newValue = tagValues.ContainsKey(dataPoint.DataTagName)
                            ? tagValues[dataPoint.DataTagName]
                            : null;

                        var isGood = !string.IsNullOrEmpty(newValue);

                        // Chỉ update nếu có thay đổi
                        if (dataPoint.Value != newValue || dataPoint.IsValid != isGood)
                        {
                            var oldValue = dataPoint.Value;
                            dataPoint.Value = newValue ?? "null";
                            dataPoint.IsValid = isGood;
                            dataPoint.LastUpdated = DateTime.Now;

                            //  Convert value theo DataType
                            if (isGood && !string.IsNullOrEmpty(newValue))
                            {
                                dataPoint.ConvertedValue = dataPoint.ConvertValueByDataType(newValue);
                            }

                            hasChanges = true;
                            changedCount++;
                        }

                        if (isGood && !string.IsNullOrEmpty(newValue))
                            successCount++;
                        else
                            errorCount++;
                    }
                    catch (Exception ex)
                    {
                        //  Chỉ log error đầu tiên để tránh spam
                        if (errorCount == 0)
                        {
                            LogMessage($"❌ Error processing tags: {ex.Message} (and possibly more...)");
                        }
                        dataPoint.IsValid = false;
                        errorCount++;
                    }
                }

                //  OPTIMIZATION 5: Optional UI update - chỉ khi enable và dataset nhỏ
                if (hasChanges && _enableGridValueUpdates && _dataPoints.Count <= 100)
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

                var totalTime = DateTime.Now - startTime;

                //  Chỉ log khi có vấn đề hoặc thay đổi đáng kể
                if (errorCount > 0 || totalTime.TotalMilliseconds > 2000 || changedCount > 10)
                {
                    LogMessage($" SCADA Scan: {successCount} Good, {errorCount} Error, {changedCount} Changed | Time: {totalTime.TotalMilliseconds:F0}ms");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error in UpdateTagValues: {ex.Message}");
            }
        }

        /// <summary>
        ///  FALLBACK: Direct tag reading using iDriver1 when DriverManager fails
        /// </summary>
        private Dictionary<string, string> GetTagValuesDirectly(List<string> tagPaths)
        {
            var results = new Dictionary<string, string>();

            if (iDriver1 == null)
            {
                LogMessage($"❌ iDriver1 not available for tag reading");
                return results;
            }

            foreach (var tagPath in tagPaths)
            {
                try
                {
                    // Parse task và tag từ DataTagName
                    var parts = tagPath.Split('.');
                    string taskName, tagName;

                    if (parts.Length >= 2)
                    {
                        taskName = parts[0];
                        tagName = parts.Length > 2 ? string.Join(".", parts, 1, parts.Length - 1) : parts[1];
                    }
                    else
                    {
                        taskName = _driverManager.DefaultTaskName ?? "DefaultTask";
                        tagName = tagPath;
                    }

                    //  Direct read từ iDriver1
                    var value = iDriver1.Task(taskName).Tag(tagName).Value?.ToString();
                    results[tagPath] = value;
                }
                catch (Exception ex)
                {
                    LogMessage($"❌ Error reading tag '{tagPath}': {ex.Message}");
                    results[tagPath] = null;
                }
            }

            return results;
        }

        /// <summary>
        ///  SỬA LỖI: Sử dụng BroadcastAsdu thay vì SendSpontaneousData không tồn tại
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
                            //  SỬA LỖI: Sử dụng BroadcastAsdu thay vì SendSpontaneousData
                            _serverService.BroadcastAsdu(asdu);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"❌ Error sending data point {point.IOA}: {ex.Message}");
                    }
                }

                //  Chỉ log khi có nhiều data points hoặc có vấn đề
                if (validPoints.Count > 50 || validPoints.Count == 0)
                {
                    LogMessage($"📤 Sent {validPoints.Count} data points to IEC104 clients");
                }
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
                if (iDriver1 == null)
                {
                    MessageBox.Show("iDriver1 chưa được khởi tạo! Cần gọi SetDriver() hoặc InitializeDriver() trước.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _serverService.Start(_serverConfig);
                _dataSendTimer.Start();
                _tagScanTimer.Start();

                UpdateServerStatusUI();
                LogMessage(" IEC104 Server started successfully");
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
        ///  HELPER: Convert string thành bool an toàn
        /// </summary>
        private bool ConvertToBoolean(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;

            value = value.Trim().ToLower();
            return value == "1" || value == "true" || value == "on" || value == "yes";
        }

        /// <summary>
        ///  HELPER: Convert string thành int an toàn
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
        ///  HELPER: Convert string thành float an toàn
        /// </summary>
        private float ConvertToSingle(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0f;

            if (float.TryParse(value.Trim(), out float result))
                return result;

            return 0f;
        }

        /// <summary>
        ///  HELPER: Convert string thành uint an toàn (cho counter)
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
        ///  THÊM MỚI: Convert DataPoint thành ASdu để gửi
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
        ///  SỬA LỖI: Event handler nhận ASdu parameter
        /// </summary>
        private void HandleReceivedAsdu(ASdu asdu)
        {
            try
            {
                var typeId = asdu.GetTypeIdentification();

                //  Chỉ log commands quan trọng, không log Interrogation spam
                if (typeId != TypeId.C_IC_NA_1)
                {
                    LogMessage($"📨 Received ASDU: Type={typeId}, " +
                              $"COT={asdu.GetCauseOfTransmission()}, " +
                              $"CA={asdu.GetCommonAddress()}");
                }

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
                        HandleSingleCommand(asdu);
                        break;

                    case TypeId.C_IC_NA_1: // Interrogation command
                        // LogMessage($"🔍 Received Interrogation Command - sending all data");
                        SendAllValidData(); // Gửi tất cả data hiện tại
                        break;

                    case TypeId.C_SE_NC_1: // Set point command (float)
                        LogMessage($" Received Set Point Float Command");
                        HandleSetPointFloatCommand(asdu);
                        break;

                    case TypeId.C_SE_NB_1: // Set point command (int)
                        LogMessage($" Received Set Point Int Command");
                        HandleSetPointIntCommand(asdu);
                        break;

                    case TypeId.C_DC_NA_1: // Double command
                        LogMessage($"🎛️  Received Double Command");
                        HandleDoubleCommand(asdu);
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

        /// <summary>
        ///  Handle Single Command (C_SC_NA_1) - Boolean control
        /// </summary>
        private void HandleSingleCommand(ASdu asdu)
        {
            try
            {
                var informationObjects = asdu.GetInformationObjects();
                if (informationObjects == null || informationObjects.Length == 0)
                {
                    LogMessage($"❌ Single Command: No information objects");
                    return;
                }

                foreach (var infoObj in informationObjects)
                {
                    var ioa = infoObj.GetInformationObjectAddress();
                    var elements = infoObj.GetInformationElements();

                    if (elements != null && elements.Length > 0 && elements[0] != null && elements[0].Length > 0)
                    {
                        var element = elements[0][0];

                        //  Extract command value using IeSingleCommand
                        bool commandValue = false;
                        if (element is IeSingleCommand singleCommand)
                        {
                            commandValue = singleCommand.IsCommandStateOn();
                        }

                        LogMessage($"🎛️  Single Command: IOA={ioa}, Value={commandValue}");

                        //  WRITE BACK TO SCADA
                        WriteToSCADA(ioa, commandValue);

                        // Send confirmation back to client
                        SendCommandConfirmation(ioa, TypeId.C_SC_NA_1, commandValue);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error handling single command: {ex.Message}");
            }
        }

        /// <summary>
        ///  Handle Set Point Float Command (C_SE_NC_1)
        /// </summary>
        private void HandleSetPointFloatCommand(ASdu asdu)
        {
            try
            {
                var informationObjects = asdu.GetInformationObjects();
                if (informationObjects == null || informationObjects.Length == 0)
                {
                    LogMessage($"❌ Set Point Float: No information objects");
                    return;
                }

                foreach (var infoObj in informationObjects)
                {
                    var ioa = infoObj.GetInformationObjectAddress();
                    var elements = infoObj.GetInformationElements();

                    if (elements != null && elements.Length > 0 && elements[0] != null && elements[0].Length > 0)
                    {
                        var element = elements[0][0];

                        //  Extract command value using IeShortFloat
                        float commandValue = 0.0f;
                        if (element is IeShortFloat shortFloat)
                        {
                            commandValue = shortFloat.GetValue();
                        }

                        LogMessage($" Set Point Float: IOA={ioa}, Value={commandValue}");

                        //  WRITE BACK TO SCADA
                        WriteToSCADA(ioa, commandValue);

                        // Send confirmation back to client
                        SendCommandConfirmation(ioa, TypeId.C_SE_NC_1, commandValue);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error handling set point float command: {ex.Message}");
            }
        }

        /// <summary>
        ///  Handle Set Point Int Command (C_SE_NB_1)
        /// </summary>
        private void HandleSetPointIntCommand(ASdu asdu)
        {
            try
            {
                var informationObjects = asdu.GetInformationObjects();
                if (informationObjects == null || informationObjects.Length == 0)
                {
                    LogMessage($"❌ Set Point Int: No information objects");
                    return;
                }

                foreach (var infoObj in informationObjects)
                {
                    var ioa = infoObj.GetInformationObjectAddress();
                    var elements = infoObj.GetInformationElements();

                    if (elements != null && elements.Length > 0 && elements[0] != null && elements[0].Length > 0)
                    {
                        var element = elements[0][0];

                        //  Extract command value using IeScaledValue
                        int commandValue = 0;
                        if (element is IeScaledValue scaledValue)
                        {
                            commandValue = scaledValue.GetValue();
                        }

                        LogMessage($" Set Point Int: IOA={ioa}, Value={commandValue}");

                        //  WRITE BACK TO SCADA
                        WriteToSCADA(ioa, commandValue);

                        // Send confirmation back to client
                        SendCommandConfirmation(ioa, TypeId.C_SE_NB_1, commandValue);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error handling set point int command: {ex.Message}");
            }
        }

        /// <summary>
        ///  Handle Double Command (C_DC_NA_1)
        /// </summary>
        private void HandleDoubleCommand(ASdu asdu)
        {
            try
            {
                var informationObjects = asdu.GetInformationObjects();
                if (informationObjects == null || informationObjects.Length == 0)
                {
                    LogMessage($"❌ Double Command: No information objects");
                    return;
                }

                foreach (var infoObj in informationObjects)
                {
                    var ioa = infoObj.GetInformationObjectAddress();
                    var elements = infoObj.GetInformationElements();

                    if (elements != null && elements.Length > 0 && elements[0] != null && elements[0].Length > 0)
                    {
                        var element = elements[0][0];

                        //  Extract command value using IeDoubleCommand
                        int commandValue = 0;
                        if (element is IeDoubleCommand doubleCommand)
                        {
                            commandValue = (int)doubleCommand.GetCommandState();
                        }

                        LogMessage($"🎛️  Double Command: IOA={ioa}, Value={commandValue}");

                        //  WRITE BACK TO SCADA
                        WriteToSCADA(ioa, commandValue);

                        // Send confirmation back to client
                        SendCommandConfirmation(ioa, TypeId.C_DC_NA_1, commandValue);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error handling double command: {ex.Message}");
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
                //  Debug: Check _dataPoints status
                LogMessage($"🔧 btnAddPoint_Click: _dataPoints is {(_dataPoints == null ? "NULL" : $"initialized with {_dataPoints.Count} items")}");

                if (_dataPoints == null)
                {
                    LogMessage("🔧 _dataPoints is null, initializing...");
                    _dataPoints = new List<DataPoint>();
                }

                using (var form = new DataPointForm())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        _dataPoints.Add(form.DataPoint);
                        RefreshDataPointsGrid();
                        LogMessage($" Added data point: IOA={form.DataPoint.IOA}, Name={form.DataPoint.Name}");
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
                            LogMessage($" Updated data point: IOA={form.DataPoint.IOA}, Name={form.DataPoint.Name}");
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
                        LogMessage($" Deleted data point: IOA={selectedPoint.IOA}, Name={selectedPoint.Name}");
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
        /// Refresh data points grid - chỉ cho config changes, không cho value updates
        /// </summary>
        private void RefreshDataPointsGrid()
        {
            try
            {
                if (_dataPointsBindingSource != null)
                {
                    //  OPTIMIZATION: Chỉ refresh grid khi config thay đổi (add/edit/delete)
                    // Không refresh cho value updates để tránh UI lag với 1000+ tags
                    _dataPointsBindingSource.ResetBindings(false);
                }

                //  OPTIMIZATION: Auto-adjust scan interval when data points change
                UpdateScanInterval();
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error refreshing grid: {ex.Message}");
            }
        }

        /// <summary>
        ///  THÊM MỚI: Force refresh grid manually (for debugging only)
        /// </summary>
        public void ForceRefreshGrid()
        {
            try
            {
                if (_dataPointsBindingSource != null)
                {
                    _dataPointsBindingSource.ResetBindings(false);
                    LogMessage("🔄 Grid manually refreshed");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error force refreshing grid: {ex.Message}");
            }
        }

        /// <summary>
        ///  THÊM MỚI: Toggle grid value updates (for debugging small datasets)
        /// </summary>
        public void ToggleGridValueUpdates(bool enable)
        {
            _enableGridValueUpdates = enable;
            LogMessage($" Grid value updates: {(enable ? "Enabled" : "Disabled")}");

            if (enable && _dataPoints.Count > 100)
            {
                LogMessage("⚠️  Warning: Grid updates enabled with large dataset - may cause UI lag");
            }
        }

        /// <summary>
        ///  THÊM MỚI: Toggle immediate read back after write commands
        /// </summary>
        public void ToggleImmediateReadBack(bool enable)
        {
            _enableImmediateReadBack = enable;
            LogMessage($"🔄 Immediate read back after write: {(enable ? "Enabled" : "Disabled")}");

            if (!enable)
            {
                LogMessage(" Clients will receive updates on next scan cycle (may take 1-10 seconds)");
            }
            else
            {
                LogMessage("⚡ Clients will receive immediate updates after write commands");
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

                LogMessage(" Application closed successfully");
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error during shutdown: {ex.Message}");
            }
        }

        /// <summary>
        ///  Send command confirmation back to client
        /// </summary>
        private void SendCommandConfirmation(int ioa, TypeId commandType, object value)
        {
            try
            {
                InformationObject[] infoObjects = null;

                switch (commandType)
                {
                    case TypeId.C_SC_NA_1: // Single Command
                        //  IeSingleCommand with proper parameters
                        var singleCommand = new IeSingleCommand((bool)value, 0, false); // value, qualifier=0, not select
                        infoObjects = new[] { new InformationObject(ioa, new InformationElement[][] { new InformationElement[] { singleCommand } }) };
                        break;

                    case TypeId.C_SE_NC_1: // Set Point Float
                        //  IeShortFloat
                        var shortFloat = new IeShortFloat((float)value);
                        infoObjects = new[] { new InformationObject(ioa, new InformationElement[][] { new InformationElement[] { shortFloat } }) };
                        break;

                    case TypeId.C_SE_NB_1: // Set Point Int
                        //  IeScaledValue
                        var scaledValue = new IeScaledValue((int)value);
                        infoObjects = new[] { new InformationObject(ioa, new InformationElement[][] { new InformationElement[] { scaledValue } }) };
                        break;

                    case TypeId.C_DC_NA_1: // Double Command
                        //  IeDoubleCommand
                        var doubleCommand = new IeDoubleCommand((DoubleCommandState)value, 0, false);
                        infoObjects = new[] { new InformationObject(ioa, new InformationElement[][] { new InformationElement[] { doubleCommand } }) };
                        break;
                }

                if (infoObjects != null)
                {
                    var confirmationAsdu = new ASdu(
                        commandType,
                        false, // Not sequence
                        CauseOfTransmission.ACTIVATION_CON, // Activation confirmation
                        false, // Not test
                        false, // Not negative
                        0, // Originator address
                        1, // Common address
                        infoObjects
                    );

                    _serverService.BroadcastAsdu(confirmationAsdu);
                    // LogMessage($" Command confirmation sent: IOA={ioa}, Type={commandType}, Value={value}");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error sending command confirmation: {ex.Message}");
            }
        }

        #endregion

        #region SCADA Write Operations

        /// <summary>
        ///  Write value to SCADA system using iDriver1
        /// </summary>
        private void WriteToSCADA(int ioa, object value)
        {
            try
            {
                if (iDriver1 == null)
                {
                    LogMessage($"❌ Write failed: iDriver1 not available");
                    return;
                }

                // Find data point by IOA
                var dataPoint = _dataPoints.FirstOrDefault(dp => dp.IOA == ioa);
                if (dataPoint == null)
                {
                    LogMessage($"❌ Write failed: IOA {ioa} not found in configuration");
                    return;
                }

                // Parse task and tag from DataTagName (format: "TaskName.TagName")
                var parts = dataPoint.DataTagName.Split('.');
                if (parts.Length < 2)
                {
                    LogMessage($"❌ Write failed: Invalid tag format '{dataPoint.DataTagName}'. Expected 'TaskName.TagName'");
                    return;
                }

                string taskName = parts[0];
                string tagName = parts.Length > 2 ? string.Join(".", parts, 1, parts.Length - 1) : parts[1];

                //  WRITE TO SCADA using iDriver1
                iDriver1.Task(taskName).Tag(tagName).Value = value.ToString();

                LogMessage($" SCADA Write: {dataPoint.DataTagName} = {value}");

                //  IMMEDIATE READ BACK and UPDATE CLIENT (if enabled)
                if (_enableImmediateReadBack)
                {
                    ReadBackAndUpdateClient(dataPoint, taskName, tagName);
                }
                else
                {
                    LogMessage($" Immediate read back disabled - client will get update on next scan cycle");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error writing to SCADA: {ex.Message}");
            }
        }

        /// <summary>
        ///  Immediate read back from SCADA after write and update client
        /// </summary>
        private void ReadBackAndUpdateClient(DataPoint dataPoint, string taskName, string tagName)
        {
            try
            {
                //  IMMEDIATE READ from SCADA
                var readBackValue = iDriver1.Task(taskName).Tag(tagName).Value?.ToString();
                var isGood = !string.IsNullOrEmpty(readBackValue);

                //  UPDATE DataPoint immediately
                if (dataPoint.Value != readBackValue || dataPoint.IsValid != isGood)
                {
                    var oldValue = dataPoint.Value;
                    dataPoint.Value = readBackValue ?? "null";
                    dataPoint.IsValid = isGood;
                    dataPoint.LastUpdated = DateTime.Now;

                    // Convert value theo DataType
                    if (isGood && !string.IsNullOrEmpty(readBackValue))
                    {
                        dataPoint.ConvertedValue = dataPoint.ConvertValueByDataType(readBackValue);
                    }

                    LogMessage($"🔄 Write feedback: {dataPoint.DataTagName} = {readBackValue}");

                    //  IMMEDIATE SEND to all clients
                    SendSingleDataPoint(dataPoint);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error reading back from SCADA: {ex.Message}");
            }
        }

        /// <summary>
        ///  Send single data point immediately to all clients
        /// </summary>
        private void SendSingleDataPoint(DataPoint dataPoint)
        {
            try
            {
                if (!dataPoint.IsValid || string.IsNullOrEmpty(dataPoint.Value))
                {
                    LogMessage($"⚠️  Skipping invalid data point: {dataPoint.Name}");
                    return;
                }

                //  Create InformationObject directly based on type
                InformationObject infoObj = null;

                switch (dataPoint.Type)
                {
                    case TypeId.M_SP_NA_1: // Single point
                        if (bool.TryParse(dataPoint.Value, out bool boolValue) ||
                            (int.TryParse(dataPoint.Value, out int intVal) && intVal != 0))
                        {
                            var singlePoint = new IeSinglePointWithQuality(boolValue, false, false, false, false);
                            infoObj = new InformationObject(dataPoint.IOA, new InformationElement[][] { new InformationElement[] { singlePoint } });
                        }
                        break;

                    case TypeId.M_ME_NC_1: // Short float
                        if (float.TryParse(dataPoint.Value, out float floatValue))
                        {
                            var shortFloat = new IeShortFloat(floatValue);
                            infoObj = new InformationObject(dataPoint.IOA, new InformationElement[][] { new InformationElement[] { shortFloat } });
                        }
                        break;

                    case TypeId.M_ME_NB_1: // Scaled value
                        if (int.TryParse(dataPoint.Value, out int intValue))
                        {
                            var scaledValue = new IeScaledValue(intValue);
                            infoObj = new InformationObject(dataPoint.IOA, new InformationElement[][] { new InformationElement[] { scaledValue } });
                        }
                        break;

                    default:
                        // Try as float for unknown types
                        if (float.TryParse(dataPoint.Value, out float defaultFloat))
                        {
                            var shortFloat = new IeShortFloat(defaultFloat);
                            infoObj = new InformationObject(dataPoint.IOA, new InformationElement[][] { new InformationElement[] { shortFloat } });
                        }
                        break;
                }

                if (infoObj != null)
                {
                    var asdu = new ASdu(
                        dataPoint.Type,
                        false, // Not sequence
                        CauseOfTransmission.SPONTANEOUS, // Spontaneous update
                        false, // Not test
                        false, // Not negative
                        0, // Originator address
                        1, // Common address
                        new InformationObject[] { infoObj }
                    );

                    _serverService.BroadcastAsdu(asdu);
                    // LogMessage($"📤 Immediate update sent: {dataPoint.Name} = {dataPoint.Value}");
                }
                else
                {
                    LogMessage($"⚠️  Could not create InformationObject for: {dataPoint.Name}");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error sending single data point: {ex.Message}");
            }
        }

        #endregion

        #region Debug Methods

        /// <summary>
        ///  THÊM MỚI: Test XML configuration functionality
        /// </summary>
        public void TestXmlConfiguration()
        {
            try
            {
                LogMessage("🔧 === XML CONFIGURATION TEST ===");

                // Test export
                var testFile = Path.Combine(Path.GetTempPath(), "test_config.xml");
                _xmlConfigService.ExportToXml(_dataPoints, testFile, "Test Project");
                LogMessage($" Export test: {testFile}");

                // Test file info
                var info = _xmlConfigService.GetXmlFileInfo(testFile);

                // Test import
                var importedPoints = _xmlConfigService.ImportFromXml(testFile);
                LogMessage($" Import test: {importedPoints.Count} tags loaded");

                // Cleanup
                if (File.Exists(testFile))
                    File.Delete(testFile);

                LogMessage("🔧 === XML TEST COMPLETED ===");
            }
            catch (Exception ex)
            {
                LogMessage($"❌ XML test error: {ex.Message}");
            }
        }

        /// <summary>
        /// Test kết nối driver và hiển thị thông tin debug
        /// </summary>
        public void TestDriverConnection()
        {
            LogMessage("🔧 === DRIVER CONNECTION TEST ===");
            LogMessage($"   iDriver1 Object: {(iDriver1 != null ? " Available" : "❌ Null")}");
            LogMessage($"   Default Task: '{_driverManager.DefaultTaskName}'");

            if (iDriver1 == null)
            {
                LogMessage("   ❌ Cannot test - iDriver1 is null");
                LogMessage("🔧 === END CONNECTION TEST ===");
                return;
            }

            if (_dataPoints.Count > 0)
            {
                var testPoints = _dataPoints.Take(3).Where(p => !string.IsNullOrEmpty(p.DataTagName));

                foreach (var testPoint in testPoints)
                {
                    try
                    {
                        // Parse task và tag từ DataTagName
                        var parts = testPoint.DataTagName.Split('.');
                        string taskName, tagName;

                        if (parts.Length >= 2)
                        {
                            taskName = parts[0];
                            tagName = parts.Length > 2 ? string.Join(".", parts, 1, parts.Length - 1) : parts[1];
                        }
                        else
                        {
                            taskName = _driverManager.DefaultTaskName ?? "DefaultTask";
                            tagName = testPoint.DataTagName;
                        }

                        //  Test đọc trực tiếp từ iDriver1
                        var value = iDriver1.Task(taskName).Tag(tagName).Value?.ToString();
                        var status = !string.IsNullOrEmpty(value) ? " OK" : "⚠️  NULL/EMPTY";

                        LogMessage($"   Test: {testPoint.DataTagName} -> Task='{taskName}', Tag='{tagName}', Value='{value ?? "null"}' {status}");
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"   ❌ Test Error: {testPoint.DataTagName} - {ex.Message}");
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

                //  Set TypeId và tự động mapping DataType
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
        ///  THÊM MỚI: Thêm data point với DataType (tự động mapping TypeId)
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

                //  Set DataType và tự động mapping TypeId
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

        /// <summary>
        ///  THÊM MỚI: Reset server config về default values
        /// </summary>
        public void ResetServerConfig()
        {
            try
            {
                _configManager.ResetToDefaultConfig();
                _serverConfig = _configManager.LoadServerConfig();
                LogMessage(" Server config reset to default values");
                LogMessage($"   T1: {_serverConfig.TimeoutT1}ms, T2: {_serverConfig.TimeoutT2}ms, T3: {_serverConfig.TimeoutT3}ms");
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error resetting server config: {ex.Message}");
            }
        }

        #endregion

        #region File Menu Event Handlers

        /// <summary>
        /// Save configuration to XML (quick save)
        /// </summary>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath;

                //  Use current file if available, otherwise create new
                if (!string.IsNullOrEmpty(_currentConfigFile))
                {
                    filePath = _currentConfigFile;
                }
                else
                {
                    // Create default filename and path
                    var defaultFileName = $"IEC104_Config_{DateTime.Now:yyyyMMdd_HHmmss}.xml";
                    var defaultPath = Path.Combine(Application.StartupPath, "Configs");

                    // Create Configs directory if not exists
                    if (!Directory.Exists(defaultPath))
                        Directory.CreateDirectory(defaultPath);

                    filePath = Path.Combine(defaultPath, defaultFileName);
                    _currentConfigFile = filePath; // Remember for next save
                }

                _xmlConfigService.ExportToXml(_dataPoints, filePath, "IEC104 Server");
                //LogMessage($"Configuration saved to: {Path.GetFileName(filePath)}");
                //LogMessage($"Saved {_dataPoints.Count} data points to XML");
            }
            catch (Exception ex)
            {
                LogMessage($"Error saving configuration: {ex.Message}");
                MessageBox.Show($"Error saving configuration: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Save As XML (export with dialog)
        /// </summary>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dialog = new SaveFileDialog())
                {
                    dialog.Title = "Save IEC104 Configuration";
                    dialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                    dialog.DefaultExt = "xml";
                    dialog.FileName = $"IEC104_Config_{DateTime.Now:yyyyMMdd_HHmmss}.xml";

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        //  Export to XML using XmlConfigService
                        _xmlConfigService.ExportToXml(_dataPoints, dialog.FileName, "IEC104 Server");
                        _currentConfigFile = dialog.FileName; // Remember for next Save
                        LogMessage($"Configuration saved to: {Path.GetFileName(dialog.FileName)}");
                        LogMessage($"Saved {_dataPoints.Count} data points to XML");
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error saving configuration: {ex.Message}");
                MessageBox.Show($"Error saving configuration: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Open XML configuration (import with dialog)
        /// </summary>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dialog = new OpenFileDialog())
                {
                    dialog.Title = "Open IEC104 Configuration";
                    dialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                    dialog.DefaultExt = "xml";

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        // Show confirmation dialog
                        var result = MessageBox.Show(
                            $"Open Configuration?\n\n" +
                            $"File: {System.IO.Path.GetFileName(dialog.FileName)}\n" +
                            $"This will replace current configuration!\n\n" +
                            $"Continue?",
                            "Confirm Open", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            //  Import from XML using XmlConfigService
                            var dataPoints = _xmlConfigService.ImportFromXml(dialog.FileName);
                            if (dataPoints != null)
                            {
                                _dataPoints.Clear();
                                _dataPoints.AddRange(dataPoints);
                                RefreshDataPointsGrid();

                                _currentConfigFile = dialog.FileName; // Remember for next Save
                                LogMessage($"Configuration loaded from: {Path.GetFileName(dialog.FileName)}");
                                LogMessage($"Loaded {_dataPoints.Count} data points from XML");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error opening configuration: {ex.Message}");
                MessageBox.Show($"Error opening configuration: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                        _serverConfig = form.ServerConfiguration; //  Fixed: Use ServerConfiguration
                        LogMessage(" Server configuration updated");
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