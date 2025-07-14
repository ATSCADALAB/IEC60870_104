namespace IEC60870Driver
{
    partial class ctlTagDesign
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTagName;
        private System.Windows.Forms.TextBox txtTagName;

        private System.Windows.Forms.GroupBox gbTypeSelection;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.ComboBox cbxCategory;
        private System.Windows.Forms.Label lblTypeId;
        private System.Windows.Forms.ComboBox cbxTypeId;
        private System.Windows.Forms.Label lblTypeDescription;
        private System.Windows.Forms.Label lblAccessType;

        private System.Windows.Forms.GroupBox gbAddressSettings;
        private System.Windows.Forms.Label lblIOARange;
        private System.Windows.Forms.ComboBox cbxIOARange;
        private System.Windows.Forms.Label lblIOA;
        private System.Windows.Forms.NumericUpDown nudIOA;
        private System.Windows.Forms.Label lblIOASuggestion;

        private System.Windows.Forms.GroupBox gbDataType;
        private System.Windows.Forms.Label lblDataType;
        private System.Windows.Forms.ComboBox cbxDataType;

        private System.Windows.Forms.GroupBox gbInformation;
        private System.Windows.Forms.RichTextBox rtbAdditionalInfo;

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
            this.gbTypeSelection = new System.Windows.Forms.GroupBox();
            this.lblCategory = new System.Windows.Forms.Label();
            this.cbxCategory = new System.Windows.Forms.ComboBox();
            this.lblTypeId = new System.Windows.Forms.Label();
            this.cbxTypeId = new System.Windows.Forms.ComboBox();
            this.lblTypeDescription = new System.Windows.Forms.Label();
            this.lblAccessType = new System.Windows.Forms.Label();
            this.gbAddressSettings = new System.Windows.Forms.GroupBox();
            this.lblIOARange = new System.Windows.Forms.Label();
            this.cbxIOARange = new System.Windows.Forms.ComboBox();
            this.lblIOA = new System.Windows.Forms.Label();
            this.nudIOA = new System.Windows.Forms.NumericUpDown();
            this.lblIOASuggestion = new System.Windows.Forms.Label();
            this.gbDataType = new System.Windows.Forms.GroupBox();
            this.lblDataType = new System.Windows.Forms.Label();
            this.cbxDataType = new System.Windows.Forms.ComboBox();
            this.gbInformation = new System.Windows.Forms.GroupBox();
            this.rtbAdditionalInfo = new System.Windows.Forms.RichTextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnCheck = new System.Windows.Forms.Button();

            this.gbTypeSelection.SuspendLayout();
            this.gbAddressSettings.SuspendLayout();
            this.gbDataType.SuspendLayout();
            this.gbInformation.SuspendLayout();
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
            // gbTypeSelection
            // 
            this.gbTypeSelection.Controls.Add(this.lblCategory);
            this.gbTypeSelection.Controls.Add(this.cbxCategory);
            this.gbTypeSelection.Controls.Add(this.lblTypeId);
            this.gbTypeSelection.Controls.Add(this.cbxTypeId);
            this.gbTypeSelection.Controls.Add(this.lblTypeDescription);
            this.gbTypeSelection.Controls.Add(this.lblAccessType);
            this.gbTypeSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.gbTypeSelection.Location = new System.Drawing.Point(15, 70);
            this.gbTypeSelection.Name = "gbTypeSelection";
            this.gbTypeSelection.Size = new System.Drawing.Size(500, 120);
            this.gbTypeSelection.TabIndex = 2;
            this.gbTypeSelection.TabStop = false;
            this.gbTypeSelection.Text = "Type Selection";

            // 
            // lblCategory
            // 
            this.lblCategory.AutoSize = true;
            this.lblCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblCategory.Location = new System.Drawing.Point(15, 25);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(52, 13);
            this.lblCategory.TabIndex = 0;
            this.lblCategory.Text = "Category:";

            // 
            // cbxCategory
            // 
            this.cbxCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbxCategory.FormattingEnabled = true;
            this.cbxCategory.Location = new System.Drawing.Point(15, 45);
            this.cbxCategory.Name = "cbxCategory";
            this.cbxCategory.Size = new System.Drawing.Size(180, 21);
            this.cbxCategory.TabIndex = 1;

            // 
            // lblTypeId
            // 
            this.lblTypeId.AutoSize = true;
            this.lblTypeId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblTypeId.Location = new System.Drawing.Point(220, 25);
            this.lblTypeId.Name = "lblTypeId";
            this.lblTypeId.Size = new System.Drawing.Size(48, 13);
            this.lblTypeId.TabIndex = 2;
            this.lblTypeId.Text = "Type ID:";

            // 
            // cbxTypeId
            // 
            this.cbxTypeId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbxTypeId.FormattingEnabled = true;
            this.cbxTypeId.Location = new System.Drawing.Point(220, 45);
            this.cbxTypeId.Name = "cbxTypeId";
            this.cbxTypeId.Size = new System.Drawing.Size(150, 21);
            this.cbxTypeId.TabIndex = 3;

            // 
            // lblTypeDescription
            // 
            this.lblTypeDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblTypeDescription.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblTypeDescription.Location = new System.Drawing.Point(15, 75);
            this.lblTypeDescription.Name = "lblTypeDescription";
            this.lblTypeDescription.Size = new System.Drawing.Size(350, 17);
            this.lblTypeDescription.TabIndex = 4;
            this.lblTypeDescription.Text = "Select a type to see description";

            // 
            // lblAccessType
            // 
            this.lblAccessType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblAccessType.ForeColor = System.Drawing.Color.Blue;
            this.lblAccessType.Location = new System.Drawing.Point(380, 47);
            this.lblAccessType.Name = "lblAccessType";
            this.lblAccessType.Size = new System.Drawing.Size(110, 17);
            this.lblAccessType.TabIndex = 5;
            this.lblAccessType.Text = "Access: ";

            // 
            // gbAddressSettings
            // 
            this.gbAddressSettings.Controls.Add(this.lblIOARange);
            this.gbAddressSettings.Controls.Add(this.cbxIOARange);
            this.gbAddressSettings.Controls.Add(this.lblIOA);
            this.gbAddressSettings.Controls.Add(this.nudIOA);
            this.gbAddressSettings.Controls.Add(this.lblIOASuggestion);
            this.gbAddressSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.gbAddressSettings.Location = new System.Drawing.Point(15, 200);
            this.gbAddressSettings.Name = "gbAddressSettings";
            this.gbAddressSettings.Size = new System.Drawing.Size(500, 100);
            this.gbAddressSettings.TabIndex = 3;
            this.gbAddressSettings.TabStop = false;
            this.gbAddressSettings.Text = "Address Settings";

            // 
            // lblIOARange
            // 
            this.lblIOARange.AutoSize = true;
            this.lblIOARange.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblIOARange.Location = new System.Drawing.Point(15, 25);
            this.lblIOARange.Name = "lblIOARange";
            this.lblIOARange.Size = new System.Drawing.Size(62, 13);
            this.lblIOARange.TabIndex = 0;
            this.lblIOARange.Text = "IOA Range:";

            // 
            // cbxIOARange
            // 
            this.cbxIOARange.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbxIOARange.FormattingEnabled = true;
            this.cbxIOARange.Location = new System.Drawing.Point(15, 45);
            this.cbxIOARange.Name = "cbxIOARange";
            this.cbxIOARange.Size = new System.Drawing.Size(200, 21);
            this.cbxIOARange.TabIndex = 1;

            // 
            // lblIOA
            // 
            this.lblIOA.AutoSize = true;
            this.lblIOA.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblIOA.Location = new System.Drawing.Point(240, 25);
            this.lblIOA.Name = "lblIOA";
            this.lblIOA.Size = new System.Drawing.Size(28, 13);
            this.lblIOA.TabIndex = 2;
            this.lblIOA.Text = "IOA:";

            // 
            // nudIOA
            // 
            this.nudIOA.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.nudIOA.Location = new System.Drawing.Point(240, 45);
            this.nudIOA.Maximum = new decimal(new int[] { 16777215, 0, 0, 0 });
            this.nudIOA.Name = "nudIOA";
            this.nudIOA.Size = new System.Drawing.Size(100, 20);
            this.nudIOA.TabIndex = 3;
            this.nudIOA.Value = new decimal(new int[] { 1, 0, 0, 0 });

            // 
            // lblIOASuggestion
            // 
            this.lblIOASuggestion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblIOASuggestion.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblIOASuggestion.Location = new System.Drawing.Point(15, 75);
            this.lblIOASuggestion.Name = "lblIOASuggestion";
            this.lblIOASuggestion.Size = new System.Drawing.Size(470, 17);
            this.lblIOASuggestion.TabIndex = 4;
            this.lblIOASuggestion.Text = "IOA range suggestion will appear here";

            // 
            // gbDataType
            // 
            this.gbDataType.Controls.Add(this.lblDataType);
            this.gbDataType.Controls.Add(this.cbxDataType);
            this.gbDataType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.gbDataType.Location = new System.Drawing.Point(15, 310);
            this.gbDataType.Name = "gbDataType";
            this.gbDataType.Size = new System.Drawing.Size(240, 70);
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
            this.cbxDataType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbxDataType.FormattingEnabled = true;
            this.cbxDataType.Location = new System.Drawing.Point(15, 45);
            this.cbxDataType.Name = "cbxDataType";
            this.cbxDataType.Size = new System.Drawing.Size(120, 21);
            this.cbxDataType.TabIndex = 1;

            // 
            // gbInformation
            // 
            this.gbInformation.Controls.Add(this.rtbAdditionalInfo);
            this.gbInformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.gbInformation.Location = new System.Drawing.Point(275, 310);
            this.gbInformation.Name = "gbInformation";
            this.gbInformation.Size = new System.Drawing.Size(240, 70);
            this.gbInformation.TabIndex = 5;
            this.gbInformation.TabStop = false;
            this.gbInformation.Text = "Type Information";

            // 
            // rtbAdditionalInfo
            // 
            this.rtbAdditionalInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.rtbAdditionalInfo.Location = new System.Drawing.Point(10, 20);
            this.rtbAdditionalInfo.Name = "rtbAdditionalInfo";
            this.rtbAdditionalInfo.ReadOnly = true;
            this.rtbAdditionalInfo.Size = new System.Drawing.Size(220, 40);
            this.rtbAdditionalInfo.TabIndex = 0;
            this.rtbAdditionalInfo.Text = "Additional type information will appear here";

            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblDescription.Location = new System.Drawing.Point(15, 395);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(75, 13);
            this.lblDescription.TabIndex = 6;
            this.lblDescription.Text = "Description:";

            // 
            // txtDescription
            // 
            this.txtDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtDescription.Location = new System.Drawing.Point(15, 415);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ReadOnly = true;
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(500, 80);
            this.txtDescription.TabIndex = 7;

            // 
            // btnCheck
            // 
            this.btnCheck.Location = new System.Drawing.Point(230, 35);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(60, 23);
            this.btnCheck.TabIndex = 8;
            this.btnCheck.Text = "Check";
            this.btnCheck.UseVisualStyleBackColor = true;

            // 
            // btnOk
            // 
            this.btnOk.Enabled = false;
            this.btnOk.Location = new System.Drawing.Point(360, 510);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 9;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;

            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(440, 510);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            //this.btnCancel.Click += new System.EventHandler((s, e) => Parent?.Dispose());

            // 
            // ctlTagDesign
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCheck);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.gbInformation);
            this.Controls.Add(this.gbDataType);
            this.Controls.Add(this.gbAddressSettings);
            this.Controls.Add(this.gbTypeSelection);
            this.Controls.Add(this.txtTagName);
            this.Controls.Add(this.lblTagName);
            this.Name = "ctlTagDesign";
            this.Size = new System.Drawing.Size(530, 550);
            this.gbTypeSelection.ResumeLayout(false);
            this.gbTypeSelection.PerformLayout();
            this.gbAddressSettings.ResumeLayout(false);
            this.gbAddressSettings.PerformLayout();
            this.gbDataType.ResumeLayout(false);
            this.gbDataType.PerformLayout();
            this.gbInformation.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudIOA)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}