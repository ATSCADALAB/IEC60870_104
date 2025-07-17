namespace IEC60870Driver
{
    partial class ctlDeviceDesign
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblDeviceName;
        private System.Windows.Forms.TextBox txtDeviceName;
        private System.Windows.Forms.GroupBox gbConnection;
        private System.Windows.Forms.Label lblIpAddress;
        private System.Windows.Forms.TextBox txtIpAddress;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.NumericUpDown nudPort;
        private System.Windows.Forms.GroupBox gbProtocol;
        private System.Windows.Forms.Label lblCommonAddress;
        private System.Windows.Forms.NumericUpDown nudCommonAddress;
        private System.Windows.Forms.Label lblOriginatorAddress;
        private System.Windows.Forms.NumericUpDown nudOriginatorAddress;
        private System.Windows.Forms.Label lblCotFieldLength;
        private System.Windows.Forms.ComboBox cbxCotFieldLength;
        private System.Windows.Forms.Label lblCaFieldLength;
        private System.Windows.Forms.ComboBox cbxCaFieldLength;
        private System.Windows.Forms.Label lblIoaFieldLength;
        private System.Windows.Forms.ComboBox cbxIoaFieldLength;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Button btnOk;
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
            this.lblDeviceName = new System.Windows.Forms.Label();
            this.txtDeviceName = new System.Windows.Forms.TextBox();
            this.gbConnection = new System.Windows.Forms.GroupBox();
            this.lblIpAddress = new System.Windows.Forms.Label();
            this.txtIpAddress = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.nudPort = new System.Windows.Forms.NumericUpDown();
            this.gbProtocol = new System.Windows.Forms.GroupBox();
            this.lblCommonAddress = new System.Windows.Forms.Label();
            this.nudCommonAddress = new System.Windows.Forms.NumericUpDown();
            this.lblOriginatorAddress = new System.Windows.Forms.Label();
            this.nudOriginatorAddress = new System.Windows.Forms.NumericUpDown();
            this.lblCotFieldLength = new System.Windows.Forms.Label();
            this.cbxCotFieldLength = new System.Windows.Forms.ComboBox();
            this.lblCaFieldLength = new System.Windows.Forms.Label();
            this.cbxCaFieldLength = new System.Windows.Forms.ComboBox();
            this.lblIoaFieldLength = new System.Windows.Forms.Label();
            this.cbxIoaFieldLength = new System.Windows.Forms.ComboBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbConnection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
            this.gbProtocol.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCommonAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOriginatorAddress)).BeginInit();
            this.SuspendLayout();
            // 
            // lblDeviceName
            // 
            this.lblDeviceName.AutoSize = true;
            this.lblDeviceName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblDeviceName.Location = new System.Drawing.Point(12, 15);
            this.lblDeviceName.Name = "lblDeviceName";
            this.lblDeviceName.Size = new System.Drawing.Size(87, 13);
            this.lblDeviceName.TabIndex = 0;
            this.lblDeviceName.Text = "Device Name:";
            // 
            // txtDeviceName
            // 
            this.txtDeviceName.Location = new System.Drawing.Point(15, 35);
            this.txtDeviceName.Name = "txtDeviceName";
            this.txtDeviceName.Size = new System.Drawing.Size(250, 20);
            this.txtDeviceName.TabIndex = 1;
            // 
            // gbConnection
            // 
            this.gbConnection.Controls.Add(this.lblIpAddress);
            this.gbConnection.Controls.Add(this.txtIpAddress);
            this.gbConnection.Controls.Add(this.lblPort);
            this.gbConnection.Controls.Add(this.nudPort);
            this.gbConnection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.gbConnection.Location = new System.Drawing.Point(15, 70);
            this.gbConnection.Name = "gbConnection";
            this.gbConnection.Size = new System.Drawing.Size(350, 80);
            this.gbConnection.TabIndex = 3;
            this.gbConnection.TabStop = false;
            this.gbConnection.Text = "Connection Settings";
            // 
            // lblIpAddress
            // 
            this.lblIpAddress.AutoSize = true;
            this.lblIpAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblIpAddress.Location = new System.Drawing.Point(15, 25);
            this.lblIpAddress.Name = "lblIpAddress";
            this.lblIpAddress.Size = new System.Drawing.Size(61, 13);
            this.lblIpAddress.TabIndex = 0;
            this.lblIpAddress.Text = "IP Address:";
            // 
            // txtIpAddress
            // 
            this.txtIpAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtIpAddress.Location = new System.Drawing.Point(15, 45);
            this.txtIpAddress.Name = "txtIpAddress";
            this.txtIpAddress.Size = new System.Drawing.Size(150, 20);
            this.txtIpAddress.TabIndex = 1;
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblPort.Location = new System.Drawing.Point(200, 25);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(29, 13);
            this.lblPort.TabIndex = 2;
            this.lblPort.Text = "Port:";
            // 
            // nudPort
            // 
            this.nudPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.nudPort.Location = new System.Drawing.Point(200, 45);
            this.nudPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudPort.Name = "nudPort";
            this.nudPort.Size = new System.Drawing.Size(80, 20);
            this.nudPort.TabIndex = 3;
            this.nudPort.Value = new decimal(new int[] {
            2404,
            0,
            0,
            0});
            // 
            // gbProtocol
            // 
            this.gbProtocol.Controls.Add(this.lblCommonAddress);
            this.gbProtocol.Controls.Add(this.nudCommonAddress);
            this.gbProtocol.Controls.Add(this.lblOriginatorAddress);
            this.gbProtocol.Controls.Add(this.nudOriginatorAddress);
            this.gbProtocol.Controls.Add(this.lblCotFieldLength);
            this.gbProtocol.Controls.Add(this.cbxCotFieldLength);
            this.gbProtocol.Controls.Add(this.lblCaFieldLength);
            this.gbProtocol.Controls.Add(this.cbxCaFieldLength);
            this.gbProtocol.Controls.Add(this.lblIoaFieldLength);
            this.gbProtocol.Controls.Add(this.cbxIoaFieldLength);
            this.gbProtocol.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.gbProtocol.Location = new System.Drawing.Point(15, 160);
            this.gbProtocol.Name = "gbProtocol";
            this.gbProtocol.Size = new System.Drawing.Size(350, 150);
            this.gbProtocol.TabIndex = 4;
            this.gbProtocol.TabStop = false;
            this.gbProtocol.Text = "Protocol Settings";
            // 
            // lblCommonAddress
            // 
            this.lblCommonAddress.AutoSize = true;
            this.lblCommonAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblCommonAddress.Location = new System.Drawing.Point(15, 25);
            this.lblCommonAddress.Name = "lblCommonAddress";
            this.lblCommonAddress.Size = new System.Drawing.Size(92, 13);
            this.lblCommonAddress.TabIndex = 0;
            this.lblCommonAddress.Text = "Common Address:";
            // 
            // nudCommonAddress
            // 
            this.nudCommonAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.nudCommonAddress.Location = new System.Drawing.Point(15, 45);
            this.nudCommonAddress.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudCommonAddress.Name = "nudCommonAddress";
            this.nudCommonAddress.Size = new System.Drawing.Size(80, 20);
            this.nudCommonAddress.TabIndex = 1;
            this.nudCommonAddress.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblOriginatorAddress
            // 
            this.lblOriginatorAddress.AutoSize = true;
            this.lblOriginatorAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblOriginatorAddress.Location = new System.Drawing.Point(180, 25);
            this.lblOriginatorAddress.Name = "lblOriginatorAddress";
            this.lblOriginatorAddress.Size = new System.Drawing.Size(96, 13);
            this.lblOriginatorAddress.TabIndex = 2;
            this.lblOriginatorAddress.Text = "Originator Address:";
            // 
            // nudOriginatorAddress
            // 
            this.nudOriginatorAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.nudOriginatorAddress.Location = new System.Drawing.Point(180, 45);
            this.nudOriginatorAddress.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudOriginatorAddress.Name = "nudOriginatorAddress";
            this.nudOriginatorAddress.Size = new System.Drawing.Size(80, 20);
            this.nudOriginatorAddress.TabIndex = 3;
            // 
            // lblCotFieldLength
            // 
            this.lblCotFieldLength.AutoSize = true;
            this.lblCotFieldLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblCotFieldLength.Location = new System.Drawing.Point(15, 75);
            this.lblCotFieldLength.Name = "lblCotFieldLength";
            this.lblCotFieldLength.Size = new System.Drawing.Size(93, 13);
            this.lblCotFieldLength.TabIndex = 4;
            this.lblCotFieldLength.Text = "COT Field Length:";
            // 
            // cbxCotFieldLength
            // 
            this.cbxCotFieldLength.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxCotFieldLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbxCotFieldLength.FormattingEnabled = true;
            this.cbxCotFieldLength.Location = new System.Drawing.Point(15, 95);
            this.cbxCotFieldLength.Name = "cbxCotFieldLength";
            this.cbxCotFieldLength.Size = new System.Drawing.Size(80, 21);
            this.cbxCotFieldLength.TabIndex = 5;
            // 
            // lblCaFieldLength
            // 
            this.lblCaFieldLength.AutoSize = true;
            this.lblCaFieldLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblCaFieldLength.Location = new System.Drawing.Point(120, 75);
            this.lblCaFieldLength.Name = "lblCaFieldLength";
            this.lblCaFieldLength.Size = new System.Drawing.Size(85, 13);
            this.lblCaFieldLength.TabIndex = 6;
            this.lblCaFieldLength.Text = "CA Field Length:";
            // 
            // cbxCaFieldLength
            // 
            this.cbxCaFieldLength.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxCaFieldLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbxCaFieldLength.FormattingEnabled = true;
            this.cbxCaFieldLength.Location = new System.Drawing.Point(120, 95);
            this.cbxCaFieldLength.Name = "cbxCaFieldLength";
            this.cbxCaFieldLength.Size = new System.Drawing.Size(80, 21);
            this.cbxCaFieldLength.TabIndex = 7;
            // 
            // lblIoaFieldLength
            // 
            this.lblIoaFieldLength.AutoSize = true;
            this.lblIoaFieldLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblIoaFieldLength.Location = new System.Drawing.Point(15, 125);
            this.lblIoaFieldLength.Name = "lblIoaFieldLength";
            this.lblIoaFieldLength.Size = new System.Drawing.Size(89, 13);
            this.lblIoaFieldLength.TabIndex = 8;
            this.lblIoaFieldLength.Text = "IOA Field Length:";
            // 
            // cbxIoaFieldLength
            // 
            this.cbxIoaFieldLength.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxIoaFieldLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbxIoaFieldLength.FormattingEnabled = true;
            this.cbxIoaFieldLength.Location = new System.Drawing.Point(120, 122);
            this.cbxIoaFieldLength.Name = "cbxIoaFieldLength";
            this.cbxIoaFieldLength.Size = new System.Drawing.Size(150, 21);
            this.cbxIoaFieldLength.TabIndex = 9;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblDescription.Location = new System.Drawing.Point(15, 325);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(75, 13);
            this.lblDescription.TabIndex = 5;
            this.lblDescription.Text = "Description:";
            // 
            // txtDescription
            // 
            this.txtDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtDescription.Location = new System.Drawing.Point(15, 345);
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
            this.btnOk.Location = new System.Drawing.Point(210, 440);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(290, 440);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // ctlDeviceDesign
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.gbProtocol);
            this.Controls.Add(this.gbConnection);
            this.Controls.Add(this.txtDeviceName);
            this.Controls.Add(this.lblDeviceName);
            this.Name = "ctlDeviceDesign";
            this.Size = new System.Drawing.Size(380, 480);
            this.gbConnection.ResumeLayout(false);
            this.gbConnection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).EndInit();
            this.gbProtocol.ResumeLayout(false);
            this.gbProtocol.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCommonAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOriginatorAddress)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}