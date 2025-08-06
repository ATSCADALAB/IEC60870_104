# XML Configuration Implementation

##  **XML Format Implemented:**

### **Simple XML Structure:**
```xml
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<IEC104Configuration ProjectName="Sample IEC104 Project" CreatedDate="2025-01-08 10:30:00" Version="1.0" TagCount="5">
  <Tags>
    <Tag IOA="1" Name="Temperature_01" Type="M_ME_NC_1" DataType="Float" DataTagName="PLC1.Temperature" Description="Temperature sensor 1" Enabled="true" />
    <Tag IOA="2" Name="Pressure_01" Type="M_ME_NB_1" DataType="Int" DataTagName="PLC1.Pressure" Description="Pressure sensor 1" Enabled="true" />
    <Tag IOA="3" Name="Pump_Status" Type="M_SP_NA_1" DataType="Bool" DataTagName="PLC1.PumpStatus" Description="Pump running status" Enabled="true" />
    <Tag IOA="4" Name="Flow_Rate" Type="M_ME_NC_1" DataType="Float" DataTagName="PLC1.FlowRate" Description="Water flow rate" Enabled="true" />
    <Tag IOA="5" Name="Tank_Level" Type="M_ME_NB_1" DataType="Int" DataTagName="PLC1.TankLevel" Description="Water tank level" Enabled="true" />
  </Tags>
</IEC104Configuration>
```

### **XML Attributes:**

**Root Element: `<IEC104Configuration>`**
- `ProjectName` - Tên project
- `CreatedDate` - Ngày tạo (yyyy-MM-dd HH:mm:ss)
- `Version` - Phiên bản format
- `TagCount` - Số lượng tags

**Tag Element: `<Tag>`**
- `IOA` - Information Object Address (required)
- `Name` - Tên tag (required)
- `Type` - IEC104 TypeId (M_ME_NC_1, M_ME_NB_1, M_SP_NA_1, etc.)
- `DataType` - Data type (Float, Int, Bool, String)
- `DataTagName` - SCADA tag path (PLC1.Temperature)
- `Description` - Mô tả tag
- `Enabled` - Tag có được enable không
- `LastValue` - Giá trị cuối (optional)
- `LastUpdated` - Thời gian update cuối (optional)

## 🔧 **Implementation Details:**

### **1. XmlConfigService Class:**
```csharp
public class XmlConfigService
{
    //  Export to XML
    public void ExportToXml(List<DataPoint> dataPoints, string filePath, string projectName)
    
    //  Import from XML  
    public List<DataPoint> ImportFromXml(string filePath)
    
    //  Get file info without loading all data
    public XmlFileInfo GetXmlFileInfo(string filePath)
    
    //  Validate XML format
    public bool ValidateXmlFile(string filePath, out string errorMessage)
}
```

### **2. MainForm Integration:**
```csharp
// Service initialization
private readonly XmlConfigService _xmlConfigService;

// Constructor
_xmlConfigService = new XmlConfigService();

// Save As implementation
private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
{
    using (var dialog = new SaveFileDialog())
    {
        dialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            _xmlConfigService.ExportToXml(_dataPoints, dialog.FileName, "IEC104 Server");
        }
    }
}

// Open implementation
private void openToolStripMenuItem_Click(object sender, EventArgs e)
{
    using (var dialog = new OpenFileDialog())
    {
        dialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            var dataPoints = _xmlConfigService.ImportFromXml(dialog.FileName);
            _dataPoints.Clear();
            _dataPoints.AddRange(dataPoints);
            RefreshDataPointsGrid();
        }
    }
}
```

##  **File Menu Functions:**

### ** Save (Ctrl+S):**
```
Function: Quick save to JSON (existing functionality)
File: datapoints.json
Usage: Fast save current configuration
```

### ** Save As (Ctrl+Shift+S):**
```
Function: Export to XML with dialog
File: IEC104_Config_20250108_103000.xml
Usage: Export configuration for sharing/backup
Filter: XML files (*.xml)|*.xml
```

### ** Open (Ctrl+O):**
```
Function: Import from XML with dialog
File: Any .xml file
Usage: Load configuration from XML
Confirmation: Shows file info before loading
```

## 🎯 **Usage Examples:**

### **1. Export Configuration:**
```
1. Configure data points in grid
2. File → Save As (or Ctrl+Shift+S)
3. Choose location and filename
4. Click Save
5. XML file created with all tags

Expected log:
 Configuration saved to: C:\Config\MyProject.xml
 Saved 25 data points to XML
```

### **2. Import Configuration:**
```
1. File → Open (or Ctrl+O)
2. Select XML file
3. Confirm replacement of current config
4. Tags loaded into grid

Expected log:
 Configuration loaded from: C:\Config\MyProject.xml
 Loaded 25 data points from XML
```

### **3. File Info Preview:**
```csharp
var info = _xmlConfigService.GetXmlFileInfo(filePath);

// Results:
info.FileName = "MyProject.xml"
info.ProjectName = "Sample IEC104 Project"
info.CreatedDate = "2025-01-08 10:30:00"
info.Version = "1.0"
info.TagCount = 25
```

## 💡 **XML Advantages:**

### **1. Human Readable:**
```xml
<!-- Clear structure -->
<Tag IOA="1" Name="Temperature_01" DataTagName="PLC1.Temperature" Description="Temperature sensor 1" />
```

### **2. Portable:**
```
 Cross-platform compatible
 Version control friendly
 Easy to edit manually
 Standard format
```

### **3. Extensible:**
```xml
<!-- Can add new attributes without breaking compatibility -->
<Tag IOA="1" Name="Temp01" NewAttribute="Value" />
```

### **4. Validation:**
```csharp
// Built-in validation
if (!_xmlConfigService.ValidateXmlFile(filePath, out string error))
{
    MessageBox.Show($"Invalid XML: {error}");
}
```

## 🔧 **Error Handling:**

### **Export Errors:**
```
❌ Error exporting to XML: Access denied to file
❌ Error exporting to XML: Invalid characters in filename
❌ Error exporting to XML: Disk full
```

### **Import Errors:**
```
❌ Error importing from XML: File not found
❌ Error importing from XML: Invalid XML format
❌ Error importing from XML: Missing required attributes
❌ Error importing from XML: Invalid TypeId value
```

### **Validation Errors:**
```
❌ Invalid XML format. Expected root element 'IEC104Configuration'
❌ No 'Tags' element found in XML
❌ Missing required attribute 'IOA' in Tag element
```

##  **Testing:**

### **Test Export:**
```
1. Add some data points
2. File → Save As → test.xml
3. Check XML file content
4. Verify all attributes present
```

### **Test Import:**
```
1. Create/edit XML file manually
2. File → Open → select XML
3. Verify data points loaded correctly
4. Check IOA, Name, Type, DataTagName
```

### **Test Validation:**
```
1. Create invalid XML file
2. Try to open
3. Should show clear error message
4. Application should not crash
```

## 📝 **Sample XML Files:**

### **Minimal Configuration:**
```xml
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<IEC104Configuration ProjectName="Minimal" CreatedDate="2025-01-08 10:30:00" Version="1.0" TagCount="1">
  <Tags>
    <Tag IOA="1" Name="Test" Type="M_ME_NC_1" DataType="Float" DataTagName="PLC1.Test" Description="" Enabled="true" />
  </Tags>
</IEC104Configuration>
```

### **Complex Configuration:**
```xml
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<IEC104Configuration ProjectName="Water Treatment Plant" CreatedDate="2025-01-08 10:30:00" Version="1.0" TagCount="10">
  <Tags>
    <!-- Analog Inputs -->
    <Tag IOA="1" Name="Tank1_Level" Type="M_ME_NC_1" DataType="Float" DataTagName="PLC1.Tank1.Level" Description="Tank 1 water level %" Enabled="true" />
    <Tag IOA="2" Name="Flow_Rate" Type="M_ME_NC_1" DataType="Float" DataTagName="PLC1.Flow.Rate" Description="Water flow rate L/min" Enabled="true" />
    
    <!-- Digital Inputs -->
    <Tag IOA="10" Name="Pump1_Status" Type="M_SP_NA_1" DataType="Bool" DataTagName="PLC1.Pump1.Status" Description="Pump 1 running status" Enabled="true" />
    <Tag IOA="11" Name="Valve1_Status" Type="M_SP_NA_1" DataType="Bool" DataTagName="PLC1.Valve1.Status" Description="Valve 1 open status" Enabled="true" />
  </Tags>
</IEC104Configuration>
```

---

**Status:** XML Configuration Save/Save As/Open fully implemented and tested! 🎉
