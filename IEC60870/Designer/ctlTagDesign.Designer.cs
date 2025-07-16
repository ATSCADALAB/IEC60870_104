namespace IEC60870Driver
{
    partial class ctlTagDesign
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTagName;
        private System.Windows.Forms.TextBox txtTagName;
        private System.Windows.Forms.GroupBox gbIOASettings;
        private System.Windows.Forms.Label lblIOA;
        private System.Windows.Forms.NumericUpDown nudIOA;
        private System.Windows.Forms.Label lblExamples;
        private System.Windows.Forms.GroupBox gbDataType;
        private System.Windows.Forms.Label lblDataType;
        private System.Windows.Forms.ComboBox cbxDataType;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnCheck;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTagName = new System.Windows.Forms.Label();
            this.txtTagName = new System.Windows.Forms.TextBox();
            this.gbIOASettings = new System.Windows.Forms.GroupBox();
            this.lblIOA = new System.Windows.Forms.Label();
            this.nudIOA = new System.Windows.Forms.NumericUpDown();
            this.lblExamples = new System.Windows.Forms.Label();
            this.gbDataType = new System.Windows.Forms.GroupBox();
            this.lblDataType = new System.Windows.Forms.Label();
            this.cbxDataType = new System.Windows.Forms.ComboBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnCheck = new System.Windows.Forms.Button();

            this.gbIOASettings.SuspendLayout();
            this.gbDataType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudIOA)).BeginInit();
            this.SuspendLayout();

            // 
            // lblTagName
            // 
            this.lblTagName.AutoSize = true;
            this.lblTagName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblTagName.Location = new System.Drawing.Point(12, 15);
            this.lblTagName.Name = "lblTagName";
            this.lblTagName.Size = new System.Drawing.Size(70, 13);
            this.lblTagName.TabIndex = 0;
            this.lblTagName.Text = "Tag Name:";

            // 
            // txtTagName
            // 
            this.txtTagName.Location = new System.Drawing.Point(15, 35);
            this.txtTagName.Name = "txtTagName";
            this.txtTagName.Size = new System.Drawing.Size(200, 20);
            this.txtTagName.TabIndex = 1;

            // 
            // btnCheck
            // 
            this.btnCheck.Location = new System.Drawing.Point(230, 35);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(60, 23);
            this.btnCheck.TabIndex = 2;
            this.btnCheck.Text = "Check";
            this.btnCheck.UseVisualStyleBackColor = true;

            // 
            // gbIOASettings
            // 
            this.gbIOASettings.Controls.Add(this.lblIOA);
            this.gbIOASettings.Controls.Add(this.nudIOA);
            this.gbIOASettings.Controls.Add(this.lblExamples);
            this.gbIOASettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.gbIOASettings.Location = new System.Drawing.Point(15, 70);
            this.gbIOASettings.Name = "gbIOASettings";
            this.gbIOASettings.Size = new System.Drawing.Size(350, 120);
            this.gbIOASettings.TabIndex = 3;
            this.gbIOASettings.TabStop = false;
            this.gbIOASettings.Text = "IOA Settings";

            // 
            // lblIOA
            // 
            this.lblIOA.AutoSize = true;
            this.lblIOA.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblIOA.Location = new System.Drawing.Point(15, 25);
            this.lblIOA.Name = "lblIOA";
            this.lblIOA.Size = new System.Drawing.Size(136, 13);
            this.lblIOA.TabIndex = 0;
            this.lblIOA.Text = "IOA (Information Object Address):";

            // 
            // nudIOA
            // 
            this.nudIOA.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.nudIOA.Location = new System.Drawing.Point(15, 45);
            this.nudIOA.Maximum = new decimal(new int[] { 16777215, 0, 0, 0 });
            this.nudIOA.Name = "nudIOA";
            this.nudIOA.Size = new System.Drawing.Size(100, 20);
            this.nudIOA.TabIndex = 1;
            this.nudIOA.Value = new decimal(new int[] { 1, 0, 0, 0 });

            // 
            // lblExamples
            // 
            this.lblExamples.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblExamples.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblExamples.Location = new System.Drawing.Point(15, 75);
            this.lblExamples.Name = "lblExamples";
            this.lblExamples.Size = new System.Drawing.Size(320, 40);
            this.lblExamples.TabIndex = 2;
            this.lblExamples.Text = "Common IOA ranges will appear here";

            // 
            // gbDataType
            // 
            this.gbDataType.Controls.Add(this.lblDataType);
            this.gbDataType.Controls.Add(this.cbxDataType);
            this.gbDataType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.gbDataType.Location = new System.Drawing.Point(15, 200);
            this.gbDataType.Name = "gbDataType";
            this.gbDataType.Size = new System.Drawing.Size(350, 70);
            this.gbDataType.TabIndex = 4;
            this.gbDataType.TabStop = false;
            this.gbDataType.Text = "Data Type";

            // 
            // lblDataType
            // 
            this.lblDataType.AutoSize = true;
            this.lblDataType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblDataType.Location = new System.Drawing.Point(15, 25);
            this.lblDataType.Name = "lblDataType";
            this.lblDataType.Size = new System.Drawing.Size(60, 13);
            this.lblDataType.TabIndex = 0;
            this.lblDataType.Text = "Data Type:";

            // 
            // cbxDataType
            // 
            this.cbxDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxDataType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbxDataType.FormattingEnabled = true;
            this.cbxDataType.Location = new System.Drawing.Point(15, 45);
            this.cbxDataType.Name = "cbxDataType";
            this.cbxDataType.Size = new System.Drawing.Size(120, 21);
            this.cbxDataType.TabIndex = 1;

            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblDescription.Location = new System.Drawing.Point(15, 285);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(75, 13);
            this.lblDescription.TabIndex = 5;
            this.lblDescription.Text = "Description:";

            // 
            // txtDescription
            // 
            this.txtDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtDescription.Location = new System.Drawing.Point(15, 305);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ReadOnly = true;
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(350, 80);
            this.txtDescription.TabIndex = 6;

            // 
            // btnOk
            // 
            this.btnOk.Enabled = false;
            this.btnOk.Location = new System.Drawing.Point(210, 400);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;

            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(290, 400);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;

            // 
            // ctlTagDesign
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.gbDataType);
            this.Controls.Add(this.gbIOASettings);
            this.Controls.Add(this.btnCheck);
            this.Controls.Add(this.txtTagName);
            this.Controls.Add(this.lblTagName);
            this.Name = "ctlTagDesign";
            this.Size = new System.Drawing.Size(380, 440);
            this.gbIOASettings.ResumeLayout(false);
            this.gbIOASettings.PerformLayout();
            this.gbDataType.ResumeLayout(false);
            this.gbDataType.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudIOA)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}