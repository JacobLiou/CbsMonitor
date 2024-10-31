namespace SofarBMS.UI
{
    partial class UpgradeControl
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
            this.lblUpgrade_01 = new System.Windows.Forms.Label();
            this.lblUpgrade_02 = new System.Windows.Forms.Label();
            this.txtUpgradeFile = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnUpgrade_03 = new System.Windows.Forms.Button();
            this.btnUpgrade_04 = new System.Windows.Forms.Button();
            this.lblUpgrade_05 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listView1 = new SofarBMS.ListViewBuff();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cbbChip_role = new System.Windows.Forms.ComboBox();
            this.txtChip_code = new System.Windows.Forms.TextBox();
            this.lblUpgradeRole = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblUpgrade_01
            // 
            this.lblUpgrade_01.AutoSize = true;
            this.lblUpgrade_01.Location = new System.Drawing.Point(42, 25);
            this.lblUpgrade_01.Name = "lblUpgrade_01";
            this.lblUpgrade_01.Size = new System.Drawing.Size(53, 12);
            this.lblUpgrade_01.TabIndex = 0;
            this.lblUpgrade_01.Text = "文件路径";
            // 
            // lblUpgrade_02
            // 
            this.lblUpgrade_02.AutoSize = true;
            this.lblUpgrade_02.Location = new System.Drawing.Point(42, 63);
            this.lblUpgrade_02.Name = "lblUpgrade_02";
            this.lblUpgrade_02.Size = new System.Drawing.Size(53, 12);
            this.lblUpgrade_02.TabIndex = 1;
            this.lblUpgrade_02.Text = "下载进度";
            // 
            // txtUpgradeFile
            // 
            this.txtUpgradeFile.Location = new System.Drawing.Point(141, 21);
            this.txtUpgradeFile.Name = "txtUpgradeFile";
            this.txtUpgradeFile.Size = new System.Drawing.Size(874, 21);
            this.txtUpgradeFile.TabIndex = 2;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(141, 58);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(874, 23);
            this.progressBar1.TabIndex = 3;
            // 
            // btnUpgrade_03
            // 
            this.btnUpgrade_03.AutoSize = true;
            this.btnUpgrade_03.Location = new System.Drawing.Point(1041, 19);
            this.btnUpgrade_03.Name = "btnUpgrade_03";
            this.btnUpgrade_03.Size = new System.Drawing.Size(75, 25);
            this.btnUpgrade_03.TabIndex = 4;
            this.btnUpgrade_03.Text = "导入文件";
            this.btnUpgrade_03.UseVisualStyleBackColor = true;
            this.btnUpgrade_03.Click += new System.EventHandler(this.btnUpgrade_03_Click);
            // 
            // btnUpgrade_04
            // 
            this.btnUpgrade_04.AutoSize = true;
            this.btnUpgrade_04.Location = new System.Drawing.Point(1041, 57);
            this.btnUpgrade_04.Name = "btnUpgrade_04";
            this.btnUpgrade_04.Size = new System.Drawing.Size(75, 25);
            this.btnUpgrade_04.TabIndex = 5;
            this.btnUpgrade_04.Text = "启动升级";
            this.btnUpgrade_04.UseVisualStyleBackColor = true;
            this.btnUpgrade_04.Click += new System.EventHandler(this.btnUpgrade_04_Click);
            // 
            // lblUpgrade_05
            // 
            this.lblUpgrade_05.AutoSize = true;
            this.lblUpgrade_05.Location = new System.Drawing.Point(139, 131);
            this.lblUpgrade_05.Name = "lblUpgrade_05";
            this.lblUpgrade_05.Size = new System.Drawing.Size(29, 12);
            this.lblUpgrade_05.TabIndex = 6;
            this.lblUpgrade_05.Text = "Tips";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listView1);
            this.groupBox1.Location = new System.Drawing.Point(44, 157);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1072, 470);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "记录日志";
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(3, 17);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(1066, 450);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "DateTime";
            this.columnHeader1.Width = 200;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "ID";
            this.columnHeader2.Width = 200;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Data";
            this.columnHeader3.Width = 500;
            // 
            // cbbChip_role
            // 
            this.cbbChip_role.FormattingEnabled = true;
            this.cbbChip_role.Items.AddRange(new object[] {
            "ARM",
            "PCU",
            "DSP",
            "BMS",
            "BDU"});
            this.cbbChip_role.Location = new System.Drawing.Point(141, 97);
            this.cbbChip_role.Name = "cbbChip_role";
            this.cbbChip_role.Size = new System.Drawing.Size(121, 20);
            this.cbbChip_role.TabIndex = 8;
            this.cbbChip_role.SelectedIndexChanged += new System.EventHandler(this.cbbChip_role_SelectedIndexChanged);
            // 
            // txtChip_code
            // 
            this.txtChip_code.Location = new System.Drawing.Point(268, 97);
            this.txtChip_code.Name = "txtChip_code";
            this.txtChip_code.ReadOnly = true;
            this.txtChip_code.Size = new System.Drawing.Size(100, 21);
            this.txtChip_code.TabIndex = 9;
            // 
            // lblUpgradeRole
            // 
            this.lblUpgradeRole.AutoSize = true;
            this.lblUpgradeRole.Location = new System.Drawing.Point(45, 100);
            this.lblUpgradeRole.Name = "lblUpgradeRole";
            this.lblUpgradeRole.Size = new System.Drawing.Size(53, 12);
            this.lblUpgradeRole.TabIndex = 10;
            this.lblUpgradeRole.Text = "升级角色";
            // 
            // UpgradeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblUpgradeRole);
            this.Controls.Add(this.txtChip_code);
            this.Controls.Add(this.cbbChip_role);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblUpgrade_05);
            this.Controls.Add(this.btnUpgrade_04);
            this.Controls.Add(this.btnUpgrade_03);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.txtUpgradeFile);
            this.Controls.Add(this.lblUpgrade_02);
            this.Controls.Add(this.lblUpgrade_01);
            this.Name = "UpgradeControl";
            this.Size = new System.Drawing.Size(1370, 630);
            this.Load += new System.EventHandler(this.UpgradeControl_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblUpgrade_01;
        private System.Windows.Forms.Label lblUpgrade_02;
        private System.Windows.Forms.TextBox txtUpgradeFile;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnUpgrade_03;
        private System.Windows.Forms.Button btnUpgrade_04;
        private System.Windows.Forms.Label lblUpgrade_05;
        //private System.Windows.Forms.ListView listView1;
        private ListViewBuff listView1;//增加listview双缓存处理
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbbChip_role;
        private System.Windows.Forms.TextBox txtChip_code;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Label lblUpgradeRole;
    }
}
