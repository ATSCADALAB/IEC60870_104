// File: Forms/MainForm.cs - Simple and Effective
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

        public MainForm()
        {
            InitializeComponent();

            _serverService = new IEC60870ServerService();
            _configManager = new ConfigManager();
            _driverManager = new DriverManager();

            // Setup events
            _driverManager.LogMessage += LogMessage;
            _serverService.OnLogMessage += LogMessage;
            _serverService.OnAsduReceived += HandleReceivedAsdu;

            // Setup timers
            _tagScanTimer = new Timer { Interval = 1000 };
            _tagScanTimer.Tick += (s, e) => UpdateTagValues();

            _dataSendTimer = new Timer { Interval = 5000 };
            _dataSendTimer.Tick += (s, e) => SendAllValidData();
        }

        /// <summary>
        /// Set ATSCADA driver - đơn giản
        /// </summary>
        public void SetDriver(iDriver driver, string defaultTaskName = "")
        {
            _driverManager.Initialize(driver, defaultTaskName);
            LogMessage($"Driver set with default task: {defaultTaskName}");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Load config và data
            _serverConfig = _configManager.LoadServerConfig();
            _dataPoints = _configManager.LoadDataPoints();

            // Setup data binding
            _dataPointsBindingSource = new BindingSource 
            { 
                DataSource = new BindingList<DataPoint>(_dataPoints) 
            };
            dgvDataPoints.DataSource = _dataPointsBindingSource;

            // Configure grid
            SetupDataGrid();
            UpdateServerStatusUI();
            
            // Start tag scanning
            _tagScanTimer.Start();
            LogMessage("Application loaded. Tag scanning started.");
        }

        private void SetupDataGrid()
        {
            dgvDataPoints.Columns["IOA"].Width = 60;
            dgvDataPoints.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvDataPoints.Columns["Type"].Width = 70;
            dgvDataPoints.Columns["Value"].Width = 80;
            dgvDataPoints.Columns["DataTagName"].Width = 150;
            dgvDataPoints.Columns["DataTagName"].HeaderText = "Tag Path";

            // Hide unnecessary columns
            var hideColumns = new[] { "Description", "ConvertedValue", "IsValid", "LastUpdated" };
            foreach (var col in hideColumns)
            {
                if (dgvDataPoints.Columns[col] != null)
                    dgvDataPoints.Columns[col].Visible = false;
            }
        }

        #region Timer Methods

        private void UpdateTagValues()
        {
            if (!_driverManager.IsInitialized) return;

            bool hasChanges = false;
            foreach (var dataPoint in _dataPoints)
            {
                if (string.IsNullOrEmpty(dataPoint.DataTagName)) continue;

                try
                {
                    var newValue = _driverManager.GetTagValue(dataPoint.DataTagName);
                    var isGood = _driverManager.IsTagGood(dataPoint.DataTagName);

                    if (dataPoint.Value != newValue || dataPoint.IsValid != isGood)
                    {
                        dataPoint.Value = newValue ?? "";
                        dataPoint.IsValid = isGood;
                        dataPoint.LastUpdated = DateTime.Now;
                        hasChanges = true;
                    }
                }
                catch (Exception ex)
                {
                    LogMessage($"Error reading {dataPoint.DataTagName}: {ex.Message}");
                }
            }

            if (hasChanges)
                _dataPointsBindingSource.ResetBindings(false);
        }

        private void SendAllValidData()
        {
            if (!_serverService.IsRunning) return;

            var validPoints = _dataPoints.Where(dp => 
                !string.IsNullOrEmpty(dp.Value) && 
                dp.IsValid && 
                !string.IsNullOrEmpty(dp.DataTagName)
            ).ToList();

            foreach (var point in validPoints)
            {
                var asdu = CreateAsduFromDataPoint(point);
                if (asdu != null)
                    _serverService.BroadcastAsdu(asdu);
            }

            if (validPoints.Count > 0)
                LogMessage($"Auto-sent {validPoints.Count} data points");
        }

        #endregion

        #region Event Handlers

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                _serverService.Start(_serverConfig);
                UpdateServerStatusUI();
                
                if (_serverService.IsRunning)
                {
                    _dataSendTimer.Start();
                    LogMessage("Server started. Auto-send enabled.");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Start error: {ex.Message}");
                MessageBox.Show($"Failed to start: {ex.Message}", "Error");
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _dataSendTimer.Stop();
            _serverService.Stop();
            UpdateServerStatusUI();
            LogMessage("Server stopped.");
        }

        private void btnAddPoint_Click(object sender, EventArgs e)
        {
            using (var form = new DataPointForm())
            {
                if (form.ShowDialog() != DialogResult.OK) return;

                // Check duplicate IOA
                if (_dataPoints.Any(dp => dp.IOA == form.DataPoint.IOA))
                {
                    MessageBox.Show($"IOA {form.DataPoint.IOA} already exists!");
                    return;
                }

                _dataPointsBindingSource.Add(form.DataPoint);
                _configManager.SaveDataPoints(_dataPoints);
                LogMessage($"Added: IOA {form.DataPoint.IOA}, Tag {form.DataPoint.DataTagName}");
            }
        }

        private void btnEditPoint_Click(object sender, EventArgs e)
        {
            if (dgvDataPoints.CurrentRow?.DataBoundItem is DataPoint selected)
            {
                using (var form = new DataPointForm(selected))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        _dataPointsBindingSource.ResetBindings(false);
                        _configManager.SaveDataPoints(_dataPoints);
                        LogMessage($"Edited: IOA {selected.IOA}");
                    }
                }
            }
        }

        private void btnDeletePoint_Click(object sender, EventArgs e)
        {
            if (dgvDataPoints.CurrentRow?.DataBoundItem is DataPoint selected)
            {
                if (MessageBox.Show($"Delete {selected.Name}?", "Confirm", 
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _dataPointsBindingSource.Remove(selected);
                    _configManager.SaveDataPoints(_dataPoints);
                    LogMessage($"Deleted: IOA {selected.IOA}");
                }
            }
        }

        private void btnSendSelected_Click(object sender, EventArgs e)
        {
            if (!_serverService.IsRunning)
            {
                MessageBox.Show("Server not running!");
                return;
            }

            var selected = dgvDataPoints.SelectedRows
                .Cast<DataGridViewRow>()
                .Select(r => r.DataBoundItem as DataPoint)
                .Where(dp => dp != null && !string.IsNullOrEmpty(dp.Value))
                .ToList();

            foreach (var point in selected)
            {
                var asdu = CreateAsduFromDataPoint(point);
                if (asdu != null)
                {
                    _serverService.BroadcastAsdu(asdu);
                    LogMessage($"Sent: IOA {point.IOA} = {point.Value}");
                }
            }
        }

        private void configureServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_serverService.IsRunning)
            {
                MessageBox.Show("Stop server first!");
                return;
            }

            using (var form = new ServerConfigForm(_serverConfig))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    _serverConfig = form.ServerConfiguration;
                    _configManager.SaveServerConfig(_serverConfig);
                    LogMessage("Configuration updated.");
                }
            }
        }

        #endregion

        #region Helper Methods

        private ASdu CreateAsduFromDataPoint(DataPoint point)
        {
            try
            {
                var converted = point.ConvertedValue;
                if (converted == null) return null;

                InformationObject infoObj;
                var commonAddr = _serverConfig.CommonAddress;

                switch (point.Type)
                {
                    case DataType.Bool:
                        var sp = new IeSinglePointWithQuality((bool)converted, false, false, false, false);
                        infoObj = new InformationObject(point.IOA, new[] { new InformationElement[] { sp } });
                        return new ASdu(TypeId.M_SP_NA_1, false, CauseOfTransmission.SPONTANEOUS, 
                            false, false, 0, commonAddr, new[] { infoObj });

                    case DataType.Float:
                        var sf = new IeShortFloat((float)converted);
                        var quality = new IeQuality(false, false, false, false, false);
                        infoObj = new InformationObject(point.IOA, new[] { new InformationElement[] { sf, quality } });
                        return new ASdu(TypeId.M_ME_NC_1, false, CauseOfTransmission.SPONTANEOUS, 
                            false, false, 0, commonAddr, new[] { infoObj });

                    case DataType.Int:
                        var sv = new IeScaledValue((short)(int)converted);
                        var qds = new IeQuality(false, false, false, false, false);
                        infoObj = new InformationObject(point.IOA, new[] { new InformationElement[] { sv, qds } });
                        return new ASdu(TypeId.M_ME_NB_1, false, CauseOfTransmission.SPONTANEOUS, 
                            false, false, 0, commonAddr, new[] { infoObj });

                    case DataType.Counter:
                        var bcr = new IeBinaryCounterReading((int)converted, 0, false, false, false);
                        infoObj = new InformationObject(point.IOA, new[] { new InformationElement[] { bcr } });
                        return new ASdu(TypeId.M_IT_NA_1, false, CauseOfTransmission.SPONTANEOUS, 
                            false, false, 0, commonAddr, new[] { infoObj });

                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                LogMessage($"ASDU creation error for IOA {point.IOA}: {ex.Message}");
                return null;
            }
        }

        private void HandleReceivedAsdu(ASdu asdu)
        {
            if (asdu.GetTypeIdentification() == TypeId.C_IC_NA_1)
            {
                LogMessage("General Interrogation received. Sending all data...");
                
                foreach (var dp in _dataPoints.Where(dp => !string.IsNullOrEmpty(dp.Value)))
                {
                    var responseAsdu = CreateAsduFromDataPoint(dp);
                    if (responseAsdu != null)
                    {
                        // Change COT to INTERROGATED_BY_STATION
                        var interrogationAsdu = new ASdu(
                            responseAsdu.GetTypeIdentification(),
                            responseAsdu.IsSequenceOfElements,
                            CauseOfTransmission.INTERROGATED_BY_STATION,
                            responseAsdu.IsTestFrame(),
                            responseAsdu.IsNegativeConfirm(),
                            responseAsdu.GetOriginatorAddress(),
                            responseAsdu.GetCommonAddress(),
                            responseAsdu.GetInformationObjects()
                        );
                        _serverService.BroadcastAsdu(interrogationAsdu);
                    }
                }
                LogMessage("Interrogation response completed.");
            }
        }

        private void UpdateServerStatusUI()
        {
            bool running = _serverService.IsRunning;
            lblServerStatus.Text = running ? "Status: Running" : "Status: Stopped";
            lblServerStatus.ForeColor = running ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            btnStart.Enabled = !running;
            btnStop.Enabled = running;
        }

        private void LogMessage(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(LogMessage), message);
                return;
            }

            if (txtLogs.Text.Length > 10000)
                txtLogs.Text = txtLogs.Text.Substring(5000);

            txtLogs.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
        }

        #endregion

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _tagScanTimer?.Stop();
            _dataSendTimer?.Stop();
            
            if (_serverService.IsRunning)
                _serverService.Stop();
                
            _configManager.SaveDataPoints(_dataPoints);
        }

        // Debug method
        public void TestDriverConnection()
        {
            LogMessage("=== DRIVER TEST ===");
            LogMessage($"Driver initialized: {_driverManager.IsInitialized}");
            LogMessage($"Default task: {_driverManager.DefaultTaskName}");
            
            if (_dataPoints.Count > 0)
            {
                var testPoint = _dataPoints.First();
                if (!string.IsNullOrEmpty(testPoint.DataTagName))
                {
                    var testValue = _driverManager.GetTagValue(testPoint.DataTagName);
                    var isGood = _driverManager.IsTagGood(testPoint.DataTagName);
                    LogMessage($"Test tag {testPoint.DataTagName}: Value={testValue}, Good={isGood}");
                }
            }
            LogMessage("=== END TEST ===");
        }
    }
}