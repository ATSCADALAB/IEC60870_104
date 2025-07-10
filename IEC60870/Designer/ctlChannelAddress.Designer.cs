namespace IEC60870Driver
{
    partial class ctlChannelAddress
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
            this.txtLifetime = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblLifetime = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblLifetime
            // 
            this.lblLifetime.AutoSize = true;
            this.lblLifetime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLifetime.Location = new System.Drawing.Point(12, 14);
            this.lblLifetime.Name = "lblLifetime";
            this.lblLifetime.Size = new System.Drawing.Size(146, 13);
            this.lblLifetime.TabIndex = 0;
            this.lblLifetime.Text = "Connection Lifetime (s):";
            // 
            // txtLifetime
            // 
            this.txtLifetime.Location = new System.Drawing.Point(12, 35);
            this.txtLifetime.Name = "txtLifetime";
            this.txtLifetime.Size = new System.Drawing.Size(200, 20);
            this.txtLifetime.TabIndex = 1;
            this.txtLifetime.Text = "3600";
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblDescription.Location = new System.Drawing.Point(12, 58);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(198, 26);
            this.lblDescription.TabIndex = 2;
            this.lblDescription.Text = "Set connection lifetime in seconds.\r\n0 = Keep connection alive permanently";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(137, 100);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // ctlChannelAddress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.txtLifetime);
            this.Controls.Add(this.lblLifetime);
            this.Name = "ctlChannelAddress";
            this.Size = new System.Drawing.Size(224, 135);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox txtLifetime;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblLifetime;
        private System.Windows.Forms.Label lblDescription;
    }
}