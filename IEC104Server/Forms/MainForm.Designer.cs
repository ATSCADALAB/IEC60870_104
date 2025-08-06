namespace IEC60870ServerWinForm.Forms
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configureServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblServerStatus = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.splitContainerTop = new System.Windows.Forms.SplitContainer();
            this.groupBoxDataPoints = new System.Windows.Forms.GroupBox();
            this.dgvDataPoints = new System.Windows.Forms.DataGridView();
            this.panelDataPointButtons = new System.Windows.Forms.Panel();
            this.btnSendSelected = new System.Windows.Forms.Button();
            this.btnDeletePoint = new System.Windows.Forms.Button();
            this.btnEditPoint = new System.Windows.Forms.Button();
            this.btnAddPoint = new System.Windows.Forms.Button();
            this.groupBoxClients = new System.Windows.Forms.GroupBox();
            this.lvConnectedClients = new System.Windows.Forms.ListView();
            this.columnHeaderIP = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderConnectedTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBoxLogs = new System.Windows.Forms.GroupBox();
            this.txtLogs = new System.Windows.Forms.TextBox();
            this.panelLogButtons = new System.Windows.Forms.Panel();
            this.btnClearLogs = new System.Windows.Forms.Button();
            this.menuStripMain.SuspendLayout();
            this.statusStripMain.SuspendLayout();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerTop)).BeginInit();
            this.splitContainerTop.Panel1.SuspendLayout();
            this.splitContainerTop.Panel2.SuspendLayout();
            this.splitContainerTop.SuspendLayout();
            this.groupBoxDataPoints.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataPoints)).BeginInit();
            this.panelDataPointButtons.SuspendLayout();
            this.groupBoxClients.SuspendLayout();
            this.groupBoxLogs.SuspendLayout();
            this.panelLogButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(784, 24);
            this.menuStripMain.TabIndex = 0;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            //
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configureServerToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // configureServerToolStripMenuItem
            // 
            this.configureServerToolStripMenuItem.Name = "configureServerToolStripMenuItem";
            this.configureServerToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.configureServerToolStripMenuItem.Text = "Configure Server...";
            this.configureServerToolStripMenuItem.Click += new System.EventHandler(this.configureServerToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.aboutToolStripMenuItem.Text = "About...";
            // 
            // statusStripMain
            // 
            this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStripMain.Location = new System.Drawing.Point(0, 539);
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.Size = new System.Drawing.Size(784, 22);
            this.statusStripMain.TabIndex = 1;
            this.statusStripMain.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(39, 17);
            this.lblStatus.Text = "Ready";
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.lblServerStatus);
            this.panelTop.Controls.Add(this.btnStop);
            this.panelTop.Controls.Add(this.btnStart);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 24);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(784, 40);
            this.panelTop.TabIndex = 2;
            // 
            // lblServerStatus
            // 
            this.lblServerStatus.AutoSize = true;
            this.lblServerStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblServerStatus.Location = new System.Drawing.Point(12, 12);
            this.lblServerStatus.Name = "lblServerStatus";
            this.lblServerStatus.Size = new System.Drawing.Size(117, 16);
            this.lblServerStatus.TabIndex = 2;
            this.lblServerStatus.Text = "Status: Stopped";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(220, 8);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(50, 23);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(164, 8);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(50, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 64);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.splitContainerTop);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.groupBoxLogs);
            this.splitContainerMain.Size = new System.Drawing.Size(784, 475);
            this.splitContainerMain.SplitterDistance = 280;
            this.splitContainerMain.TabIndex = 3;
            // 
            // splitContainerTop
            // 
            this.splitContainerTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerTop.Location = new System.Drawing.Point(0, 0);
            this.splitContainerTop.Name = "splitContainerTop";
            // 
            // splitContainerTop.Panel1
            // 
            this.splitContainerTop.Panel1.Controls.Add(this.groupBoxDataPoints);
            // 
            // splitContainerTop.Panel2
            // 
            this.splitContainerTop.Panel2.Controls.Add(this.groupBoxClients);
            this.splitContainerTop.Size = new System.Drawing.Size(784, 280);
            this.splitContainerTop.SplitterDistance = 500;
            this.splitContainerTop.TabIndex = 0;
            // 
            // groupBoxDataPoints
            // 
            this.groupBoxDataPoints.Controls.Add(this.dgvDataPoints);
            this.groupBoxDataPoints.Controls.Add(this.panelDataPointButtons);
            this.groupBoxDataPoints.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxDataPoints.Location = new System.Drawing.Point(0, 0);
            this.groupBoxDataPoints.Name = "groupBoxDataPoints";
            this.groupBoxDataPoints.Padding = new System.Windows.Forms.Padding(8);
            this.groupBoxDataPoints.Size = new System.Drawing.Size(500, 280);
            this.groupBoxDataPoints.TabIndex = 0;
            this.groupBoxDataPoints.TabStop = false;
            this.groupBoxDataPoints.Text = "Data Points";
            // 
            // dgvDataPoints
            // 
            this.dgvDataPoints.AllowUserToAddRows = false;
            this.dgvDataPoints.AllowUserToDeleteRows = false;
            this.dgvDataPoints.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDataPoints.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDataPoints.Location = new System.Drawing.Point(8, 21);
            this.dgvDataPoints.Name = "dgvDataPoints";
            this.dgvDataPoints.ReadOnly = true;
            this.dgvDataPoints.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDataPoints.Size = new System.Drawing.Size(484, 216);
            this.dgvDataPoints.TabIndex = 1;
            // 
            // panelDataPointButtons
            // 
            this.panelDataPointButtons.Controls.Add(this.btnSendSelected);
            this.panelDataPointButtons.Controls.Add(this.btnDeletePoint);
            this.panelDataPointButtons.Controls.Add(this.btnEditPoint);
            this.panelDataPointButtons.Controls.Add(this.btnAddPoint);
            this.panelDataPointButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelDataPointButtons.Location = new System.Drawing.Point(8, 237);
            this.panelDataPointButtons.Name = "panelDataPointButtons";
            this.panelDataPointButtons.Size = new System.Drawing.Size(484, 35);
            this.panelDataPointButtons.TabIndex = 0;
            // 
            // btnSendSelected
            // 
            this.btnSendSelected.Location = new System.Drawing.Point(246, 6);
            this.btnSendSelected.Name = "btnSendSelected";
            this.btnSendSelected.Size = new System.Drawing.Size(90, 23);
            this.btnSendSelected.TabIndex = 3;
            this.btnSendSelected.Text = "Send Selected";
            this.btnSendSelected.UseVisualStyleBackColor = true;
            this.btnSendSelected.Click += new System.EventHandler(this.btnSendSelected_Click);
            // 
            // btnDeletePoint
            // 
            this.btnDeletePoint.Location = new System.Drawing.Point(165, 6);
            this.btnDeletePoint.Name = "btnDeletePoint";
            this.btnDeletePoint.Size = new System.Drawing.Size(75, 23);
            this.btnDeletePoint.TabIndex = 2;
            this.btnDeletePoint.Text = "Delete";
            this.btnDeletePoint.UseVisualStyleBackColor = true;
            this.btnDeletePoint.Click += new System.EventHandler(this.btnDeletePoint_Click);
            // 
            // btnEditPoint
            // 
            this.btnEditPoint.Location = new System.Drawing.Point(84, 6);
            this.btnEditPoint.Name = "btnEditPoint";
            this.btnEditPoint.Size = new System.Drawing.Size(75, 23);
            this.btnEditPoint.TabIndex = 1;
            this.btnEditPoint.Text = "Edit";
            this.btnEditPoint.UseVisualStyleBackColor = true;
            this.btnEditPoint.Click += new System.EventHandler(this.btnEditPoint_Click);
            // 
            // btnAddPoint
            // 
            this.btnAddPoint.Location = new System.Drawing.Point(3, 6);
            this.btnAddPoint.Name = "btnAddPoint";
            this.btnAddPoint.Size = new System.Drawing.Size(75, 23);
            this.btnAddPoint.TabIndex = 0;
            this.btnAddPoint.Text = "Add";
            this.btnAddPoint.UseVisualStyleBackColor = true;
            this.btnAddPoint.Click += new System.EventHandler(this.btnAddPoint_Click);
            // 
            // groupBoxClients
            // 
            this.groupBoxClients.Controls.Add(this.lvConnectedClients);
            this.groupBoxClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxClients.Location = new System.Drawing.Point(0, 0);
            this.groupBoxClients.Name = "groupBoxClients";
            this.groupBoxClients.Padding = new System.Windows.Forms.Padding(8);
            this.groupBoxClients.Size = new System.Drawing.Size(280, 280);
            this.groupBoxClients.TabIndex = 0;
            this.groupBoxClients.TabStop = false;
            this.groupBoxClients.Text = "Connected Clients";
            // 
            // lvConnectedClients
            // 
            this.lvConnectedClients.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderIP,
            this.columnHeaderConnectedTime});
            this.lvConnectedClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvConnectedClients.HideSelection = false;
            this.lvConnectedClients.Location = new System.Drawing.Point(8, 21);
            this.lvConnectedClients.Name = "lvConnectedClients";
            this.lvConnectedClients.Size = new System.Drawing.Size(264, 251);
            this.lvConnectedClients.TabIndex = 0;
            this.lvConnectedClients.UseCompatibleStateImageBehavior = false;
            this.lvConnectedClients.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderIP
            // 
            this.columnHeaderIP.Text = "Client IP";
            this.columnHeaderIP.Width = 150;
            // 
            // columnHeaderConnectedTime
            // 
            this.columnHeaderConnectedTime.Text = "Connected Time";
            this.columnHeaderConnectedTime.Width = 110;
            // 
            // groupBoxLogs
            // 
            this.groupBoxLogs.Controls.Add(this.txtLogs);
            this.groupBoxLogs.Controls.Add(this.panelLogButtons);
            this.groupBoxLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxLogs.Location = new System.Drawing.Point(0, 0);
            this.groupBoxLogs.Name = "groupBoxLogs";
            this.groupBoxLogs.Padding = new System.Windows.Forms.Padding(8);
            this.groupBoxLogs.Size = new System.Drawing.Size(784, 191);
            this.groupBoxLogs.TabIndex = 0;
            this.groupBoxLogs.TabStop = false;
            this.groupBoxLogs.Text = "Server Logs";
            // 
            // txtLogs
            // 
            this.txtLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLogs.Location = new System.Drawing.Point(8, 21);
            this.txtLogs.Multiline = true;
            this.txtLogs.Name = "txtLogs";
            this.txtLogs.ReadOnly = true;
            this.txtLogs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLogs.Size = new System.Drawing.Size(768, 127);
            this.txtLogs.TabIndex = 0;
            // 
            // panelLogButtons
            // 
            this.panelLogButtons.Controls.Add(this.btnClearLogs);
            this.panelLogButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelLogButtons.Location = new System.Drawing.Point(8, 148);
            this.panelLogButtons.Name = "panelLogButtons";
            this.panelLogButtons.Size = new System.Drawing.Size(768, 35);
            this.panelLogButtons.TabIndex = 1;
            // 
            // btnClearLogs
            // 
            this.btnClearLogs.Location = new System.Drawing.Point(3, 6);
            this.btnClearLogs.Name = "btnClearLogs";
            this.btnClearLogs.Size = new System.Drawing.Size(75, 23);
            this.btnClearLogs.TabIndex = 0;
            this.btnClearLogs.Text = "Clear Logs";
            this.btnClearLogs.UseVisualStyleBackColor = true;
            //this.btnClearLogs.Click += new System.EventHandler(this.btnClearLogs_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.statusStripMain);
            this.Controls.Add(this.menuStripMain);
            this.MainMenuStrip = this.menuStripMain;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "MainForm";
            this.Text = "IEC 60870-5-104 Server";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.statusStripMain.ResumeLayout(false);
            this.statusStripMain.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.splitContainerTop.Panel1.ResumeLayout(false);
            this.splitContainerTop.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerTop)).EndInit();
            this.splitContainerTop.ResumeLayout(false);
            this.groupBoxDataPoints.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataPoints)).EndInit();
            this.panelDataPointButtons.ResumeLayout(false);
            this.groupBoxClients.ResumeLayout(false);
            this.groupBoxLogs.ResumeLayout(false);
            this.groupBoxLogs.PerformLayout();
            this.panelLogButtons.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.SplitContainer splitContainerTop;
        private System.Windows.Forms.GroupBox groupBoxDataPoints;
        private System.Windows.Forms.GroupBox groupBoxClients;
        private System.Windows.Forms.GroupBox groupBoxLogs;
        private System.Windows.Forms.TextBox txtLogs;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Panel panelDataPointButtons;
        private System.Windows.Forms.Button btnSendSelected;
        private System.Windows.Forms.Button btnDeletePoint;
        private System.Windows.Forms.Button btnEditPoint;
        private System.Windows.Forms.Button btnAddPoint;
        private System.Windows.Forms.DataGridView dgvDataPoints;
        private System.Windows.Forms.ListView lvConnectedClients;
        private System.Windows.Forms.ColumnHeader columnHeaderIP;
        private System.Windows.Forms.ColumnHeader columnHeaderConnectedTime;
        private System.Windows.Forms.Panel panelLogButtons;
        private System.Windows.Forms.Button btnClearLogs;
        private System.Windows.Forms.Label lblServerStatus;
        private System.Windows.Forms.ToolStripMenuItem configureServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
    }
}