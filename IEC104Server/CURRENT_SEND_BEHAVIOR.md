# Current Send Behavior Analysis

##  **Hiện tại Server đang hoạt động như thế nào:**

### **1. Timer Configuration:**

```csharp
// Setup timers với interval hợp lý
_tagScanTimer = new Timer { Interval = 1000 }; // Scan mỗi 1 giây
_tagScanTimer.Tick += (s, e) => UpdateTagValues();

_dataSendTimer = new Timer { Interval = 3000 }; // Gửi data mỗi 3 giây
_dataSendTimer.Tick += (s, e) => SendAllValidData();
```

### **2. Timer Lifecycle:**

**Khi MainForm_Load:**
```csharp
_tagScanTimer.Start(); // Bắt đầu scan tags ngay
```

**Khi StartServer:**
```csharp
_serverService.Start(_serverConfig);
_dataSendTimer.Start(); // Bắt đầu gửi data
_tagScanTimer.Start();  // Đảm bảo scan timer chạy
```

**Khi StopServer:**
```csharp
_serverService.Stop();
_dataSendTimer.Stop();  // Dừng gửi data
_tagScanTimer.Stop();   // Dừng scan tags
```

## 🔄 **Workflow hiện tại:**

### **Step 1: Tag Scanning (Mỗi 1 giây)**
```csharp
UpdateTagValues() // Chạy mỗi 1000ms
├── Đọc từ iDriver1.Task("MAFAGSBL1").Tag("Gio").Value
├── Đọc từ iDriver1.Task("MAFAGSBL1").Tag("Phut").Value
├── Đọc từ iDriver1.Task("MAFAGSBL1").Tag("Giay").Value
├── ... (tất cả tags)
├── Update DataPoint.Value nếu có thay đổi
├── Update DataPoint.ConvertedValue
└── Log: "📈 SCADA Scan: X Good, Y Error, Z Total"
```

### **Step 2: Data Sending (Mỗi 3 giây)**
```csharp
SendAllValidData() // Chạy mỗi 3000ms
├── Lọc validPoints = _dataPoints.Where(p => p.IsValid && !string.IsNullOrEmpty(p.Value))
├── foreach (var point in validPoints)
│   ├── ConvertToASdu(point) // Convert thành IEC60870 format
│   └── _serverService.BroadcastAsdu(asdu) // Gửi từng point
└── Log: "📤 Sent X data points to IEC104 clients"
```

## 📤 **Cách gửi data:**

### **Gửi TẤT CẢ tags cùng lúc:**
```csharp
//  HIỆN TẠI: Gửi tất cả valid data points
foreach (var point in validPoints)
{
    var asdu = ConvertToASdu(point);
    if (asdu != null)
    {
        _serverService.BroadcastAsdu(asdu); // Gửi từng ASDU riêng biệt
    }
}
```

### **Không phải gửi từng tag:**
- Server gửi **TẤT CẢ valid data points** mỗi 3 giây
- Mỗi data point được convert thành 1 ASDU riêng biệt
- Tất cả ASDUs được gửi trong cùng 1 chu kỳ

## ⏱️ **Timing Summary:**

| Timer | Interval | Function | Purpose |
|-------|----------|----------|---------|
| **Tag Scan** | **1000ms (1 giây)** | UpdateTagValues() | Đọc data từ SCADA |
| **Data Send** | **3000ms (3 giây)** | SendAllValidData() | Gửi data qua IEC104 |

## 🎯 **Triggers gửi data:**

### **1. Automatic (Timer-based):**
```csharp
_dataSendTimer.Tick += (s, e) => SendAllValidData(); // Mỗi 3 giây
```

### **2. Manual (On-demand):**
```csharp
// Interrogation command từ client
case TypeId.C_IC_NA_1:
    SendAllValidData(); // Gửi tất cả data ngay lập tức

// Force send từ UI
public void ForceSendData()
{
    SendAllValidData();
}
```

### **3. Send Selected (User action):**
```csharp
btnSendSelected_Click() // Gửi 1 data point cụ thể
├── var asdu = ConvertToASdu(selectedPoint);
└── _serverService.SendASdu(asdu);
```

##  **Data Flow:**

```
iDriver1 Tags (Real-time)
    ↓ (1 giây)
UpdateTagValues() → DataPoint.Value updated
    ↓ (3 giây)  
SendAllValidData() → Convert to ASDUs → Broadcast to IEC104 clients
```

## 🔧 **Configuration Options:**

### **Thay đổi timer intervals:**
```csharp
// Scan nhanh hơn (500ms)
_tagScanTimer.Interval = 500;

// Gửi chậm hơn (5 giây)
_dataSendTimer.Interval = 5000;

// Gửi nhanh hơn (1 giây)
_dataSendTimer.Interval = 1000;
```

### **Disable automatic sending:**
```csharp
_dataSendTimer.Stop(); // Chỉ gửi khi có Interrogation hoặc manual
```

### **Send only on change:**
```csharp
// Có thể modify SendAllValidData() để chỉ gửi khi có thay đổi
var changedPoints = _dataPoints.Where(p => p.HasChanged).ToList();
```

## 📈 **Performance Characteristics:**

### **Current Load:**
- **Tag Scanning**: 1 Hz (1 lần/giây)
- **Data Transmission**: 0.33 Hz (1 lần/3 giây)
- **Network Traffic**: Tất cả data points mỗi 3 giây

### **Scalability:**
- **4 data points**: ~1.33 ASDUs/giây
- **100 data points**: ~33.33 ASDUs/giây  
- **1000 data points**: ~333.33 ASDUs/giây

## 💡 **Recommendations:**

### **For Real-time Applications:**
```csharp
_dataSendTimer.Interval = 1000; // Gửi mỗi 1 giây
```

### **For Bandwidth Conservation:**
```csharp
_dataSendTimer.Interval = 5000; // Gửi mỗi 5 giây
// Hoặc implement "send only on change"
```

### **For High-frequency Data:**
```csharp
_tagScanTimer.Interval = 500;   // Scan mỗi 500ms
_dataSendTimer.Interval = 1000; // Gửi mỗi 1 giây
```

---

**Kết luận:** Server hiện tại gửi **TẤT CẢ valid data points** mỗi **3 giây**, với tag scanning mỗi **1 giây**.
