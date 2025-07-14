using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using IEC60870.Enum;

namespace IEC60870Driver
{
    public partial class ctlTagDesign : UserControl
    {
        private readonly ATDriver driver;
        private Dictionary<TypeId, string> typeDescriptions;
        private Dictionary<TypeId, DataType[]> typeSupportedDataTypes;
        private Dictionary<string, TypeId[]> categoryTypes;

        #region PROPERTIES

        public string TagName
        {
            get => txtTagName.Text.Trim();
            set => txtTagName.Text = value.Trim();
        }

        public string TagAddress
        {
            get => GenerateTagAddress();
            set => ParseTagAddress(value);
        }

        public DataType TagType
        {
            get
            {
                if (cbxDataType.SelectedItem is DataType dataType)
                    return dataType;
                return DataType.Default;
            }
            set => cbxDataType.SelectedItem = value;
        }

        public string Description
        {
            get => txtDescription.Text.Trim();
            set => txtDescription.Text = value;
        }

        public bool IsValid { get; private set; }

        #endregion

        public ctlTagDesign(ATDriver driver)
        {
            InitializeComponent();
            this.driver = driver;

            InitializeTypeDescriptions();
            InitializeControls();

            Load += (sender, e) => Init();
            KeyPress += (sender, e) => CheckKey(e.KeyChar);

            btnOk.Click += (sender, e) => UpdateTag();
            btnCheck.Click += (sender, e) => ValidateTag();

            // Add validation events
            txtTagName.TextChanged += ValidateInputs;
            cbxCategory.SelectedIndexChanged += OnCategoryChanged;
            cbxTypeId.SelectedIndexChanged += OnTypeIdChanged;
            nudIOA.ValueChanged += ValidateInputs;
            cbxDataType.SelectedIndexChanged += ValidateInputs;
        }

        private void InitializeTypeDescriptions()
        {
            typeDescriptions = new Dictionary<TypeId, string>
            {
                // Monitor types
                { TypeId.M_SP_NA_1, "Single-point information without time tag" },
                { TypeId.M_DP_NA_1, "Double-point information without time tag" },
                { TypeId.M_ST_NA_1, "Step position information" },
                { TypeId.M_BO_NA_1, "Bitstring of 32 bit" },
                { TypeId.M_ME_NA_1, "Measured value, normalized value" },
                { TypeId.M_ME_NB_1, "Measured value, scaled value" },
                { TypeId.M_ME_NC_1, "Measured value, short floating point" },
                { TypeId.M_IT_NA_1, "Integrated totals" },
                { TypeId.M_ME_ND_1, "Measured value, normalized value without quality" },
                
                // Monitor types with time tag CP56Time2a
                { TypeId.M_SP_TB_1, "Single-point information with time tag CP56Time2a" },
                { TypeId.M_DP_TB_1, "Double-point information with time tag CP56Time2a" },
                { TypeId.M_ST_TB_1, "Step position information with time tag CP56Time2a" },
                { TypeId.M_BO_TB_1, "Bitstring of 32 bits with time tag CP56Time2a" },
                { TypeId.M_ME_TD_1, "Measured value, normalized value with time tag CP56Time2a" },
                { TypeId.M_ME_TE_1, "Measured value, scaled value with time tag CP56Time2a" },
                { TypeId.M_ME_TF_1, "Measured value, short floating point with time tag CP56Time2a" },
                { TypeId.M_IT_TB_1, "Integrated totals with time tag CP56Time2a" },
                
                // Control types
                { TypeId.C_SC_NA_1, "Single command" },
                { TypeId.C_DC_NA_1, "Double command" },
                { TypeId.C_RC_NA_1, "Regulating step command" },
                { TypeId.C_SE_NA_1, "Set point command, normalized value" },
                { TypeId.C_SE_NB_1, "Set point command, scaled value" },
                { TypeId.C_SE_NC_1, "Set point command, short floating point" },
                { TypeId.C_BO_NA_1, "Bitstring of 32 bits command" },
                
                // Control types with time tag
                { TypeId.C_SC_TA_1, "Single command with time tag CP56Time2a" },
                { TypeId.C_DC_TA_1, "Double command with time tag CP56Time2a" },
                { TypeId.C_RC_TA_1, "Regulating step command with time tag CP56Time2a" },
                { TypeId.C_SE_TA_1, "Set-point command with time tag CP56Time2a, normalized value" },
                { TypeId.C_SE_TB_1, "Set-point command with time tag CP56Time2a, scaled value" },
                { TypeId.C_SE_TC_1, "Set-point command with time tag CP56Time2a, short floating point" },
                { TypeId.C_BO_TA_1, "Bitstring of 32 bit with time tag CP56Time2a" },
                
                // System types
                { TypeId.C_IC_NA_1, "Interrogation command" },
                { TypeId.C_CI_NA_1, "Counter interrogation command" },
                { TypeId.C_RD_NA_1, "Read command" },
                { TypeId.C_CS_NA_1, "Clock synchronization command" },
                { TypeId.C_TS_NA_1, "Test command" },
                { TypeId.C_RP_NA_1, "Reset process command" },
                { TypeId.M_EI_NA_1, "End of initialization" }
            };

            typeSupportedDataTypes = new Dictionary<TypeId, DataType[]>
            {
                // Monitor types
                { TypeId.M_SP_NA_1, new[] { DataType.Bool } },
                { TypeId.M_DP_NA_1, new[] { DataType.Word, DataType.Int } },
                { TypeId.M_ST_NA_1, new[] { DataType.Word, DataType.Int } },
                { TypeId.M_BO_NA_1, new[] { DataType.DWord } },
                { TypeId.M_ME_NA_1, new[] { DataType.Word, DataType.Int, DataType.Float } },
                { TypeId.M_ME_NB_1, new[] { DataType.Word, DataType.Int } },
                { TypeId.M_ME_NC_1, new[] { DataType.Float } },
                { TypeId.M_IT_NA_1, new[] { DataType.DWord } },
                { TypeId.M_ME_ND_1, new[] { DataType.Word, DataType.Int, DataType.Float } },
                
                // Monitor types with time tag
                { TypeId.M_SP_TB_1, new[] { DataType.Bool } },
                { TypeId.M_DP_TB_1, new[] { DataType.Word, DataType.Int } },
                { TypeId.M_ST_TB_1, new[] { DataType.Word, DataType.Int } },
                { TypeId.M_BO_TB_1, new[] { DataType.DWord } },
                { TypeId.M_ME_TD_1, new[] { DataType.Word, DataType.Int, DataType.Float } },
                { TypeId.M_ME_TE_1, new[] { DataType.Word, DataType.Int } },
                { TypeId.M_ME_TF_1, new[] { DataType.Float } },
                { TypeId.M_IT_TB_1, new[] { DataType.DWord } },
                
                // Control types
                { TypeId.C_SC_NA_1, new[] { DataType.Bool } },
                { TypeId.C_DC_NA_1, new[] { DataType.Word, DataType.Int } },
                { TypeId.C_RC_NA_1, new[] { DataType.Word, DataType.Int } },
                { TypeId.C_SE_NA_1, new[] { DataType.Word, DataType.Int, DataType.Float } },
                { TypeId.C_SE_NB_1, new[] { DataType.Word, DataType.Int } },
                { TypeId.C_SE_NC_1, new[] { DataType.Float } },
                { TypeId.C_BO_NA_1, new[] { DataType.DWord } },
                
                // Control types with time tag
                { TypeId.C_SC_TA_1, new[] { DataType.Bool } },
                { TypeId.C_DC_TA_1, new[] { DataType.Word, DataType.Int } },
                { TypeId.C_RC_TA_1, new[] { DataType.Word, DataType.Int } },
                { TypeId.C_SE_TA_1, new[] { DataType.Word, DataType.Int, DataType.Float } },
                { TypeId.C_SE_TB_1, new[] { DataType.Word, DataType.Int } },
                { TypeId.C_SE_TC_1, new[] { DataType.Float } },
                { TypeId.C_BO_TA_1, new[] { DataType.DWord } },
                
                // System types
                { TypeId.C_IC_NA_1, new[] { DataType.Word } },
                { TypeId.C_CI_NA_1, new[] { DataType.Word } },
                { TypeId.C_RD_NA_1, new[] { DataType.String } },
                { TypeId.C_CS_NA_1, new[] { DataType.String } },
                { TypeId.C_TS_NA_1, new[] { DataType.String } },
                { TypeId.C_RP_NA_1, new[] { DataType.Word } },
                { TypeId.M_EI_NA_1, new[] { DataType.Word } }
            };

            categoryTypes = new Dictionary<string, TypeId[]>
            {
                { "Monitor - Basic", new[] {
                    TypeId.M_SP_NA_1, TypeId.M_DP_NA_1, TypeId.M_ST_NA_1, TypeId.M_BO_NA_1,
                    TypeId.M_ME_NA_1, TypeId.M_ME_NB_1, TypeId.M_ME_NC_1, TypeId.M_IT_NA_1, TypeId.M_ME_ND_1
                }},
                { "Monitor - With Time Tag", new[] {
                    TypeId.M_SP_TB_1, TypeId.M_DP_TB_1, TypeId.M_ST_TB_1, TypeId.M_BO_TB_1,
                    TypeId.M_ME_TD_1, TypeId.M_ME_TE_1, TypeId.M_ME_TF_1, TypeId.M_IT_TB_1
                }},
                { "Control - Basic", new[] {
                    TypeId.C_SC_NA_1, TypeId.C_DC_NA_1, TypeId.C_RC_NA_1,
                    TypeId.C_SE_NA_1, TypeId.C_SE_NB_1, TypeId.C_SE_NC_1, TypeId.C_BO_NA_1
                }},
                { "Control - With Time Tag", new[] {
                    TypeId.C_SC_TA_1, TypeId.C_DC_TA_1, TypeId.C_RC_TA_1,
                    TypeId.C_SE_TA_1, TypeId.C_SE_TB_1, TypeId.C_SE_TC_1, TypeId.C_BO_TA_1
                }},
                { "System Commands", new[] {
                    TypeId.C_IC_NA_1, TypeId.C_CI_NA_1, TypeId.C_RD_NA_1,
                    TypeId.C_CS_NA_1, TypeId.C_TS_NA_1, TypeId.C_RP_NA_1, TypeId.M_EI_NA_1
                }}
            };
        }

        private void InitializeControls()
        {
            // Initialize Category ComboBox
            cbxCategory.Items.Clear();
            foreach (var category in categoryTypes.Keys)
            {
                cbxCategory.Items.Add(category);
            }
            cbxCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxCategory.SelectedIndex = 0; // Default to Monitor - Basic

            // Initialize TypeId ComboBox
            cbxTypeId.DropDownStyle = ComboBoxStyle.DropDownList;

            // Initialize IOA range
            nudIOA.Minimum = 0;
            nudIOA.Maximum = 16777215; // 3 bytes max
            nudIOA.Value = 1;

            // Initialize Data Type ComboBox
            cbxDataType.DropDownStyle = ComboBoxStyle.DropDownList;

            // Initialize common IOA ranges
            cbxIOARange.Items.Clear();
            cbxIOARange.Items.AddRange(new object[]
            {
                "Custom",
                "Digital Inputs (1-999)",
                "Digital Outputs (1000-1999)",
                "Analog Inputs (2000-2999)",
                "Analog Outputs (3000-3999)",
                "Counters (4000-4999)",
                "System Commands (0)"
            });
            cbxIOARange.SelectedIndex = 0;
            cbxIOARange.SelectedIndexChanged += OnIOARangeChanged;
        }

        private void Init()
        {
            TagName = driver.TagNameDesignMode;

            if (!string.IsNullOrEmpty(driver.TagAddressDesignMode))
            {
                TagAddress = driver.TagAddressDesignMode;
            }

            if (!string.IsNullOrEmpty(driver.TagTypeDesignMode))
            {
                TagType = driver.TagTypeDesignMode.GetDataType();
            }

            ValidateInputs(null, null);
        }

        private void OnCategoryChanged(object sender, EventArgs e)
        {
            if (cbxCategory.SelectedItem is string selectedCategory)
            {
                if (categoryTypes.ContainsKey(selectedCategory))
                {
                    var types = categoryTypes[selectedCategory];
                    cbxTypeId.Items.Clear();
                    foreach (var type in types)
                    {
                        cbxTypeId.Items.Add(type);
                    }

                    if (cbxTypeId.Items.Count > 0)
                        cbxTypeId.SelectedIndex = 0;

                    // Update IOA suggestions based on category
                    UpdateIOASuggestionsForCategory(selectedCategory);
                }
            }
        }

        private void OnTypeIdChanged(object sender, EventArgs e)
        {
            if (cbxTypeId.SelectedItem is TypeId selectedType)
            {
                // Update supported data types
                if (typeSupportedDataTypes.ContainsKey(selectedType))
                {
                    var supportedDataTypes = typeSupportedDataTypes[selectedType];
                    cbxDataType.Items.Clear();
                    foreach (var dataType in supportedDataTypes)
                    {
                        cbxDataType.Items.Add(dataType);
                    }
                    if (cbxDataType.Items.Count > 0)
                        cbxDataType.SelectedIndex = 0;
                }

                // Update description
                UpdateTypeDescription(selectedType);

                // Update IOA suggestions based on type
                UpdateIOASuggestionsForType(selectedType);
            }

            ValidateInputs(sender, e);
        }

        private void OnIOARangeChanged(object sender, EventArgs e)
        {
            if (cbxIOARange.SelectedItem is string range && range != "Custom")
            {
                switch (range)
                {
                    case "Digital Inputs (1-999)":
                        nudIOA.Value = 1;
                        break;
                    case "Digital Outputs (1000-1999)":
                        nudIOA.Value = 1000;
                        break;
                    case "Analog Inputs (2000-2999)":
                        nudIOA.Value = 2000;
                        break;
                    case "Analog Outputs (3000-3999)":
                        nudIOA.Value = 3000;
                        break;
                    case "Counters (4000-4999)":
                        nudIOA.Value = 4000;
                        break;
                    case "System Commands (0)":
                        nudIOA.Value = 0;
                        break;
                }
            }
        }

        private void UpdateTypeDescription(TypeId typeId)
        {
            if (typeDescriptions.ContainsKey(typeId))
            {
                lblTypeDescription.Text = typeDescriptions[typeId];

                // Update access type
                string accessType = GetAccessType(typeId);
                lblAccessType.Text = $"Access: {accessType}";

                // Update additional info
                rtbAdditionalInfo.Text = GetAdditionalTypeInfo(typeId);
            }
        }

        private void UpdateIOASuggestionsForCategory(string category)
        {
            string suggestion = "";

            switch (category)
            {
                case "Monitor - Basic":
                case "Monitor - With Time Tag":
                    suggestion = "Monitoring points: 1-4999 (DI: 1-999, AI: 2000-2999, Counters: 4000-4999)";
                    break;
                case "Control - Basic":
                case "Control - With Time Tag":
                    suggestion = "Control points: 1000-3999 (DO: 1000-1999, AO: 3000-3999)";
                    break;
                case "System Commands":
                    suggestion = "System commands: 0 (broadcast) or specific IOA";
                    break;
            }

            lblIOASuggestion.Text = suggestion;
        }

        private void UpdateIOASuggestionsForType(TypeId typeId)
        {
            // Additional specific suggestions based on type
            string typeSpecificSuggestion = "";

            if (typeId.ToString().Contains("_SP_")) // Single point
                typeSpecificSuggestion = " | Single Point: DI (1-999) or DO (1000-1999)";
            else if (typeId.ToString().Contains("_ME_")) // Measured value
                typeSpecificSuggestion = " | Measured Value: AI (2000-2999) or AO (3000-3999)";
            else if (typeId.ToString().Contains("_IT_")) // Integrated totals
                typeSpecificSuggestion = " | Counters: 4000-4999";
            else if (typeId.ToString().StartsWith("C_")) // Commands
                typeSpecificSuggestion = " | Commands: 0 for system, 1000+ for specific points";

            if (!string.IsNullOrEmpty(typeSpecificSuggestion))
            {
                lblIOASuggestion.Text += typeSpecificSuggestion;
            }
        }

        private string GetAdditionalTypeInfo(TypeId typeId)
        {
            var info = "";

            if (typeId.ToString().Contains("_TB_") || typeId.ToString().Contains("_TA_") ||
                typeId.ToString().Contains("_TC_") || typeId.ToString().Contains("_TD_") ||
                typeId.ToString().Contains("_TE_") || typeId.ToString().Contains("_TF_"))
            {
                info += "• Includes CP56Time2a timestamp\n";
            }

            if (typeId.ToString().StartsWith("M_"))
            {
                info += "• Monitoring/Status information\n• Read-only access\n";
            }
            else if (typeId.ToString().StartsWith("C_"))
            {
                info += "• Control/Command information\n• Write access\n";

                if (typeId.ToString().Contains("_IC_") || typeId.ToString().Contains("_CI_") ||
                    typeId.ToString().Contains("_RD_") || typeId.ToString().Contains("_CS_") ||
                    typeId.ToString().Contains("_TS_") || typeId.ToString().Contains("_RP_"))
                {
                    info += "• System command - typically uses IOA = 0\n";
                }
            }

            // Add quality information
            if (!typeId.ToString().Contains("_ND_"))
            {
                info += "• Includes quality information\n";
            }

            return info.TrimEnd('\n');
        }

        private void ValidateInputs(object sender, EventArgs e)
        {
            try
            {
                IsValid = !string.IsNullOrWhiteSpace(TagName) &&
                         cbxTypeId.SelectedItem != null &&
                         cbxDataType.SelectedItem != null &&
                         nudIOA.Value >= nudIOA.Minimum;

                btnOk.Enabled = IsValid;
                btnCheck.Enabled = IsValid;

                if (IsValid)
                {
                    var typeId = (TypeId)cbxTypeId.SelectedItem;
                    var ioa = (int)nudIOA.Value;
                    var dataType = (DataType)cbxDataType.SelectedItem;

                    Description = $"Address: {typeId}:{ioa}\n" +
                                 $"Description: {typeDescriptions[typeId]}\n" +
                                 $"Data Type: {dataType}\n" +
                                 $"Access: {GetAccessType(typeId)}\n" +
                                 $"IOA Range: {GetIOARange(ioa)}";
                }
                else
                {
                    Description = "Please fill in all required fields.";
                }
            }
            catch (Exception ex)
            {
                Description = $"Validation error: {ex.Message}";
                IsValid = false;
                btnOk.Enabled = false;
                btnCheck.Enabled = false;
            }
        }

        private string GetAccessType(TypeId typeId)
        {
            if (typeId.ToString().StartsWith("M_"))
                return "Read Only";
            else if (typeId.ToString().StartsWith("C_"))
                return "Write Only";
            else
                return "System";
        }

        private string GetIOARange(int ioa)
        {
            if (ioa == 0) return "System/Broadcast";
            if (ioa >= 1 && ioa <= 999) return "Digital Inputs";
            if (ioa >= 1000 && ioa <= 1999) return "Digital Outputs";
            if (ioa >= 2000 && ioa <= 2999) return "Analog Inputs";
            if (ioa >= 3000 && ioa <= 3999) return "Analog Outputs";
            if (ioa >= 4000 && ioa <= 4999) return "Counters";
            return "Custom Range";
        }

        private string GenerateTagAddress()
        {
            try
            {
                if (cbxTypeId.SelectedItem is TypeId typeId)
                {
                    return $"{typeId}:{(int)nudIOA.Value}";
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private void ParseTagAddress(string tagAddress)
        {
            try
            {
                if (string.IsNullOrEmpty(tagAddress)) return;

                var parts = tagAddress.Split(':');
                if (parts.Length != 2) return;

                if (Enum.TryParse<TypeId>(parts[0], out TypeId typeId))
                {
                    // Find which category contains this type
                    foreach (var kvp in categoryTypes)
                    {
                        if (kvp.Value.Contains(typeId))
                        {
                            cbxCategory.SelectedItem = kvp.Key;
                            break;
                        }
                    }

                    cbxTypeId.SelectedItem = typeId;
                }

                if (int.TryParse(parts[1], out int ioa))
                {
                    nudIOA.Value = Math.Max(nudIOA.Minimum, Math.Min(nudIOA.Maximum, ioa));

                    // Update IOA range selection
                    string range = GetIOARange(ioa);
                    for (int i = 0; i < cbxIOARange.Items.Count; i++)
                    {
                        if (cbxIOARange.Items[i].ToString().Contains(range) ||
                            (range == "System/Broadcast" && cbxIOARange.Items[i].ToString().Contains("System Commands")))
                        {
                            cbxIOARange.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
            catch { }
        }

        private void CheckKey(char keyChar)
        {
            if (keyChar == (char)13) // Enter
            {
                UpdateTag();
            }
            else if (keyChar == (char)27) // Escape
            {
                Parent?.Dispose();
            }
        }

        private void UpdateTag()
        {
            if (!ValidateTag()) return;

            try
            {
                driver.TagNameDesignMode = TagName;
                driver.TagAddressDesignMode = TagAddress;
                driver.TagTypeDesignMode = TagType.ToDisplayName();

                MessageBox.Show("Tag configuration saved successfully!",
                    "IEC60870 Driver", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Parent?.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving tag configuration: {ex.Message}",
                    "IEC60870 Driver", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateTag()
        {
            if (string.IsNullOrWhiteSpace(TagName))
            {
                MessageBox.Show("Tag name cannot be empty.",
                    "IEC60870 Driver", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTagName.Focus();
                return false;
            }

            if (cbxTypeId.SelectedItem == null)
            {
                MessageBox.Show("Please select a Type ID.",
                    "IEC60870 Driver", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbxTypeId.Focus();
                return false;
            }

            if (cbxDataType.SelectedItem == null)
            {
                MessageBox.Show("Please select a data type.",
                    "IEC60870 Driver", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbxDataType.Focus();
                return false;
            }

            // Validate IOA range for specific types
            var typeId = (TypeId)cbxTypeId.SelectedItem;
            var ioa = (int)nudIOA.Value;

            if (typeId.ToString().StartsWith("C_IC") || typeId.ToString().StartsWith("C_CI") ||
                typeId.ToString().StartsWith("C_CS") || typeId.ToString().StartsWith("C_TS") ||
                typeId.ToString().StartsWith("C_RP"))
            {
                if (ioa != 0)
                {
                    var result = MessageBox.Show(
                        "System commands typically use IOA = 0. Do you want to continue with the current IOA?",
                        "IEC60870 Driver", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.No)
                    {
                        nudIOA.Focus();
                        return false;
                    }
                }
            }

            return true;
        }
    }
}