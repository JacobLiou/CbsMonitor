namespace SofarBMS.UI
{
    partial class CBSUpgradeControl
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
            this.lblUpgrade_07 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtFC = new System.Windows.Forms.TextBox();
            this.cbbChiprole_val = new System.Windows.Forms.ComboBox();
            this.txtSlaveAddress = new System.Windows.Forms.TextBox();
            this.cbbChipcode = new System.Windows.Forms.ComboBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblUpgrade_02 = new System.Windows.Forms.Label();
            this.cbbChiprole = new System.Windows.Forms.ComboBox();
            this.btnUpgrade_03 = new System.Windows.Forms.Button();
            this.lblUpgrade_01 = new System.Windows.Forms.Label();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.lblUpgrade_05 = new System.Windows.Forms.Label();
            this.btnUpgrade_04 = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.ckUpgrade_06 = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtFD = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblUpgrade_07
            // 
            this.lblUpgrade_07.AutoSize = true;
            this.lblUpgrade_07.Location = new System.Drawing.Point(59, 107);
            this.lblUpgrade_07.Name = "lblUpgrade_07";
            this.lblUpgrade_07.Size = new System.Drawing.Size(53, 12);
            this.lblUpgrade_07.TabIndex = 25;
            this.lblUpgrade_07.Text = "升级类型";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtFD);
            this.groupBox2.Controls.Add(this.txtFC);
            this.groupBox2.Controls.Add(this.cbbChiprole_val);
            this.groupBox2.Controls.Add(this.txtSlaveAddress);
            this.groupBox2.Controls.Add(this.cbbChipcode);
            this.groupBox2.Controls.Add(this.progressBar1);
            this.groupBox2.Controls.Add(this.lblUpgrade_02);
            this.groupBox2.Controls.Add(this.cbbChiprole);
            this.groupBox2.Controls.Add(this.btnUpgrade_03);
            this.groupBox2.Controls.Add(this.lblUpgrade_01);
            this.groupBox2.Controls.Add(this.txtPath);
            this.groupBox2.Controls.Add(this.lblUpgrade_05);
            this.groupBox2.Controls.Add(this.lblUpgrade_07);
            this.groupBox2.Controls.Add(this.btnUpgrade_04);
            this.groupBox2.Controls.Add(this.dateTimePicker1);
            this.groupBox2.Controls.Add(this.ckUpgrade_06);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1232, 181);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "升级固件";
            // 
            // txtFC
            // 
            this.txtFC.Location = new System.Drawing.Point(882, 103);
            this.txtFC.Name = "txtFC";
            this.txtFC.Size = new System.Drawing.Size(68, 21);
            this.txtFC.TabIndex = 34;
            this.txtFC.Text = "20";
            // 
            // cbbChiprole_val
            // 
            this.cbbChiprole_val.FormattingEnabled = true;
            this.cbbChiprole_val.Items.AddRange(new object[] {
            "0x24",
            "0x2D"});
            this.cbbChiprole_val.Location = new System.Drawing.Point(567, 103);
            this.cbbChiprole_val.Name = "cbbChiprole_val";
            this.cbbChiprole_val.Size = new System.Drawing.Size(100, 20);
            this.cbbChiprole_val.TabIndex = 33;
            this.cbbChiprole_val.SelectedIndexChanged += new System.EventHandler(this.cbbChiprole_val_SelectedIndexChanged);
            // 
            // txtSlaveAddress
            // 
            this.txtSlaveAddress.Location = new System.Drawing.Point(673, 103);
            this.txtSlaveAddress.Name = "txtSlaveAddress";
            this.txtSlaveAddress.ReadOnly = true;
            this.txtSlaveAddress.Size = new System.Drawing.Size(75, 21);
            this.txtSlaveAddress.TabIndex = 32;
            // 
            // cbbChipcode
            // 
            this.cbbChipcode.FormattingEnabled = true;
            this.cbbChipcode.Items.AddRange(new object[] {
            "E0",
            "S3",
            "N2"});
            this.cbbChipcode.Location = new System.Drawing.Point(754, 103);
            this.cbbChipcode.Name = "cbbChipcode";
            this.cbbChipcode.Size = new System.Drawing.Size(100, 20);
            this.cbbChipcode.TabIndex = 30;
            this.cbbChipcode.SelectedIndexChanged += new System.EventHandler(this.cbbChipcode_SelectedIndexChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(146, 62);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(875, 24);
            this.progressBar1.TabIndex = 3;
            // 
            // lblUpgrade_02
            // 
            this.lblUpgrade_02.AutoSize = true;
            this.lblUpgrade_02.Location = new System.Drawing.Point(59, 68);
            this.lblUpgrade_02.Name = "lblUpgrade_02";
            this.lblUpgrade_02.Size = new System.Drawing.Size(53, 12);
            this.lblUpgrade_02.TabIndex = 24;
            this.lblUpgrade_02.Text = "升级进度";
            // 
            // cbbChiprole
            // 
            this.cbbChiprole.FormattingEnabled = true;
            this.cbbChiprole.Items.AddRange(new object[] {
            "BCU",
            "BMU"});
            this.cbbChiprole.Location = new System.Drawing.Point(440, 103);
            this.cbbChiprole.Name = "cbbChiprole";
            this.cbbChiprole.Size = new System.Drawing.Size(121, 20);
            this.cbbChiprole.TabIndex = 29;
            this.cbbChiprole.SelectedIndexChanged += new System.EventHandler(this.cbbChiprole_SelectedIndexChanged);
            // 
            // btnUpgrade_03
            // 
            this.btnUpgrade_03.Location = new System.Drawing.Point(1028, 20);
            this.btnUpgrade_03.Name = "btnUpgrade_03";
            this.btnUpgrade_03.Size = new System.Drawing.Size(75, 23);
            this.btnUpgrade_03.TabIndex = 21;
            this.btnUpgrade_03.Text = "导入文件";
            this.btnUpgrade_03.UseVisualStyleBackColor = true;
            this.btnUpgrade_03.Click += new System.EventHandler(this.btnUpgrade_03_Click);
            // 
            // lblUpgrade_01
            // 
            this.lblUpgrade_01.AutoSize = true;
            this.lblUpgrade_01.Location = new System.Drawing.Point(59, 25);
            this.lblUpgrade_01.Name = "lblUpgrade_01";
            this.lblUpgrade_01.Size = new System.Drawing.Size(53, 12);
            this.lblUpgrade_01.TabIndex = 19;
            this.lblUpgrade_01.Text = "文件路径";
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(146, 21);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(875, 21);
            this.txtPath.TabIndex = 20;
            // 
            // lblUpgrade_05
            // 
            this.lblUpgrade_05.AutoSize = true;
            this.lblUpgrade_05.Location = new System.Drawing.Point(144, 136);
            this.lblUpgrade_05.Name = "lblUpgrade_05";
            this.lblUpgrade_05.Size = new System.Drawing.Size(29, 12);
            this.lblUpgrade_05.TabIndex = 6;
            this.lblUpgrade_05.Text = "Tips";
            // 
            // btnUpgrade_04
            // 
            this.btnUpgrade_04.BackColor = System.Drawing.SystemColors.Control;
            this.btnUpgrade_04.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnUpgrade_04.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnUpgrade_04.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnUpgrade_04.Location = new System.Drawing.Point(1027, 60);
            this.btnUpgrade_04.Name = "btnUpgrade_04";
            this.btnUpgrade_04.Size = new System.Drawing.Size(75, 28);
            this.btnUpgrade_04.TabIndex = 5;
            this.btnUpgrade_04.Text = "启动升级";
            this.btnUpgrade_04.UseVisualStyleBackColor = false;
            this.btnUpgrade_04.Click += new System.EventHandler(this.btnUpgrade_04_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Checked = false;
            this.dateTimePicker1.CustomFormat = "yy-MM-dd HH:mm:ss";
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(146, 103);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.ShowUpDown = true;
            this.dateTimePicker1.Size = new System.Drawing.Size(165, 21);
            this.dateTimePicker1.TabIndex = 17;
            // 
            // ckUpgrade_06
            // 
            this.ckUpgrade_06.AutoSize = true;
            this.ckUpgrade_06.Location = new System.Drawing.Point(317, 105);
            this.ckUpgrade_06.Name = "ckUpgrade_06";
            this.ckUpgrade_06.Size = new System.Drawing.Size(96, 16);
            this.ckUpgrade_06.TabIndex = 18;
            this.ckUpgrade_06.Text = "开启定时升级";
            this.ckUpgrade_06.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listView1);
            this.groupBox1.Location = new System.Drawing.Point(0, 153);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1232, 604);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
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
            this.listView1.Size = new System.Drawing.Size(1226, 584);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "datetime";
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
            // txtFD
            // 
            this.txtFD.Location = new System.Drawing.Point(953, 102);
            this.txtFD.Name = "txtFD";
            this.txtFD.Size = new System.Drawing.Size(68, 21);
            this.txtFD.TabIndex = 35;
            this.txtFD.Text = "3";
            // 
            // CBSUpgradeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "CBSUpgradeControl";
            this.Size = new System.Drawing.Size(1232, 757);
            this.Load += new System.EventHandler(this.CBSUpgradeControl_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblUpgrade_07;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnUpgrade_03;
        private System.Windows.Forms.Label lblUpgrade_01;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label lblUpgrade_05;
        private System.Windows.Forms.Button btnUpgrade_04;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.CheckBox ckUpgrade_06;
        private System.Windows.Forms.Label lblUpgrade_02;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbbChipcode;
        private System.Windows.Forms.ComboBox cbbChiprole;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.TextBox txtSlaveAddress;
        private System.Windows.Forms.ComboBox cbbChiprole_val;
        private System.Windows.Forms.TextBox txtFC;
        private System.Windows.Forms.TextBox txtFD;
    }
}
