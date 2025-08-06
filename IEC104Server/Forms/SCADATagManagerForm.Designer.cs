namespace IEC60870ServerWinForm.Forms
{
    partial class SCADATagManagerForm
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
            this.components = new System.ComponentModel.Container();
            this.grpControls = new System.Windows.Forms.GroupBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.lblFilter = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnAutoRefresh = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnTestAll = new System.Windows.Forms.Button();
            this.grpTagList = new System.Windows.Forms.GroupBox();
            this.dgvTags = new System.Windows.Forms.DataGridView();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.grpControls.SuspendLayout();
            this.grpTagList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTags)).BeginInit();
            this.SuspendLayout();
            //
            // grpControls
            //
            this.grpControls.Controls.Add(this.lblStatus);
            this.grpControls.Controls.Add(this.txtFilter);
            this.grpControls.Controls.Add(this.lblFilter);
            this.grpControls.Controls.Add(this.btnExport);
            this.grpControls.Controls.Add(this.btnAutoRefresh);
            this.grpControls.Controls.Add(this.btnRefresh);
            this.grpControls.Controls.Add(this.btnTestAll);
            this.grpControls.Location = new System.Drawing.Point(12, 12);
            this.grpControls.Name = "grpControls";
            this.grpControls.Size = new System.Drawing.Size(960, 80);
            this.grpControls.TabIndex = 0;
            this.grpControls.TabStop = false;
            this.grpControls.Text = "Controls";
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.Blue;
            this.lblStatus.Location = new System.Drawing.Point(720, 25);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(37, 13);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Ready";
            this.toolTip1.SetToolTip(this.lblStatus, "Current status of SCADA tag operations");
            //
            // txtFilter
            //
            this.txtFilter.Location = new System.Drawing.Point(495, 22);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(200, 20);
            this.txtFilter.TabIndex = 5;
            this.toolTip1.SetToolTip(this.txtFilter, "Filter tags by IOA, Name, or Tag Path");
            this.txtFilter.TextChanged += new System.EventHandler(this.TxtFilter_TextChanged);
            //
            // lblFilter
            //
            this.lblFilter.AutoSize = true;
            this.lblFilter.Location = new System.Drawing.Point(450, 25);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new System.Drawing.Size(32, 13);
            this.lblFilter.TabIndex = 4;
            this.lblFilter.Text = "Filter:";
            //
            // btnExport
            //
            this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.Location = new System.Drawing.Point(340, 20);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(80, 30);
            this.btnExport.TabIndex = 3;
            this.btnExport.Text = "Export";
            this.btnExport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.btnExport, "Export tag data to CSV file");
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.BtnExport_Click);
            //
            // btnAutoRefresh
            //
            this.btnAutoRefresh.Location = new System.Drawing.Point(210, 20);
            this.btnAutoRefresh.Name = "btnAutoRefresh";
            this.btnAutoRefresh.Size = new System.Drawing.Size(120, 30);
            this.btnAutoRefresh.TabIndex = 2;
            this.btnAutoRefresh.Text = "Auto Refresh: OFF";
            this.toolTip1.SetToolTip(this.btnAutoRefresh, "Toggle automatic refresh every 2 seconds");
            this.btnAutoRefresh.UseVisualStyleBackColor = true;
            this.btnAutoRefresh.Click += new System.EventHandler(this.BtnAutoRefresh_Click);
            //
            // btnRefresh
            //
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRefresh.Location = new System.Drawing.Point(120, 20);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(80, 30);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.btnRefresh, "Refresh tag data manually");
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            //
            // btnTestAll
            //
            this.btnTestAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTestAll.Location = new System.Drawing.Point(10, 20);
            this.btnTestAll.Name = "btnTestAll";
            this.btnTestAll.Size = new System.Drawing.Size(100, 30);
            this.btnTestAll.TabIndex = 0;
            this.btnTestAll.Text = "Test All Tags";
            this.btnTestAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.btnTestAll, "Test connection to all SCADA tags");
            this.btnTestAll.UseVisualStyleBackColor = true;
            this.btnTestAll.Click += new System.EventHandler(this.BtnTestAll_Click);
            //
            // grpTagList
            //
            this.grpTagList.Controls.Add(this.dgvTags);
            this.grpTagList.Location = new System.Drawing.Point(12, 100);
            this.grpTagList.Name = "grpTagList";
            this.grpTagList.Size = new System.Drawing.Size(960, 450);
            this.grpTagList.TabIndex = 1;
            this.grpTagList.TabStop = false;
            this.grpTagList.Text = "SCADA Tags";
            //
            // dgvTags
            //
            this.dgvTags.AllowUserToAddRows = false;
            this.dgvTags.AllowUserToDeleteRows = false;
            this.dgvTags.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTags.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvTags.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTags.Location = new System.Drawing.Point(10, 20);
            this.dgvTags.MultiSelect = false;
            this.dgvTags.Name = "dgvTags";
            this.dgvTags.ReadOnly = true;
            this.dgvTags.RowHeadersVisible = false;
            this.dgvTags.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTags.Size = new System.Drawing.Size(940, 420);
            this.dgvTags.TabIndex = 0;
            this.toolTip1.SetToolTip(this.dgvTags, "Double-click a row to view detailed tag information");
            //
            // SCADATagManagerForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 562);
            this.Controls.Add(this.grpTagList);
            this.Controls.Add(this.grpControls);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "SCADATagManagerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SCADA Tag Manager";
            this.grpControls.ResumeLayout(false);
            this.grpControls.PerformLayout();
            this.grpTagList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTags)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpControls;
        private System.Windows.Forms.GroupBox grpTagList;
        private System.Windows.Forms.DataGridView dgvTags;
        private System.Windows.Forms.Button btnTestAll;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnAutoRefresh;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Label lblFilter;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}