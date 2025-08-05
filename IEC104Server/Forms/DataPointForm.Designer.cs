// File: Forms/DataPointForm.Designer.cs
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
            this.lblType = new System.Windows.Forms.Label();
            this.cmbDataType = new System.Windows.Forms.ComboBox();
            this.lblValue = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.TagName = new ATSCADAWebApp.ToolExtensions.TagCollection.SmartTagComboBox();
            this.SuspendLayout();
            // 
            // lblIOA
            // 
            this.lblIOA.AutoSize = true;
            this.lblIOA.Location = new System.Drawing.Point(12, 15);
            this.lblIOA.Name = "lblIOA";
            this.lblIOA.Size = new System.Drawing.Size(106, 13);
            this.lblIOA.TabIndex = 0;
            this.lblIOA.Text = "Information Address :";
            // 
            // txtIOA
            // 
            this.txtIOA.Location = new System.Drawing.Point(125, 12);
            this.txtIOA.Name = "txtIOA";
            this.txtIOA.Size = new System.Drawing.Size(147, 20);
            this.txtIOA.TabIndex = 1;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(12, 41);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.TabIndex = 2;
            this.lblName.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(125, 38);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(147, 20);
            this.txtName.TabIndex = 2;
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(12, 67);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(60, 13);
            this.lblType.TabIndex = 4;
            this.lblType.Text = "Data Type:";
            // 
            // cmbDataType
            // 
            this.cmbDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDataType.FormattingEnabled = true;
            this.cmbDataType.Location = new System.Drawing.Point(125, 64);
            this.cmbDataType.Name = "cmbDataType";
            this.cmbDataType.Size = new System.Drawing.Size(147, 21);
            this.cmbDataType.TabIndex = 3;
            // 
            // lblValue
            // 
            this.lblValue.AutoSize = true;
            this.lblValue.Location = new System.Drawing.Point(12, 94);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(59, 13);
            this.lblValue.TabIndex = 6;
            this.lblValue.Text = "Tag Value:";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(116, 128);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(197, 128);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // TagName
            // 
            this.TagName.InRuntime = false;
            this.TagName.Location = new System.Drawing.Point(125, 94);
            this.TagName.Name = "TagName";
            this.TagName.Size = new System.Drawing.Size(147, 21);
            this.TagName.TabIndex = 7;
            this.TagName.TagName = "";
            // 
            // DataPointForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(284, 163);
            this.Controls.Add(this.TagName);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblValue);
            this.Controls.Add(this.cmbDataType);
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtIOA);
            this.Controls.Add(this.lblIOA);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DataPointForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Data Point Configuration";
            this.Load += new System.EventHandler(this.DataPointForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblIOA;
        private System.Windows.Forms.TextBox txtIOA;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.ComboBox cmbDataType;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private ATSCADAWebApp.ToolExtensions.TagCollection.SmartTagComboBox TagName;
    }
}