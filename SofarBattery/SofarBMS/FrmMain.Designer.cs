
namespace SofarBMS
{
    partial class FrmMain
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.panel1 = new System.Windows.Forms.Panel();
            this.Menu = new System.Windows.Forms.MenuStrip();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnAlarmInfo = new System.Windows.Forms.Button();
            this.btnStratListen = new System.Windows.Forms.Button();
            this.btnClearInit = new System.Windows.Forms.Button();
            this.cbbIDP = new System.Windows.Forms.ComboBox();
            this.btnResetCAN = new System.Windows.Forms.Button();
            this.cbbBaud = new System.Windows.Forms.ComboBox();
            this.cbbID = new System.Windows.Forms.ComboBox();
            this.lblSp_01 = new System.Windows.Forms.Label();
            this.lblSp_03 = new System.Windows.Forms.Label();
            this.btnConnectionCAN = new System.Windows.Forms.Button();
            this.lblSp_02 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1364, 631);
            this.panel1.TabIndex = 27;
            // 
            // Menu
            // 
            this.Menu.AutoSize = false;
            this.Menu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.Menu.Location = new System.Drawing.Point(0, 0);
            this.Menu.Name = "Menu";
            this.Menu.Padding = new System.Windows.Forms.Padding(4, 1, 0, 1);
            this.Menu.Size = new System.Drawing.Size(1364, 30);
            this.Menu.TabIndex = 0;
            this.Menu.Text = "menuStrip1";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 699);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1364, 22);
            this.statusStrip1.TabIndex = 28;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(80, 17);
            this.toolStripStatusLabel1.Text = "错误码反馈值";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 30);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(1364, 669);
            this.splitContainer1.SplitterDistance = 34;
            this.splitContainer1.TabIndex = 29;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.panel2.Controls.Add(this.btnAlarmInfo);
            this.panel2.Controls.Add(this.btnStratListen);
            this.panel2.Controls.Add(this.btnClearInit);
            this.panel2.Controls.Add(this.cbbIDP);
            this.panel2.Controls.Add(this.btnResetCAN);
            this.panel2.Controls.Add(this.cbbBaud);
            this.panel2.Controls.Add(this.cbbID);
            this.panel2.Controls.Add(this.lblSp_01);
            this.panel2.Controls.Add(this.lblSp_03);
            this.panel2.Controls.Add(this.btnConnectionCAN);
            this.panel2.Controls.Add(this.lblSp_02);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1364, 34);
            this.panel2.TabIndex = 1;
            // 
            // btnAlarmInfo
            // 
            this.btnAlarmInfo.AutoSize = true;
            this.btnAlarmInfo.BackColor = System.Drawing.Color.Blue;
            this.btnAlarmInfo.FlatAppearance.BorderSize = 0;
            this.btnAlarmInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAlarmInfo.ForeColor = System.Drawing.SystemColors.Control;
            this.btnAlarmInfo.Location = new System.Drawing.Point(1056, 5);
            this.btnAlarmInfo.Name = "btnAlarmInfo";
            this.btnAlarmInfo.Size = new System.Drawing.Size(77, 25);
            this.btnAlarmInfo.TabIndex = 56;
            this.btnAlarmInfo.Text = "报警记录";
            this.btnAlarmInfo.UseVisualStyleBackColor = false;
            this.btnAlarmInfo.Click += new System.EventHandler(this.btnAlarmInfo_Click);
            // 
            // btnStratListen
            // 
            this.btnStratListen.AutoSize = true;
            this.btnStratListen.BackColor = System.Drawing.Color.Green;
            this.btnStratListen.FlatAppearance.BorderSize = 0;
            this.btnStratListen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStratListen.ForeColor = System.Drawing.SystemColors.Control;
            this.btnStratListen.Location = new System.Drawing.Point(973, 6);
            this.btnStratListen.Name = "btnStratListen";
            this.btnStratListen.Size = new System.Drawing.Size(77, 25);
            this.btnStratListen.TabIndex = 55;
            this.btnStratListen.Text = "总线监听";
            this.btnStratListen.UseVisualStyleBackColor = false;
            this.btnStratListen.Click += new System.EventHandler(this.btnStratListen_Click);
            // 
            // btnClearInit
            // 
            this.btnClearInit.AutoSize = true;
            this.btnClearInit.Location = new System.Drawing.Point(860, 6);
            this.btnClearInit.Name = "btnClearInit";
            this.btnClearInit.Size = new System.Drawing.Size(107, 25);
            this.btnClearInit.TabIndex = 54;
            this.btnClearInit.Text = "清除出厂设置";
            this.btnClearInit.UseVisualStyleBackColor = true;
            this.btnClearInit.Visible = false;
            this.btnClearInit.Click += new System.EventHandler(this.btnClearInit_Click);
            // 
            // cbbIDP
            // 
            this.cbbIDP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbIDP.Font = new System.Drawing.Font("宋体", 11.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbbIDP.FormattingEnabled = true;
            this.cbbIDP.Items.AddRange(new object[] {
            "1",
            "2"});
            this.cbbIDP.Location = new System.Drawing.Point(57, 7);
            this.cbbIDP.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.cbbIDP.Name = "cbbIDP";
            this.cbbIDP.Size = new System.Drawing.Size(65, 23);
            this.cbbIDP.TabIndex = 53;
            this.cbbIDP.SelectedIndexChanged += new System.EventHandler(this.cbbIDP_SelectedIndexChanged);
            // 
            // btnResetCAN
            // 
            this.btnResetCAN.AutoSize = true;
            this.btnResetCAN.Location = new System.Drawing.Point(413, 6);
            this.btnResetCAN.Name = "btnResetCAN";
            this.btnResetCAN.Size = new System.Drawing.Size(64, 25);
            this.btnResetCAN.TabIndex = 50;
            this.btnResetCAN.Text = "重连";
            this.btnResetCAN.UseVisualStyleBackColor = true;
            this.btnResetCAN.Visible = false;
            this.btnResetCAN.Click += new System.EventHandler(this.btnResetCAN_Click);
            // 
            // cbbBaud
            // 
            this.cbbBaud.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbBaud.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbbBaud.Font = new System.Drawing.Font("宋体", 11.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbbBaud.FormattingEnabled = true;
            this.cbbBaud.Items.AddRange(new object[] {
            "100Kbps",
            "125Kbps",
            "200Kbps",
            "250Kbps",
            "400Kbps",
            "500Kbps",
            "666Kbps",
            "800Kbps",
            "1000Kbps"});
            this.cbbBaud.Location = new System.Drawing.Point(260, 7);
            this.cbbBaud.Name = "cbbBaud";
            this.cbbBaud.Size = new System.Drawing.Size(80, 23);
            this.cbbBaud.TabIndex = 42;
            // 
            // cbbID
            // 
            this.cbbID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbID.Font = new System.Drawing.Font("宋体", 11.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbbID.FormattingEnabled = true;
            this.cbbID.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.cbbID.Location = new System.Drawing.Point(146, 7);
            this.cbbID.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.cbbID.Name = "cbbID";
            this.cbbID.Size = new System.Drawing.Size(65, 23);
            this.cbbID.TabIndex = 31;
            this.cbbID.SelectedIndexChanged += new System.EventHandler(this.cbbID_SelectedIndexChanged);
            // 
            // lblSp_01
            // 
            this.lblSp_01.AutoSize = true;
            this.lblSp_01.ForeColor = System.Drawing.Color.Black;
            this.lblSp_01.Location = new System.Drawing.Point(212, 12);
            this.lblSp_01.Name = "lblSp_01";
            this.lblSp_01.Size = new System.Drawing.Size(41, 12);
            this.lblSp_01.TabIndex = 41;
            this.lblSp_01.Text = "波特率";
            // 
            // lblSp_03
            // 
            this.lblSp_03.AutoSize = true;
            this.lblSp_03.ForeColor = System.Drawing.Color.Black;
            this.lblSp_03.Location = new System.Drawing.Point(123, 12);
            this.lblSp_03.Name = "lblSp_03";
            this.lblSp_03.Size = new System.Drawing.Size(17, 12);
            this.lblSp_03.TabIndex = 47;
            this.lblSp_03.Text = "ID";
            // 
            // btnConnectionCAN
            // 
            this.btnConnectionCAN.AutoSize = true;
            this.btnConnectionCAN.Location = new System.Drawing.Point(342, 6);
            this.btnConnectionCAN.Name = "btnConnectionCAN";
            this.btnConnectionCAN.Size = new System.Drawing.Size(64, 25);
            this.btnConnectionCAN.TabIndex = 43;
            this.btnConnectionCAN.Text = "连接";
            this.btnConnectionCAN.UseVisualStyleBackColor = true;
            this.btnConnectionCAN.Click += new System.EventHandler(this.btnConnectionCAN_Click);
            // 
            // lblSp_02
            // 
            this.lblSp_02.AutoSize = true;
            this.lblSp_02.ForeColor = System.Drawing.Color.Black;
            this.lblSp_02.Location = new System.Drawing.Point(9, 12);
            this.lblSp_02.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSp_02.Name = "lblSp_02";
            this.lblSp_02.Size = new System.Drawing.Size(41, 12);
            this.lblSp_02.TabIndex = 30;
            this.lblSp_02.Text = "电池簇";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1364, 721);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.Menu);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.Menu;
            this.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BMS电池上位机V1.0.2.3.20240722";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
#pragma warning disable CS0108 // “FrmMain.Menu”隐藏继承的成员“Form.Menu”。如果是有意隐藏，请使用关键字 new。
        private System.Windows.Forms.MenuStrip Menu;
#pragma warning restore CS0108 // “FrmMain.Menu”隐藏继承的成员“Form.Menu”。如果是有意隐藏，请使用关键字 new。
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox cbbIDP;
        private System.Windows.Forms.Button btnResetCAN;
        private System.Windows.Forms.ComboBox cbbBaud;
        private System.Windows.Forms.ComboBox cbbID;
        private System.Windows.Forms.Label lblSp_01;
        private System.Windows.Forms.Label lblSp_03;
        private System.Windows.Forms.Button btnConnectionCAN;
        private System.Windows.Forms.Label lblSp_02;
        private System.Windows.Forms.Button btnClearInit;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnStratListen;
        private System.Windows.Forms.Button btnAlarmInfo;
    }
}

