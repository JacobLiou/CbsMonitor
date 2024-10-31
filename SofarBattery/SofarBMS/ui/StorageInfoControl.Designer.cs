namespace SofarBMS.UI
{
    partial class StorageInfoControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvStorageInfo = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnExport = new System.Windows.Forms.Button();
            this.btn_01 = new System.Windows.Forms.Button();
            this.btn_03 = new System.Windows.Forms.Button();
            this.btn_04 = new System.Windows.Forms.Button();
            this.btn_02 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStorageInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvStorageInfo
            // 
            this.dgvStorageInfo.AllowUserToAddRows = false;
            this.dgvStorageInfo.AllowUserToDeleteRows = false;
            this.dgvStorageInfo.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgvStorageInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStorageInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvStorageInfo.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.dgvStorageInfo.Location = new System.Drawing.Point(0, 0);
            this.dgvStorageInfo.Margin = new System.Windows.Forms.Padding(15, 16, 15, 16);
            this.dgvStorageInfo.Name = "dgvStorageInfo";
            this.dgvStorageInfo.ReadOnly = true;
            this.dgvStorageInfo.RowHeadersWidth = 51;
            this.dgvStorageInfo.RowTemplate.Height = 23;
            this.dgvStorageInfo.Size = new System.Drawing.Size(1370, 578);
            this.dgvStorageInfo.TabIndex = 0;
            this.dgvStorageInfo.RowStateChanged += new System.Windows.Forms.DataGridViewRowStateChangedEventHandler(this.dataGridView1_RowStateChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.splitContainer1.Panel1.Controls.Add(this.btnExport);
            this.splitContainer1.Panel1.Controls.Add(this.btn_01);
            this.splitContainer1.Panel1.Controls.Add(this.btn_03);
            this.splitContainer1.Panel1.Controls.Add(this.btn_02);
            this.splitContainer1.Panel1.Controls.Add(this.btn_04);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.splitContainer1.Panel2.Controls.Add(this.dgvStorageInfo);
            this.splitContainer1.Size = new System.Drawing.Size(1370, 630);
            this.splitContainer1.SplitterDistance = 49;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 2;
            // 
            // btnExport
            // 
            this.btnExport.FlatAppearance.BorderSize = 0;
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnExport.ForeColor = System.Drawing.Color.Black;
            this.btnExport.Image = global::SofarBMS.Properties.Resources.daochu;
            this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.Location = new System.Drawing.Point(948, 8);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(130, 32);
            this.btnExport.TabIndex = 12;
            this.btnExport.Text = "导出";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btn_01
            // 
            this.btn_01.FlatAppearance.BorderSize = 0;
            this.btn_01.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn_01.ForeColor = System.Drawing.Color.Black;
            this.btn_01.Image = global::SofarBMS.Properties.Resources.zhongzhi;
            this.btn_01.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_01.Location = new System.Drawing.Point(63, 8);
            this.btn_01.Name = "btn_01";
            this.btn_01.Size = new System.Drawing.Size(130, 32);
            this.btn_01.TabIndex = 8;
            this.btn_01.Text = "重置";
            this.btn_01.UseVisualStyleBackColor = true;
            this.btn_01.Click += new System.EventHandler(this.btn_01_Click);
            // 
            // btn_03
            // 
            this.btn_03.FlatAppearance.BorderSize = 0;
            this.btn_03.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn_03.ForeColor = System.Drawing.Color.Black;
            this.btn_03.Image = global::SofarBMS.Properties.Resources.zanting;
            this.btn_03.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_03.Location = new System.Drawing.Point(500, 8);
            this.btn_03.Name = "btn_03";
            this.btn_03.Size = new System.Drawing.Size(130, 32);
            this.btn_03.TabIndex = 11;
            this.btn_03.Text = "停止";
            this.btn_03.UseVisualStyleBackColor = true;
            this.btn_03.Click += new System.EventHandler(this.btn_Stop_Click);
            // 
            // btn_04
            // 
            this.btn_04.FlatAppearance.BorderSize = 0;
            this.btn_04.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn_04.ForeColor = System.Drawing.Color.Black;
            this.btn_04.Image = global::SofarBMS.Properties.Resources.qingkong;
            this.btn_04.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_04.Location = new System.Drawing.Point(724, 8);
            this.btn_04.Name = "btn_04";
            this.btn_04.Size = new System.Drawing.Size(130, 32);
            this.btn_04.TabIndex = 10;
            this.btn_04.Text = "清空";
            this.btn_04.UseVisualStyleBackColor = true;
            this.btn_04.Click += new System.EventHandler(this.btn_AA_Click);
            // 
            // btn_02
            // 
            this.btn_02.FlatAppearance.BorderSize = 0;
            this.btn_02.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn_02.ForeColor = System.Drawing.Color.Black;
            this.btn_02.Image = global::SofarBMS.Properties.Resources.duqushujuku;
            this.btn_02.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_02.Location = new System.Drawing.Point(276, 8);
            this.btn_02.Name = "btn_02";
            this.btn_02.Size = new System.Drawing.Size(130, 32);
            this.btn_02.TabIndex = 9;
            this.btn_02.Text = "读取";
            this.btn_02.UseVisualStyleBackColor = true;
            this.btn_02.Click += new System.EventHandler(this.btn_02_Click);
            // 
            // StorageInfoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "StorageInfoControl";
            this.Size = new System.Drawing.Size(1370, 630);
            this.Load += new System.EventHandler(this.StorageInfoControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStorageInfo)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvStorageInfo;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btn_03;
        private System.Windows.Forms.Button btn_04;
        private System.Windows.Forms.Button btn_02;
        private System.Windows.Forms.Button btn_01;
        private System.Windows.Forms.Button btnExport;
    }
}
