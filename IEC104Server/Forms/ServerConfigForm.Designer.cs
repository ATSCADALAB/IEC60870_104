// File: Forms/ServerConfigForm.Designer.cs
namespace IEC60870ServerWinForm.Forms
{
    partial class ServerConfigForm
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
            this.components = new System.ComponentModel.Container();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBoxNetwork = new System.Windows.Forms.GroupBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtIPAddress = new System.Windows.Forms.TextBox();
            this.lblIPAddress = new System.Windows.Forms.Label();
            this.groupBoxProtocol = new System.Windows.Forms.GroupBox();
            this.txtIoaLength = new System.Windows.Forms.TextBox();
            this.lblIoaLength = new System.Windows.Forms.Label();
            this.txtCaLength = new System.Windows.Forms.TextBox();
            this.lblCaLength = new System.Windows.Forms.Label();
            this.txtCotLength = new System.Windows.Forms.TextBox();
            this.lblCotLength = new System.Windows.Forms.Label();
            this.txtCommonAddress = new System.Windows.Forms.TextBox();
            this.lblCommonAddress = new System.Windows.Forms.Label();
            this.groupBoxTimeouts = new System.Windows.Forms.GroupBox();
            this.lblSeconds3 = new System.Windows.Forms.Label();
            this.lblSeconds2 = new System.Windows.Forms.Label();
            this.lblSeconds1 = new System.Windows.Forms.Label();
            this.lblSeconds0 = new System.Windows.Forms.Label();
            this.txtTimeoutT3 = new System.Windows.Forms.TextBox();
            this.lblTimeoutT3 = new System.Windows.Forms.Label();
            this.txtTimeoutT2 = new System.Windows.Forms.TextBox();
            this.lblTimeoutT2 = new System.Windows.Forms.Label();
            this.txtTimeoutT1 = new System.Windows.Forms.TextBox();
            this.lblTimeoutT1 = new System.Windows.Forms.Label();
            this.txtTimeoutT0 = new System.Windows.Forms.TextBox();
            this.lblTimeoutT0 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBoxNetwork.SuspendLayout();
            this.groupBoxProtocol.SuspendLayout();
            this.groupBoxTimeouts.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(217, 335);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(298, 335);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // groupBoxNetwork
            // 
            this.groupBoxNetwork.Controls.Add(this.txtPort);
            this.groupBoxNetwork.Controls.Add(this.lblPort);
            this.groupBoxNetwork.Controls.Add(this.txtIPAddress);
            this.groupBoxNetwork.Controls.Add(this.lblIPAddress);
            this.groupBoxNetwork.Location = new System.Drawing.Point(12, 12);
            this.groupBoxNetwork.Name = "groupBoxNetwork";
            this.groupBoxNetwork.Size = new System.Drawing.Size(361, 80);
            this.groupBoxNetwork.TabIndex = 0;
            this.groupBoxNetwork.TabStop = false;
            this.groupBoxNetwork.Text = "Network Settings";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(145, 48);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(100, 20);
            this.txtPort.TabIndex = 1;
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(16, 51);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(29, 13);
            this.lblPort.TabIndex = 2;
            this.lblPort.Text = "Port:";
            // 
            // txtIPAddress
            // 
            this.txtIPAddress.Location = new System.Drawing.Point(145, 22);
            this.txtIPAddress.Name = "txtIPAddress";
            this.txtIPAddress.Size = new System.Drawing.Size(200, 20);
            this.txtIPAddress.TabIndex = 0;
            // 
            // lblIPAddress
            // 
            this.lblIPAddress.AutoSize = true;
            this.lblIPAddress.Location = new System.Drawing.Point(16, 25);
            this.lblIPAddress.Name = "lblIPAddress";
            this.lblIPAddress.Size = new System.Drawing.Size(61, 13);
            this.lblIPAddress.TabIndex = 0;
            this.lblIPAddress.Text = "IP Address:";
            // 
            // groupBoxProtocol
            // 
            this.groupBoxProtocol.Controls.Add(this.txtIoaLength);
            this.groupBoxProtocol.Controls.Add(this.lblIoaLength);
            this.groupBoxProtocol.Controls.Add(this.txtCaLength);
            this.groupBoxProtocol.Controls.Add(this.lblCaLength);
            this.groupBoxProtocol.Controls.Add(this.txtCotLength);
            this.groupBoxProtocol.Controls.Add(this.lblCotLength);
            this.groupBoxProtocol.Controls.Add(this.txtCommonAddress);
            this.groupBoxProtocol.Controls.Add(this.lblCommonAddress);
            this.groupBoxProtocol.Location = new System.Drawing.Point(12, 98);
            this.groupBoxProtocol.Name = "groupBoxProtocol";
            this.groupBoxProtocol.Size = new System.Drawing.Size(361, 130);
            this.groupBoxProtocol.TabIndex = 1;
            this.groupBoxProtocol.TabStop = false;
            this.groupBoxProtocol.Text = "Protocol Parameters";
            // 
            // txtIoaLength
            // 
            this.txtIoaLength.Location = new System.Drawing.Point(145, 99);
            this.txtIoaLength.Name = "txtIoaLength";
            this.txtIoaLength.Size = new System.Drawing.Size(100, 20);
            this.txtIoaLength.TabIndex = 3;
            // 
            // lblIoaLength
            // 
            this.lblIoaLength.AutoSize = true;
            this.lblIoaLength.Location = new System.Drawing.Point(16, 102);
            this.lblIoaLength.Name = "lblIoaLength";
            this.lblIoaLength.Size = new System.Drawing.Size(92, 13);
            this.lblIoaLength.TabIndex = 6;
            this.lblIoaLength.Text = "IOA Field Length:";
            // 
            // txtCaLength
            // 
            this.txtCaLength.Location = new System.Drawing.Point(145, 73);
            this.txtCaLength.Name = "txtCaLength";
            this.txtCaLength.Size = new System.Drawing.Size(100, 20);
            this.txtCaLength.TabIndex = 2;
            // 
            // lblCaLength
            // 
            this.lblCaLength.AutoSize = true;
            this.lblCaLength.Location = new System.Drawing.Point(16, 76);
            this.lblCaLength.Name = "lblCaLength";
            this.lblCaLength.Size = new System.Drawing.Size(86, 13);
            this.lblCaLength.TabIndex = 4;
            this.lblCaLength.Text = "CA Field Length:";
            // 
            // txtCotLength
            // 
            this.txtCotLength.Location = new System.Drawing.Point(145, 47);
            this.txtCotLength.Name = "txtCotLength";
            this.txtCotLength.Size = new System.Drawing.Size(100, 20);
            this.txtCotLength.TabIndex = 1;
            // 
            // lblCotLength
            // 
            this.lblCotLength.AutoSize = true;
            this.lblCotLength.Location = new System.Drawing.Point(16, 50);
            this.lblCotLength.Name = "lblCotLength";
            this.lblCotLength.Size = new System.Drawing.Size(94, 13);
            this.lblCotLength.TabIndex = 2;
            this.lblCotLength.Text = "COT Field Length:";
            // 
            // txtCommonAddress
            // 
            this.txtCommonAddress.Location = new System.Drawing.Point(145, 21);
            this.txtCommonAddress.Name = "txtCommonAddress";
            this.txtCommonAddress.Size = new System.Drawing.Size(100, 20);
            this.txtCommonAddress.TabIndex = 0;
            // 
            // lblCommonAddress
            // 
            this.lblCommonAddress.AutoSize = true;
            this.lblCommonAddress.Location = new System.Drawing.Point(16, 24);
            this.lblCommonAddress.Name = "lblCommonAddress";
            this.lblCommonAddress.Size = new System.Drawing.Size(93, 13);
            this.lblCommonAddress.TabIndex = 0;
            this.lblCommonAddress.Text = "Common Address:";
            // 
            // groupBoxTimeouts
            // 
            this.groupBoxTimeouts.Controls.Add(this.lblSeconds3);
            this.groupBoxTimeouts.Controls.Add(this.lblSeconds2);
            this.groupBoxTimeouts.Controls.Add(this.lblSeconds1);
            this.groupBoxTimeouts.Controls.Add(this.lblSeconds0);
            this.groupBoxTimeouts.Controls.Add(this.txtTimeoutT3);
            this.groupBoxTimeouts.Controls.Add(this.lblTimeoutT3);
            this.groupBoxTimeouts.Controls.Add(this.txtTimeoutT2);
            this.groupBoxTimeouts.Controls.Add(this.lblTimeoutT2);
            this.groupBoxTimeouts.Controls.Add(this.txtTimeoutT1);
            this.groupBoxTimeouts.Controls.Add(this.lblTimeoutT1);
            this.groupBoxTimeouts.Controls.Add(this.txtTimeoutT0);
            this.groupBoxTimeouts.Controls.Add(this.lblTimeoutT0);
            this.groupBoxTimeouts.Location = new System.Drawing.Point(12, 234);
            this.groupBoxTimeouts.Name = "groupBoxTimeouts";
            this.groupBoxTimeouts.Size = new System.Drawing.Size(361, 88);
            this.groupBoxTimeouts.TabIndex = 2;
            this.groupBoxTimeouts.TabStop = false;
            this.groupBoxTimeouts.Text = "Timeouts";
            // 
            // lblSeconds3
            // 
            this.lblSeconds3.AutoSize = true;
            this.lblSeconds3.Location = new System.Drawing.Point(323, 51);
            this.lblSeconds3.Name = "lblSeconds3";
            this.lblSeconds3.Size = new System.Drawing.Size(12, 13);
            this.lblSeconds3.TabIndex = 11;
            this.lblSeconds3.Text = "s";
            // 
            // lblSeconds2
            // 
            this.lblSeconds2.AutoSize = true;
            this.lblSeconds2.Location = new System.Drawing.Point(159, 51);
            this.lblSeconds2.Name = "lblSeconds2";
            this.lblSeconds2.Size = new System.Drawing.Size(12, 13);
            this.lblSeconds2.TabIndex = 10;
            this.lblSeconds2.Text = "s";
            // 
            // lblSeconds1
            // 
            this.lblSeconds1.AutoSize = true;
            this.lblSeconds1.Location = new System.Drawing.Point(323, 25);
            this.lblSeconds1.Name = "lblSeconds1";
            this.lblSeconds1.Size = new System.Drawing.Size(12, 13);
            this.lblSeconds1.TabIndex = 9;
            this.lblSeconds1.Text = "s";
            // 
            // lblSeconds0
            // 
            this.lblSeconds0.AutoSize = true;
            this.lblSeconds0.Location = new System.Drawing.Point(159, 25);
            this.lblSeconds0.Name = "lblSeconds0";
            this.lblSeconds0.Size = new System.Drawing.Size(12, 13);
            this.lblSeconds0.TabIndex = 8;
            this.lblSeconds0.Text = "s";
            // 
            // txtTimeoutT3
            // 
            this.txtTimeoutT3.Location = new System.Drawing.Point(267, 48);
            this.txtTimeoutT3.Name = "txtTimeoutT3";
            this.txtTimeoutT3.Size = new System.Drawing.Size(50, 20);
            this.txtTimeoutT3.TabIndex = 3;
            // 
            // lblTimeoutT3
            // 
            this.lblTimeoutT3.AutoSize = true;
            this.lblTimeoutT3.Location = new System.Drawing.Point(188, 51);
            this.lblTimeoutT3.Name = "lblTimeoutT3";
            this.lblTimeoutT3.Size = new System.Drawing.Size(64, 13);
            this.lblTimeoutT3.TabIndex = 6;
            this.lblTimeoutT3.Text = "Keep-alive (T3):";
            // 
            // txtTimeoutT2
            // 
            this.txtTimeoutT2.Location = new System.Drawing.Point(103, 48);
            this.txtTimeoutT2.Name = "txtTimeoutT2";
            this.txtTimeoutT2.Size = new System.Drawing.Size(50, 20);
            this.txtTimeoutT2.TabIndex = 2;
            // 
            // lblTimeoutT2
            // 
            this.lblTimeoutT2.AutoSize = true;
            this.lblTimeoutT2.Location = new System.Drawing.Point(16, 51);
            this.lblTimeoutT2.Name = "lblTimeoutT2";
            this.lblTimeoutT2.Size = new System.Drawing.Size(81, 13);
            this.lblTimeoutT2.TabIndex = 4;
            this.lblTimeoutT2.Text = "S-Frame Ack (T2):";
            // 
            // txtTimeoutT1
            // 
            this.txtTimeoutT1.Location = new System.Drawing.Point(267, 22);
            this.txtTimeoutT1.Name = "txtTimeoutT1";
            this.txtTimeoutT1.Size = new System.Drawing.Size(50, 20);
            this.txtTimeoutT1.TabIndex = 1;
            // 
            // lblTimeoutT1
            // 
            this.lblTimeoutT1.AutoSize = true;
            this.lblTimeoutT1.Location = new System.Drawing.Point(188, 25);
            this.lblTimeoutT1.Name = "lblTimeoutT1";
            this.lblTimeoutT1.Size = new System.Drawing.Size(77, 13);
            this.lblTimeoutT1.TabIndex = 2;
            this.lblTimeoutT1.Text = "Response (T1):";
            // 
            // txtTimeoutT0
            // 
            this.txtTimeoutT0.Location = new System.Drawing.Point(103, 22);
            this.txtTimeoutT0.Name = "txtTimeoutT0";
            this.txtTimeoutT0.Size = new System.Drawing.Size(50, 20);
            this.txtTimeoutT0.TabIndex = 0;
            // 
            // lblTimeoutT0
            // 
            this.lblTimeoutT0.AutoSize = true;
            this.lblTimeoutT0.Location = new System.Drawing.Point(16, 25);
            this.lblTimeoutT0.Name = "lblTimeoutT0";
            this.lblTimeoutT0.Size = new System.Drawing.Size(81, 13);
            this.lblTimeoutT0.TabIndex = 0;
            this.lblTimeoutT0.Text = "Connection (T0):";
            // 
            // ServerConfigForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(384, 369);
            this.Controls.Add(this.groupBoxTimeouts);
            this.Controls.Add(this.groupBoxProtocol);
            this.Controls.Add(this.groupBoxNetwork);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ServerConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Server Configuration";
            this.Load += new System.EventHandler(this.ServerConfigForm_Load);
            this.groupBoxNetwork.ResumeLayout(false);
            this.groupBoxNetwork.PerformLayout();
            this.groupBoxProtocol.ResumeLayout(false);
            this.groupBoxProtocol.PerformLayout();
            this.groupBoxTimeouts.ResumeLayout(false);
            this.groupBoxTimeouts.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBoxNetwork;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtIPAddress;
        private System.Windows.Forms.Label lblIPAddress;
        private System.Windows.Forms.GroupBox groupBoxProtocol;
        private System.Windows.Forms.TextBox txtCommonAddress;
        private System.Windows.Forms.Label lblCommonAddress;
        private System.Windows.Forms.TextBox txtCotLength;
        private System.Windows.Forms.Label lblCotLength;
        private System.Windows.Forms.TextBox txtCaLength;
        private System.Windows.Forms.Label lblCaLength;
        private System.Windows.Forms.TextBox txtIoaLength;
        private System.Windows.Forms.Label lblIoaLength;
        private System.Windows.Forms.GroupBox groupBoxTimeouts;
        private System.Windows.Forms.TextBox txtTimeoutT0;
        private System.Windows.Forms.Label lblTimeoutT0;
        private System.Windows.Forms.TextBox txtTimeoutT3;
        private System.Windows.Forms.Label lblTimeoutT3;
        private System.Windows.Forms.TextBox txtTimeoutT2;
        private System.Windows.Forms.Label lblTimeoutT2;
        private System.Windows.Forms.TextBox txtTimeoutT1;
        private System.Windows.Forms.Label lblTimeoutT1;
        private System.Windows.Forms.Label lblSeconds0;
        private System.Windows.Forms.Label lblSeconds3;
        private System.Windows.Forms.Label lblSeconds2;
        private System.Windows.Forms.Label lblSeconds1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}