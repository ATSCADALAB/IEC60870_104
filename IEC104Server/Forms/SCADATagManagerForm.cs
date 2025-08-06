// File: Forms/SCADATagManagerForm.cs - Quản lý SCADA Tags
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using IEC104Server.Models;
using IEC60870ServerWinForm.Models;
using IEC60870ServerWinForm.Services;

namespace IEC60870ServerWinForm.Forms
{
    /// <summary>
    /// Form quản lý SCADA Tags - Test, Monitor, Debug
    /// </summary>
    public partial class SCADATagManagerForm : Form
    {
        private readonly DriverManager _driverManager;
        private readonly List<DataPoint> _dataPoints;
        private Timer _refreshTimer;
        private bool _autoRefresh = false;

        public SCADATagManagerForm(DriverManager driverManager, List<DataPoint> dataPoints)
        {
            _driverManager = driverManager;
            _dataPoints = dataPoints;

            InitializeComponent();
            InitializeTimer();
            LoadTagData();
        }
        private void InitializeTimer()
        {
            _refreshTimer = new Timer
            {
                Interval = 2000 // 2 seconds
            };
            _refreshTimer.Tick += RefreshTimer_Tick;
        }

        private void LoadTagData()
        {
            try
            {
                var tagData = _dataPoints
                    .Where(dp => !string.IsNullOrEmpty(dp.DataTagName))
                    .Select(dp => new
                    {
                        IOA = dp.IOA,
                        Name = dp.Name,
                        TagPath = dp.DataTagName,
                        DataType = dp.GetDataTypeDisplayName(),
                        TypeId = dp.GetTypeIdDisplayName(),
                        Value = dp.Value ?? "",
                        ConvertedValue = dp.ConvertedValue?.ToString() ?? "",
                        IsValid = dp.IsValid ? "" : "❌",
                        LastUpdated = dp.LastUpdated.ToString("HH:mm:ss"),
                        Status = GetTagStatus(dp)
                    })
                    .ToList();

                dgvTags.DataSource = tagData;

                // Customize columns
                if (dgvTags.Columns.Count > 0)
                {
                    dgvTags.Columns["IOA"].Width = 60;
                    dgvTags.Columns["Name"].Width = 120;
                    dgvTags.Columns["TagPath"].Width = 150;
                    dgvTags.Columns["DataType"].Width = 80;
                    dgvTags.Columns["TypeId"].Width = 100;
                    dgvTags.Columns["Value"].Width = 100;
                    dgvTags.Columns["ConvertedValue"].Width = 100;
                    dgvTags.Columns["IsValid"].Width = 60;
                    dgvTags.Columns["LastUpdated"].Width = 80;
                    dgvTags.Columns["Status"].Width = 100;
                }

                UpdateStatus($"Loaded {tagData.Count} SCADA tags");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error loading tags: {ex.Message}");
            }
        }

        private string GetTagStatus(DataPoint dp)
        {
            if (string.IsNullOrEmpty(dp.DataTagName))
                return "No Tag";

            if (!_driverManager.IsInitialized)
                return "Driver Not Ready";

            if (dp.IsValid)
                return "Good";
            else
                return "Bad/Unknown";
        }

        private void UpdateStatus(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateStatus(message)));
                return;
            }

            lblStatus.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
        }

        #region Event Handlers

        private void BtnTestAll_Click(object sender, EventArgs e)
        {
            TestAllTags();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadTagData();
        }

        private void BtnAutoRefresh_Click(object sender, EventArgs e)
        {
            _autoRefresh = !_autoRefresh;
            btnAutoRefresh.Text = $"Auto Refresh: {(_autoRefresh ? "ON" : "OFF")}";
            btnAutoRefresh.BackColor = _autoRefresh ? System.Drawing.Color.LightGreen : System.Drawing.SystemColors.Control;

            if (_autoRefresh)
                _refreshTimer.Start();
            else
                _refreshTimer.Stop();

            UpdateStatus($"Auto refresh {(_autoRefresh ? "enabled" : "disabled")}");
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            ExportTagData();
        }

        private void TxtFilter_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            if (_autoRefresh)
            {
                LoadTagData();
            }
        }

        #endregion

        #region Methods

        private void TestAllTags()
        {
            if (!_driverManager.IsInitialized)
            {
                UpdateStatus("SCADA Driver not initialized");
                return;
            }

            UpdateStatus("Testing all tags...");

            var tagsToTest = _dataPoints
                .Where(dp => !string.IsNullOrEmpty(dp.DataTagName))
                .ToList();

            int successCount = 0;
            int errorCount = 0;

            foreach (var dp in tagsToTest)
            {
                try
                {
                    var value = _driverManager.GetTagValue(dp.DataTagName);
                    var isGood = _driverManager.IsTagGood(dp.DataTagName);

                    if (value != null && isGood)
                        successCount++;
                    else
                        errorCount++;
                }
                catch
                {
                    errorCount++;
                }
            }

            UpdateStatus($"Test completed: {successCount} OK, {errorCount} Error");
            LoadTagData(); // Refresh display
        }

        private void ApplyFilter()
        {
            // Simple filter implementation
            var filter = txtFilter.Text.ToLower();
            if (string.IsNullOrEmpty(filter))
            {
                LoadTagData();
                return;
            }

            try
            {
                var filteredData = _dataPoints
                    .Where(dp => !string.IsNullOrEmpty(dp.DataTagName) &&
                                (dp.Name?.ToLower().Contains(filter) == true ||
                                 dp.DataTagName.ToLower().Contains(filter) ||
                                 dp.IOA.ToString().Contains(filter)))
                    .Select(dp => new
                    {
                        IOA = dp.IOA,
                        Name = dp.Name,
                        TagPath = dp.DataTagName,
                        DataType = dp.GetDataTypeDisplayName(),
                        TypeId = dp.GetTypeIdDisplayName(),
                        Value = dp.Value ?? "",
                        ConvertedValue = dp.ConvertedValue?.ToString() ?? "",
                        IsValid = dp.IsValid ? "" : "❌",
                        LastUpdated = dp.LastUpdated.ToString("HH:mm:ss"),
                        Status = GetTagStatus(dp)
                    })
                    .ToList();

                dgvTags.DataSource = filteredData;
                UpdateStatus($"Filter applied: {filteredData.Count} tags shown");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Filter error: {ex.Message}");
            }
        }

        private void ExportTagData()
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt",
                    DefaultExt = "csv",
                    FileName = $"SCADA_Tags_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var lines = new List<string>();
                    lines.Add("IOA,Name,TagPath,DataType,TypeId,Value,ConvertedValue,IsValid,LastUpdated,Status");

                    foreach (var dp in _dataPoints.Where(dp => !string.IsNullOrEmpty(dp.DataTagName)))
                    {
                        var line = $"{dp.IOA},{dp.Name},{dp.DataTagName},{dp.GetDataTypeDisplayName()}," +
                                  $"{dp.GetTypeIdDisplayName()},{dp.Value},{dp.ConvertedValue}," +
                                  $"{dp.IsValid},{dp.LastUpdated:yyyy-MM-dd HH:mm:ss},{GetTagStatus(dp)}";
                        lines.Add(line);
                    }

                    System.IO.File.WriteAllLines(saveDialog.FileName, lines);
                    UpdateStatus($"Exported to {saveDialog.FileName}");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Export error: {ex.Message}");
            }
        }

        /// <summary>
        ///  THÊM MỚI: Double-click để xem chi tiết tag
        /// </summary>
        private void DgvTags_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvTags.Rows[e.RowIndex].DataBoundItem != null)
            {
                try
                {
                    var row = dgvTags.Rows[e.RowIndex];
                    var tagPath = row.Cells["TagPath"].Value?.ToString();

                    if (!string.IsNullOrEmpty(tagPath))
                    {
                        ShowTagDetails(tagPath);
                    }
                }
                catch (Exception ex)
                {
                    UpdateStatus($"Error showing tag details: {ex.Message}");
                }
            }
        }

        /// <summary>
        ///  THÊM MỚI: Hiển thị chi tiết tag trong dialog
        /// </summary>
        private void ShowTagDetails(string tagPath)
        {
            try
            {
                var dataPoint = _dataPoints.FirstOrDefault(dp => dp.DataTagName == tagPath);
                if (dataPoint == null)
                {
                    MessageBox.Show($"Data point not found for tag: {tagPath}", "Tag Details",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var details = $"Tag Details:\n\n" +
                             $"IOA: {dataPoint.IOA}\n" +
                             $"Name: {dataPoint.Name}\n" +
                             $"Description: {dataPoint.Description}\n" +
                             $"Tag Path: {dataPoint.DataTagName}\n" +
                             $"Data Type: {dataPoint.GetDataTypeDisplayName()}\n" +
                             $"Type ID: {dataPoint.GetTypeIdDisplayName()}\n" +
                             $"Current Value: {dataPoint.Value}\n" +
                             $"Converted Value: {dataPoint.ConvertedValue}\n" +
                             $"Is Valid: {dataPoint.IsValid}\n" +
                             $"Last Updated: {dataPoint.LastUpdated:yyyy-MM-dd HH:mm:ss}\n\n";

                if (_driverManager.IsInitialized)
                {
                    var testValue = _driverManager.GetTagValue(tagPath);
                    var isGood = _driverManager.IsTagGood(tagPath);
                    var tagInfo = _driverManager.GetTagInfo(tagPath);

                    details += $"SCADA Connection Test:\n" +
                              $"Test Value: {testValue}\n" +
                              $"Status: {(isGood ? "Good" : "Bad")}\n" +
                              $"Driver Info: {tagInfo}";
                }
                else
                {
                    details += "SCADA Driver: Not initialized";
                }

                MessageBox.Show(details, $"Tag Details - {tagPath}",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting tag details: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///  THÊM MỚI: Context menu cho DataGridView
        /// </summary>
        private void SetupContextMenu()
        {
            var contextMenu = new ContextMenuStrip();

            var menuItemDetails = new ToolStripMenuItem("View Details");
            menuItemDetails.Click += (s, e) => {
                if (dgvTags.SelectedRows.Count > 0)
                {
                    var tagPath = dgvTags.SelectedRows[0].Cells["TagPath"].Value?.ToString();
                    if (!string.IsNullOrEmpty(tagPath))
                        ShowTagDetails(tagPath);
                }
            };

            var menuItemTest = new ToolStripMenuItem("Test Connection");
            menuItemTest.Click += (s, e) => {
                if (dgvTags.SelectedRows.Count > 0)
                {
                    var tagPath = dgvTags.SelectedRows[0].Cells["TagPath"].Value?.ToString();
                    if (!string.IsNullOrEmpty(tagPath))
                        TestSingleTag(tagPath);
                }
            };

            var menuItemRefresh = new ToolStripMenuItem("Refresh This Tag");
            menuItemRefresh.Click += (s, e) => {
                if (dgvTags.SelectedRows.Count > 0)
                {
                    var tagPath = dgvTags.SelectedRows[0].Cells["TagPath"].Value?.ToString();
                    if (!string.IsNullOrEmpty(tagPath))
                        RefreshSingleTag(tagPath);
                }
            };

            contextMenu.Items.AddRange(new ToolStripItem[] {
                menuItemDetails, menuItemTest, menuItemRefresh
            });

            dgvTags.ContextMenuStrip = contextMenu;
        }

        /// <summary>
        ///  THÊM MỚI: Test single tag
        /// </summary>
        private void TestSingleTag(string tagPath)
        {
            try
            {
                if (!_driverManager.IsInitialized)
                {
                    UpdateStatus("SCADA Driver not initialized");
                    return;
                }

                var value = _driverManager.GetTagValue(tagPath);
                var isGood = _driverManager.IsTagGood(tagPath);

                var result = $"Tag Test Result:\n" +
                            $"Tag: {tagPath}\n" +
                            $"Value: {value}\n" +
                            $"Status: {(isGood ? "Good" : "Bad")}";

                MessageBox.Show(result, "Tag Test", MessageBoxButtons.OK,
                    isGood ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

                UpdateStatus($"Tested tag: {tagPath} = {value} ({(isGood ? "Good" : "Bad")})");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error testing tag: {ex.Message}", "Test Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///  THÊM MỚI: Refresh single tag
        /// </summary>
        private void RefreshSingleTag(string tagPath)
        {
            try
            {
                var dataPoint = _dataPoints.FirstOrDefault(dp => dp.DataTagName == tagPath);
                if (dataPoint != null && _driverManager.IsInitialized)
                {
                    var value = _driverManager.GetTagValue(tagPath);
                    var isGood = _driverManager.IsTagGood(tagPath);

                    dataPoint.Value = value ?? "null";
                    dataPoint.IsValid = isGood && !string.IsNullOrEmpty(value);
                    dataPoint.LastUpdated = DateTime.Now;

                    if (dataPoint.IsValid)
                    {
                        dataPoint.ConvertedValue = dataPoint.ConvertValueByDataType(value);
                    }

                    LoadTagData(); // Refresh display
                    UpdateStatus($"Refreshed tag: {tagPath}");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error refreshing tag: {ex.Message}");
            }
        }

        #endregion

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _refreshTimer?.Stop();
            _refreshTimer?.Dispose();
            base.OnFormClosed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //  Setup additional features
            SetupContextMenu();
            dgvTags.CellDoubleClick += DgvTags_CellDoubleClick;

            //  Load initial data
            LoadTagData();
        }
    }
}
