namespace IEC60870Driver
{
    partial class ctlTagDesign
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtTagName = new System.Windows.Forms.TextBox();
            this.txtTagAddress = new System.Windows.Forms.TextBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.cbxDataType = new System.Windows.Forms.ComboBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCheck = new System.Windows.Forms.Button();
            this.lblTagName = new System.Windows.Forms.Label();
            this.lblTagAddress = new System.Windows.Forms.Label();
            this.lblDataType = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblExamples = new System.Windows.Forms.Label();
            this.lblFormat = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblTypeInfo = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTagName
            // 
            this.lblTagName.AutoSize = true;
            this.lblTagName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTagName.Location = new System.Drawing.Point(12, 14);
            this.lblTagName.Name = "lblTagName";
            this.lblTagName.Size = new System.Drawing.Size(70, 13);
            this.lblTagName.TabIndex = 0;
            this.lblTagName.Text = "Tag Name:";
            // 
            // txtTagName
            // 
            this.txtTagName.Location = new System.Drawing.Point(12, 35);
            this.txtTagName.Name = "txtTagName";
            this.txtTagName.Size = new System.Drawing.Size(200, 20);
            this.txtTagName.TabIndex = 1;
            // 
            // lblTagAddress
            // 
            this.lblTagAddress.AutoSize = true;
            this.lblTagAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTagAddress.Location = new System.Drawing.Point(230, 14);
            this.lblTagAddress.Name = "lblTagAddress";
            this.lblTagAddress.Size = new System.Drawing.Size(81, 13);
            this.lblTagAddress.TabIndex = 2;
            this.lblTagAddress.Text = "Tag Address:";
            // 
            // txtTagAddress
            // 
            this.txtTagAddress.Location = new System.Drawing.Point(230, 35);
            this.txtTagAddress.Name = "txtTagAddress";
            this.txtTagAddress.Size = new System.Drawing.Size(150, 20);
            this.txtTagAddress.TabIndex = 3;
            // 
            // btnCheck
            // 
            this.btnCheck.Location = new System.Drawing.Point(390, 34);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(60, 23);
            this.btnCheck.TabIndex = 4;
            this.btnCheck.Text = "Check";
            this.btnCheck.UseVisualStyleBackColor = true;
            // 
            // lblDataType
            // 
            this.lblDataType.AutoSize = true;
            this.lblDataType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDataType.Location = new System.Drawing.Point(12, 70);
            this.lblDataType.Name = "lblDataType";
            this.lblDataType.Size = new System.Drawing.Size(70, 13);
            this.lblDataType.TabIndex = 5;
            this.lblDataType.Text = "Data Type:";
            // 
            // cbxDataType
            // 
            this.cbxDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxDataType.FormattingEnabled = true;
            this.cbxDataType.Location = new System.Drawing.Point(12, 91);
            this.cbxDataType.Name = "cbxDataType";
            this.cbxDataType.Size = new System.Drawing.Size(120, 21);
            this.cbxDataType.TabIndex = 6;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblExamples);
            this.groupBox1.Controls.Add(this.lblFormat);
            this.groupBox1.Location = new System.Drawing.Point(12, 130);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(438, 100);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Address Format";
            // 
            // lblFormat
            // 
            this.lblFormat.AutoSize = true;
            this.lblFormat.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFormat.Location = new System.Drawing.Point(6, 20);
            this.lblFormat.Name = "lblFormat";
            this.lblFormat.Size = new System.Drawing.Size(85, 13);
            this.lblFormat.TabIndex = 0;
            this.lblFormat.Text = "Format: TypeId:IOA";
            // 
            // lblExamples
            // 
            this.lblExamples.AutoSize = true;
            this.lblExamples.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblExamples.Location = new System.Drawing.Point(6, 40);
            this.lblExamples.Name = "lblExamples";
            this.lblExamples.Size = new System.Drawing.Size(415, 52);
            this.lblExamples.TabIndex = 1;
            this.lblExamples.Text = "Examples:\r\nM_SP_NA_1:100    - Single Point Information at IOA 100\r\nM_ME_NC_1:200" +
    "    - Measured Value (Float) at IOA 200\r\nC_SC_NA_1:1000   - Single Command at I" +
    "OA 1000";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblTypeInfo);
            this.groupBox2.Location = new System.Drawing.Point(12, 240);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(438, 80);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Supported IEC60870 Types";
            // 
            // lblTypeInfo
            // 
            this.lblTypeInfo.AutoSize = true;
            this.lblTypeInfo.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTypeInfo.Location = new System.Drawing.Point(6, 20);
            this.lblTypeInfo.Name = "lblTypeInfo";
            this.lblTypeInfo.Size = new System.Drawing.Size(415, 52);
            this.lblTypeInfo.TabIndex = 0;
            this.lblTypeInfo.Text = "Monitor: M_SP_NA_1, M_DP_NA_1, M_ME_NA_1, M_ME_NB_1, M_ME_NC_1\r\nControl: C_SC_N" +
    "A_1, C_DC_NA_1, C_SE_NA_1, C_SE_NC_1\r\nSystem:  C_IC_NA_1 (Interrogation)\r\nData" +
    " Types: Bool, Word, Int, DWord, Float, String";
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescription.Location = new System.Drawing.Point(12, 335);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(75, 13);
            this.lblDescription.TabIndex = 9;
            this.lblDescription.Text = "Description:";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(12, 356);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ReadOnly = true;
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(438, 50);
            this.txtDescription.TabIndex = 10;
            // 
            // btnOk
            // 
            this.btnOk.Enabled = false;
            this.btnOk.Location = new System.Drawing.Point(375, 420);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 11;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // ctlTagDesign
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cbxDataType);
            this.Controls.Add(this.lblDataType);
            this.Controls.Add(this.btnCheck);
            this.Controls.Add(this.txtTagAddress);
            this.Controls.Add(this.lblTagAddress);
            this.Controls.Add(this.txtTagName);
            this.Controls.Add(this.lblTagName);
            this.Name = "ctlTagDesign";
            this.Size = new System.Drawing.Size(462, 455);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox txtTagName;
        private System.Windows.Forms.TextBox txtTagAddress;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.ComboBox cbxDataType;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCheck;
        private System.Windows.Forms.Label lblTagName;
        private System.Windows.Forms.Label lblTagAddress;
        private System.Windows.Forms.Label lblDataType;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblExamples;
        private System.Windows.Forms.Label lblFormat;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblTypeInfo;
    }
}