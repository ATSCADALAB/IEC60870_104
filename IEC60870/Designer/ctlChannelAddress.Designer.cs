namespace IEC60870Driver
{
    partial class ctlChannelAddress
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.GroupBox gbConnectionSettings;
        private System.Windows.Forms.RadioButton rbPermanent;
        private System.Windows.Forms.RadioButton rbCustom;
        private System.Windows.Forms.NumericUpDown nudLifetime;
        private System.Windows.Forms.Label lblSeconds;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;

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
            this.gbConnectionSettings = new System.Windows.Forms.GroupBox();
            this.rbPermanent = new System.Windows.Forms.RadioButton();
            this.rbCustom = new System.Windows.Forms.RadioButton();
            this.nudLifetime = new System.Windows.Forms.NumericUpDown();
            this.lblSeconds = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbConnectionSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLifetime)).BeginInit();
            this.SuspendLayout();
            // 
            // gbConnectionSettings
            // 
            this.gbConnectionSettings.Controls.Add(this.rbPermanent);
            this.gbConnectionSettings.Controls.Add(this.rbCustom);
            this.gbConnectionSettings.Controls.Add(this.nudLifetime);
            this.gbConnectionSettings.Controls.Add(this.lblSeconds);
            this.gbConnectionSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.gbConnectionSettings.Location = new System.Drawing.Point(12, 12);
            this.gbConnectionSettings.Name = "gbConnectionSettings";
            this.gbConnectionSettings.Size = new System.Drawing.Size(320, 80);
            this.gbConnectionSettings.TabIndex = 0;
            this.gbConnectionSettings.TabStop = false;
            this.gbConnectionSettings.Text = "Connection Lifetime Settings";
            // 
            // rbPermanent
            // 
            this.rbPermanent.AutoSize = true;
            this.rbPermanent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.rbPermanent.Location = new System.Drawing.Point(15, 25);
            this.rbPermanent.Name = "rbPermanent";
            this.rbPermanent.Size = new System.Drawing.Size(139, 17);
            this.rbPermanent.TabIndex = 0;
            this.rbPermanent.Text = "Keep connection alive";
            this.rbPermanent.UseVisualStyleBackColor = true;
            // 
            // rbCustom
            // 
            this.rbCustom.AutoSize = true;
            this.rbCustom.Checked = true;
            this.rbCustom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.rbCustom.Location = new System.Drawing.Point(15, 48);
            this.rbCustom.Name = "rbCustom";
            this.rbCustom.Size = new System.Drawing.Size(112, 17);
            this.rbCustom.TabIndex = 1;
            this.rbCustom.TabStop = true;
            this.rbCustom.Text = "Refresh every";
            this.rbCustom.UseVisualStyleBackColor = true;
            // 
            // nudLifetime
            // 
            this.nudLifetime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.nudLifetime.Location = new System.Drawing.Point(133, 47);
            this.nudLifetime.Maximum = new decimal(new int[] { 86400, 0, 0, 0 });
            this.nudLifetime.Minimum = new decimal(new int[] { 60, 0, 0, 0 });
            this.nudLifetime.Name = "nudLifetime";
            this.nudLifetime.Size = new System.Drawing.Size(80, 20);
            this.nudLifetime.TabIndex = 2;
            this.nudLifetime.Value = new decimal(new int[] { 3600, 0, 0, 0 });
            // 
            // lblSeconds
            // 
            this.lblSeconds.AutoSize = true;
            this.lblSeconds.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblSeconds.Location = new System.Drawing.Point(219, 49);
            this.lblSeconds.Name = "lblSeconds";
            this.lblSeconds.Size = new System.Drawing.Size(47, 13);
            this.lblSeconds.TabIndex = 3;
            this.lblSeconds.Text = "seconds";
            // 
            // lblDescription
            // 
            this.lblDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblDescription.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblDescription.Location = new System.Drawing.Point(12, 105);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(320, 40);
            this.lblDescription.TabIndex = 1;
            this.lblDescription.Text = "Description will be updated based on selection.";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(176, 155);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(257, 155);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            //this.btnCancel.Click += new System.EventHandler((s, e) => Parent?.Dispose());
            // 
            // ctlChannelAddress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.gbConnectionSettings);
            this.Name = "ctlChannelAddress";
            this.Size = new System.Drawing.Size(344, 190);
            this.gbConnectionSettings.ResumeLayout(false);
            this.gbConnectionSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLifetime)).EndInit();
            this.ResumeLayout(false);
        }
    }
}