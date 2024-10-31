
namespace SofarBMS.ui
{
    partial class DownloadControl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblUpgrade_05 = new System.Windows.Forms.Label();
            this.txtChip_code = new System.Windows.Forms.TextBox();
            this.cbbChip_role = new System.Windows.Forms.ComboBox();
            this.lblchip_code = new System.Windows.Forms.Label();
            this.lblchip_role = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnUpgrade = new System.Windows.Forms.Button();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBox1);
            this.groupBox1.Controls.Add(this.lblUpgrade_05);
            this.groupBox1.Controls.Add(this.txtChip_code);
            this.groupBox1.Controls.Add(this.cbbChip_role);
            this.groupBox1.Controls.Add(this.lblchip_code);
            this.groupBox1.Controls.Add(this.lblchip_role);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.btnUpgrade);
            this.groupBox1.Controls.Add(this.btnOpenFile);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(20, 20);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1192, 717);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "升级模块";
            // 
            // lblUpgrade_05
            // 
            this.lblUpgrade_05.AutoSize = true;
            this.lblUpgrade_05.Location = new System.Drawing.Point(426, 97);
            this.lblUpgrade_05.Name = "lblUpgrade_05";
            this.lblUpgrade_05.Size = new System.Drawing.Size(41, 12);
            this.lblUpgrade_05.TabIndex = 10;
            this.lblUpgrade_05.Text = "label3";
            // 
            // txtChip_code
            // 
            this.txtChip_code.Location = new System.Drawing.Point(292, 93);
            this.txtChip_code.Name = "txtChip_code";
            this.txtChip_code.Size = new System.Drawing.Size(100, 21);
            this.txtChip_code.TabIndex = 9;
            this.txtChip_code.Text = "X0";
            // 
            // cbbChip_role
            // 
            this.cbbChip_role.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbChip_role.FormattingEnabled = true;
            this.cbbChip_role.Items.AddRange(new object[] {
            "0：ARM",
            "1：DSP_M",
            "2：DSP_S",
            "3：BMS"});
            this.cbbChip_role.Location = new System.Drawing.Point(88, 93);
            this.cbbChip_role.Name = "cbbChip_role";
            this.cbbChip_role.Size = new System.Drawing.Size(121, 20);
            this.cbbChip_role.TabIndex = 8;
            // 
            // lblchip_code
            // 
            this.lblchip_code.AutoSize = true;
            this.lblchip_code.Location = new System.Drawing.Point(224, 97);
            this.lblchip_code.Name = "lblchip_code";
            this.lblchip_code.Size = new System.Drawing.Size(53, 12);
            this.lblchip_code.TabIndex = 7;
            this.lblchip_code.Text = "芯片编码";
            // 
            // lblchip_role
            // 
            this.lblchip_role.AutoSize = true;
            this.lblchip_role.Location = new System.Drawing.Point(20, 97);
            this.lblchip_role.Name = "lblchip_role";
            this.lblchip_role.Size = new System.Drawing.Size(53, 12);
            this.lblchip_role.TabIndex = 6;
            this.lblchip_role.Text = "芯片角色";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(90, 61);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(983, 23);
            this.progressBar1.TabIndex = 5;
            // 
            // btnUpgrade
            // 
            this.btnUpgrade.Location = new System.Drawing.Point(1094, 61);
            this.btnUpgrade.Name = "btnUpgrade";
            this.btnUpgrade.Size = new System.Drawing.Size(75, 23);
            this.btnUpgrade.TabIndex = 4;
            this.btnUpgrade.Text = "确认升级";
            this.btnUpgrade.UseVisualStyleBackColor = true;
            this.btnUpgrade.Click += new System.EventHandler(this.btnUpgrade_Click);
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(1094, 31);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(75, 23);
            this.btnOpenFile.TabIndex = 3;
            this.btnOpenFile.Text = "浏览";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(90, 32);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(983, 21);
            this.textBox1.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "下载进度";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "文件路径";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(22, 132);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(1147, 568);
            this.listBox1.TabIndex = 11;
            // 
            // DownloadControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "DownloadControl";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.Size = new System.Drawing.Size(1232, 757);
            this.Load += new System.EventHandler(this.DownloadControl_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnUpgrade;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblchip_code;
        private System.Windows.Forms.Label lblchip_role;
        private System.Windows.Forms.TextBox txtChip_code;
        private System.Windows.Forms.ComboBox cbbChip_role;
        private System.Windows.Forms.Label lblUpgrade_05;
        private System.Windows.Forms.ListBox listBox1;
    }
}
