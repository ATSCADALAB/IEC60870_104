# Simple Usage Example - IEC104Server với iDriver1

## Cách sử dụng đơn giản như code của bạn

### 1. **Trong Form chính (như frmLayout của bạn):**

```csharp
using System;
using System.Windows.Forms;
using IEC60870ServerWinForm.Forms;

namespace Demo
{
    public partial class frmLayout : Form
    {
        private MainForm iecServerForm;

        public frmLayout()
        {
            InitializeComponent();
            Load += FrmLayout_Load;
        }

        private void FrmLayout_Load(object sender, EventArgs e)
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

            // ✅ Tạo và setup IEC104 Server
            iecServerForm = new MainForm();
            iecServerForm.SetDriver(iDriver1); // Truyền iDriver1 vào
            
            // ✅ Thêm data points
            AddSampleDataPoints();
            
            // ✅ Hiển thị form
            iecServerForm.Show();
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
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Stop();
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
            }
            catch
            {
                timer2.Start();
            }
            timer2.Start();
        }
    }
}
```

### 2. **IEC104Server sẽ tự động:**

```csharp
// ✅ Đọc giá trị từ iDriver1 như này:
var value = iDriver1.Task("MAFAGSBL1").Tag("Gio").Value;

// ✅ Convert và gửi qua IEC60870 protocol
// ✅ Update UI real-time
// ✅ Log activities
```

## Workflow đơn giản:

### **Step 1: Setup iDriver1**
```csharp
// Trong form chính của bạn
iDriver1.Task("MAFAGSBL1").Tag("Gio").Value = "12";
iDriver1.Task("MAFAGSBL1").Tag("Phut").Value = "30";
// ... các tags khác
```

### **Step 2: Tạo IEC104Server**
```csharp
var iecServer = new MainForm();
iecServer.SetDriver(iDriver1); // Truyền iDriver1 vào
```

### **Step 3: Thêm Data Points**
```csharp
// Format: IOA, Name, DataType, TagPath
iecServer.AddDataPointByDataType(16385, "Gio", DataType.Int, "MAFAGSBL1.Gio");
iecServer.AddDataPointByDataType(16386, "Phut", DataType.Int, "MAFAGSBL1.Phut");
```

### **Step 4: Start Server**
```csharp
iecServer.Show(); // Hiển thị form và có thể start server
```

## Tag Path Format:

```
"MAFAGSBL1.Gio"     → iDriver1.Task("MAFAGSBL1").Tag("Gio").Value
"MAFAGSBL1.Phut"    → iDriver1.Task("MAFAGSBL1").Tag("Phut").Value
"PLC1.Temperature"  → iDriver1.Task("PLC1").Tag("Temperature").Value
"Tank.Level.Main"   → iDriver1.Task("Tank").Tag("Level.Main").Value
```

## Data Types hỗ trợ:

- **DataType.Bool** → IEC60870 Single Point (M_SP_NA_1)
- **DataType.Int** → IEC60870 Scaled Value (M_ME_NB_1)  
- **DataType.Float** → IEC60870 Short Float (M_ME_NC_1)
- **DataType.Counter** → IEC60870 Integrated Totals (M_IT_NA_1)
- **DataType.Double** → IEC60870 Short Float (M_ME_NC_1)
- **DataType.String** → IEC60870 Bitstring (M_ME_NB_1)

## Log Messages:

```
✅ iDriver1 set successfully!
✅ iDriver1 found and ready!
🔄 Starting tag scanning...
📈 SCADA Scan: 7 Good, 0 Error, 7 Total
🚀 IEC104 Server started successfully
```

## Lợi ích:

✅ **Đơn giản**: Chỉ cần `SetDriver(iDriver1)`  
✅ **Tự động**: Đọc và gửi data real-time  
✅ **Tương thích**: Sử dụng iDriver1 như code hiện tại  
✅ **Flexible**: Thêm/sửa data points dễ dàng  
✅ **Monitoring**: UI để theo dõi và debug  

---

**Kết quả:** IEC104Server sẽ đọc data từ iDriver1 và gửi cho IEC60870 clients một cách tự động!
