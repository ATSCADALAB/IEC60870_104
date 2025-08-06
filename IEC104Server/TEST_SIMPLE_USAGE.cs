// Test file để verify việc sử dụng đơn giản với iDriver1
using System;
using System.Windows.Forms;
using IEC60870ServerWinForm.Forms;
using IEC60870ServerWinForm.Models;

namespace IEC104Server
{
    /// <summary>
    /// ✅ Test class để verify việc sử dụng đơn giản với iDriver1
    /// </summary>
    public partial class TestSimpleUsage : Form
    {
        private MainForm iecServerForm;

        public TestSimpleUsage()
        {
            InitializeComponent();
            Load += TestSimpleUsage_Load;
        }

        private void TestSimpleUsage_Load(object sender, EventArgs e)
        {
            try
            {
                // ✅ Giả lập setup SCADA data như code của bạn
                SetupSCADAData();

                // ✅ Tạo và setup IEC104 Server
                iecServerForm = new MainForm();
                
                // ✅ Set iDriver1 - ĐƠN GIẢN!
                iecServerForm.SetDriver(iDriver1);
                
                // ✅ Thêm data points
                AddSampleDataPoints();
                
                // ✅ Hiển thị form
                iecServerForm.Show();
                
                LogMessage("✅ Test setup completed successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in test setup: {ex.Message}", "Test Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupSCADAData()
        {
            // ✅ Setup SCADA data như code của bạn
            DateTime dateTime = DateTime.Now;
            dateTime = dateTime.AddSeconds(2);
            
            iDriver1.Task("MAFAGSBL1").Tag("Gio").Value = dateTime.ToString("HH");
            iDriver1.Task("MAFAGSBL1").Tag("Phut").Value = dateTime.ToString("mm");
            iDriver1.Task("MAFAGSBL1").Tag("Giay").Value = dateTime.ToString("ss");
            iDriver1.Task("MAFAGSBL1").Tag("Ngay").Value = dateTime.ToString("dd");
            iDriver1.Task("MAFAGSBL1").Tag("Thang").Value = dateTime.ToString("MM");
            iDriver1.Task("MAFAGSBL1").Tag("Nam").Value = dateTime.ToString("yy");
            iDriver1.Task("MAFAGSBL1").Tag("XacNhanDoiGio").Value = "100";
            
            LogMessage("✅ SCADA data setup completed");
        }

        private void AddSampleDataPoints()
        {
            // ✅ Thêm data points tương ứng với tags của bạn
            iecServerForm.AddDataPointByDataType(16385, "Gio", DataType.Int, "MAFAGSBL1.Gio");
            iecServerForm.AddDataPointByDataType(16386, "Phut", DataType.Int, "MAFAGSBL1.Phut");
            iecServerForm.AddDataPointByDataType(16387, "Giay", DataType.Int, "MAFAGSBL1.Giay");
            iecServerForm.AddDataPointByDataType(16388, "Ngay", DataType.Int, "MAFAGSBL1.Ngay");
            iecServerForm.AddDataPointByDataType(16389, "Thang", DataType.Int, "MAFAGSBL1.Thang");
            iecServerForm.AddDataPointByDataType(16390, "Nam", DataType.Int, "MAFAGSBL1.Nam");
            iecServerForm.AddDataPointByDataType(16391, "XacNhanDoiGio", DataType.Int, "MAFAGSBL1.XacNhanDoiGio");
            
            LogMessage("✅ Data points added successfully");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            try
            {
                // ✅ Update SCADA data như code của bạn
                DateTime dateTime = DateTime.Now;
                dateTime = dateTime.AddSeconds(2);
                
                iDriver1.Task("MAFAGSBL1").Tag("Gio").Value = dateTime.ToString("HH");
                iDriver1.Task("MAFAGSBL1").Tag("Phut").Value = dateTime.ToString("mm");
                iDriver1.Task("MAFAGSBL1").Tag("Giay").Value = dateTime.ToString("ss");
                iDriver1.Task("MAFAGSBL1").Tag("Ngay").Value = dateTime.ToString("dd");
                iDriver1.Task("MAFAGSBL1").Tag("Thang").Value = dateTime.ToString("MM");
                iDriver1.Task("MAFAGSBL1").Tag("Nam").Value = dateTime.ToString("yy");
                iDriver1.Task("MAFAGSBL1").Tag("XacNhanDoiGio").Value = "100";
                
                // ✅ IEC104Server sẽ tự động đọc và gửi data
                LogMessage($"🔄 SCADA data updated at {DateTime.Now:HH:mm:ss}");
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error updating SCADA data: {ex.Message}");
                timer1.Start();
            }
            timer1.Start();
        }

        private void LogMessage(string message)
        {
            // Simple logging to console or debug output
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
        }

        // ✅ Test methods để verify functionality
        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                if (iDriver1 == null)
                {
                    LogMessage("❌ iDriver1 is null");
                    return;
                }

                // Test đọc giá trị
                var gio = iDriver1.Task("MAFAGSBL1").Tag("Gio").Value;
                var phut = iDriver1.Task("MAFAGSBL1").Tag("Phut").Value;
                
                LogMessage($"✅ Test read: Gio={gio}, Phut={phut}");
                
                MessageBox.Show($"Connection test successful!\nGio: {gio}\nPhut: {phut}", 
                    "Test Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Test failed: {ex.Message}");
                MessageBox.Show($"Test failed: {ex.Message}", "Test Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            try
            {
                if (iecServerForm != null)
                {
                    // Trigger start server programmatically
                    LogMessage("🚀 Starting IEC104 Server...");
                    // iecServerForm.StartServer(); // Nếu có public method
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error starting server: {ex.Message}");
            }
        }
    }
}

/*
✅ Expected workflow:

1. TestSimpleUsage_Load:
   - Setup SCADA data với iDriver1
   - Tạo MainForm
   - SetDriver(iDriver1) 
   - Add data points
   - Show form

2. Timer updates:
   - Update SCADA data
   - IEC104Server tự động đọc và gửi

3. Expected logs:
   ✅ SCADA data setup completed
   ✅ iDriver1 set successfully!
   ✅ Data points added successfully
   ✅ SCADA Test OK: MAFAGSBL1.Gio = 12
   ✅ SCADA Test OK: MAFAGSBL1.Phut = 30
   🔄 Starting tag scanning...
   📈 SCADA Scan: 7 Good, 0 Error, 7 Total
   🚀 IEC104 Server started successfully

4. No more errors:
   ❌ Driver chưa được khởi tạo! <- SHOULD NOT APPEAR
*/
