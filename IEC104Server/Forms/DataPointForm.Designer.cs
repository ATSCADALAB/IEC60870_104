// File: Forms/DataPointForm.Designer.cs - Cập nhật đầy đủ controls
namespace IEC60870ServerWinForm.Forms
{
    partial class DataPointForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblIOA = new System.Windows.Forms.Label();
            this.txtIOA = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblDataType = new System.Windows.Forms.Label();
            this.cmbDataType = new System.Windows.Forms.ComboBox();
            this.lblTypeId = new System.Windows.Forms.Label();
            this.cmbTypeId = new System.Windows.Forms.ComboBox();
            this.lblTagPath = new System.Windows.Forms.Label();
            this.txtTagPath = new System.Windows.Forms.TextBox();
            this.btnTestTag = new System.Windows.Forms.Button();
            this.lblExample = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpBasicInfo = new System.Windows.Forms.GroupBox();
            this.grpDataMapping = new System.Windows.Forms.GroupBox();
            this.grpTagConfiguration = new System.Windows.Forms.GroupBox();
            this.grpBasicInfo.SuspendLayout();
            this.grpDataMapping.SuspendLayout();
            this.grpTagConfiguration.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblIOA
            // 
            this.lblIOA.AutoSize = true;
            this.lblIOA.Location = new System.Drawing.Point(15, 25);
            this.lblIOA.Name = "lblIOA";
            this.lblIOA.Size = new System.Drawing.Size(140, 13);
            this.lblIOA.TabIndex = 0;
            this.lblIOA.Text = "Information Object Address:";
            // 
            // txtIOA
            // 
            this.txtIOA.Location = new System.Drawing.Point(160, 22);
            this.txtIOA.Name = "txtIOA";
            this.txtIOA.Size = new System.Drawing.Size(120, 20);
            this.txtIOA.TabIndex = 1;
            this.txtIOA.Text = "1001";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(15, 51);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.TabIndex = 2;
            this.lblName.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(160, 48);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(200, 20);
            this.txtName.TabIndex = 3;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(15, 77);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(63, 13);
            this.lblDescription.TabIndex = 4;
            this.lblDescription.Text = "Description:";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(160, 74);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(200, 40);
            this.txtDescription.TabIndex = 5;
            // 
            // lblDataType
            // 
            this.lblDataType.AutoSize = true;
            this.lblDataType.Location = new System.Drawing.Point(15, 25);
            this.lblDataType.Name = "lblDataType";
            this.lblDataType.Size = new System.Drawing.Size(62, 13);
            this.lblDataType.TabIndex = 6;
            this.lblDataType.Text = "Data Type:";
            // 
            // cmbDataType
            // 
            this.cmbDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDataType.FormattingEnabled = true;
            this.cmbDataType.Location = new System.Drawing.Point(120, 22);
            this.cmbDataType.Name = "cmbDataType";
            this.cmbDataType.Size = new System.Drawing.Size(120, 21);
            this.cmbDataType.TabIndex = 7;
            // 
            // lblTypeId
            // 
            this.lblTypeId.AutoSize = true;
            this.lblTypeId.Location = new System.Drawing.Point(15, 52);
            this.lblTypeId.Name = "lblTypeId";
            this.lblTypeId.Size = new System.Drawing.Size(74, 13);
            this.lblTypeId.TabIndex = 8;
            this.lblTypeId.Text = "IEC60870 Type:";
            // 
            // cmbTypeId
            // 
            this.cmbTypeId.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTypeId.FormattingEnabled = true;
            this.cmbTypeId.Location = new System.Drawing.Point(120, 49);
            this.cmbTypeId.Name = "cmbTypeId";
            this.cmbTypeId.Size = new System.Drawing.Size(160, 21);
            this.cmbTypeId.TabIndex = 9;
            // 
            // lblTagPath
            // 
            this.lblTagPath.AutoSize = true;
            this.lblTagPath.Location = new System.Drawing.Point(15, 25);
            this.lblTagPath.Name = "lblTagPath";
            this.lblTagPath.Size = new System.Drawing.Size(96, 13);
            this.lblTagPath.TabIndex = 10;
            this.lblTagPath.Text = "Tag Path (Task.Tag):";
            // 
            // txtTagPath
            // 
            this.txtTagPath.Location = new System.Drawing.Point(120, 22);
            this.txtTagPath.Name = "txtTagPath";
            this.txtTagPath.Size = new System.Drawing.Size(180, 20);
            this.txtTagPath.TabIndex = 11;
            this.txtTagPath.Text = "";
            // 
            // btnTestTag
            // 
            this.btnTestTag.Location = new System.Drawing.Point(306, 20);
            this.btnTestTag.Name = "btnTestTag";
            this.btnTestTag.Size = new System.Drawing.Size(60, 23);
            this.btnTestTag.TabIndex = 12;
            this.btnTestTag.Text = "Test";
            this.btnTestTag.UseVisualStyleBackColor = true;
            this.btnTestTag.Click += new System.EventHandler(this.btnTestTag_Click);
            // 
            // lblExample
            // 
            this.lblExample.AutoSize = true;
            this.lblExample.ForeColor = System.Drawing.Color.Gray;
            this.lblExample.Location = new System.Drawing.Point(117, 48);
            this.lblExample.Name = "lblExample";
            this.lblExample.Size = new System.Drawing.Size(200, 13);
            this.lblExample.TabIndex = 13;
            this.lblExample.Text = "Example: PLC1.Motor1_Status";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(230, 350);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 14;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(311, 350);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // grpBasicInfo
            // 
            this.grpBasicInfo.Controls.Add(this.lblIOA);
            this.grpBasicInfo.Controls.Add(this.txtIOA);
            this.grpBasicInfo.Controls.Add(this.lblName);
            this.grpBasicInfo.Controls.Add(this.txtName);
            this.grpBasicInfo.Controls.Add(this.lblDescription);
            this.grpBasicInfo.Controls.Add(this.txtDescription);
            this.grpBasicInfo.Location = new System.Drawing.Point(12, 12);
            this.grpBasicInfo.Name = "grpBasicInfo";
            this.grpBasicInfo.Size = new System.Drawing.Size(374, 130);
            this.grpBasicInfo.TabIndex = 16;
            this.grpBasicInfo.TabStop = false;
            this.grpBasicInfo.Text = "Basic Information";
            // 
            // grpDataMapping
            // 
            this.grpDataMapping.Controls.Add(this.lblDataType);
            this.grpDataMapping.Controls.Add(this.cmbDataType);
            this.grpDataMapping.Controls.Add(this.lblTypeId);
            this.grpDataMapping.Controls.Add(this.cmbTypeId);
            this.grpDataMapping.Location = new System.Drawing.Point(12, 148);
            this.grpDataMapping.Name = "grpDataMapping";
            this.grpDataMapping.Size = new System.Drawing.Size(374, 85);
            this.grpDataMapping.TabIndex = 17;
            this.grpDataMapping.TabStop = false;
            this.grpDataMapping.Text = "Data Type Mapping";
            // 
            // grpTagConfiguration
            // 
            this.grpTagConfiguration.Controls.Add(this.lblTagPath);
            this.grpTagConfiguration.Controls.Add(this.txtTagPath);
            this.grpTagConfiguration.Controls.Add(this.btnTestTag);
            this.grpTagConfiguration.Controls.Add(this.lblExample);
            this.grpTagConfiguration.Location = new System.Drawing.Point(12, 239);
            this.grpTagConfiguration.Name = "grpTagConfiguration";
            this.grpTagConfiguration.Size = new System.Drawing.Size(374, 75);
            this.grpTagConfiguration.TabIndex = 18;
            this.grpTagConfiguration.TabStop = false;
            this.grpTagConfiguration.Text = "SCADA Tag Configuration";
            // 
            // DataPointForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(398, 385);
            this.Controls.Add(this.grpTagConfiguration);
            this.Controls.Add(this.grpDataMapping);
            this.Controls.Add(this.grpBasicInfo);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DataPointForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Data Point Configuration";
            this.Load += new System.EventHandler(this.DataPointForm_Load);
            this.grpBasicInfo.ResumeLayout(false);
            this.grpBasicInfo.PerformLayout();
            this.grpDataMapping.ResumeLayout(false);
            this.grpDataMapping.PerformLayout();
            this.grpTagConfiguration.ResumeLayout(false);
            this.grpTagConfiguration.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblIOA;
        private System.Windows.Forms.TextBox txtIOA;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblDataType;
        private System.Windows.Forms.ComboBox cmbDataType;
        private System.Windows.Forms.Label lblTypeId;
        private System.Windows.Forms.ComboBox cmbTypeId;
        private System.Windows.Forms.Label lblTagPath;
        private System.Windows.Forms.TextBox txtTagPath;
        private System.Windows.Forms.Button btnTestTag;
        private System.Windows.Forms.Label lblExample;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpBasicInfo;
        private System.Windows.Forms.GroupBox grpDataMapping;
        private System.Windows.Forms.GroupBox grpTagConfiguration;
    }
}