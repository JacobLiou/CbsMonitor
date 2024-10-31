using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SofarBMS
{
    public partial class FrmConfig : Form
    {
        bool flag;
        List<int> ids;

        public FrmConfig(List<int> _ids, bool _flag)
        {
            InitializeComponent();

            this.ids = _ids;
            this.flag = _flag;
        }

        private void FrmConfig_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < ids.Count; i++)
            {
                string name = "ckCluster" + ids[i];
                if (this.groupBox1.Controls.Find(name, true).Length > 0)
                {
                    CheckBox ck = (CheckBox)this.groupBox1.Controls.Find(name, true)[0];
                    ck.Checked = true;
                }
            }

            //ckFlag.Checked = flag;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //遍历控件状态
            ids = new List<int>();

            foreach (var item in this.groupBox1.Controls)
            {
                if (item is CheckBox)
                {
                    CheckBox ck = (CheckBox)item;
                    string id = ck.Name.Replace("ckCluster", "");

                    if (ck.Checked)
                    {
                        ids.Add(Convert.ToInt32(id));
                    }
                    else
                    {
                        ids.Remove(Convert.ToInt32(id));
                    }
                }
            }

            flag = ids.Count != 0;//ckFlag.Checked;

            //参数回传
            FrmMain.flag = flag;
            FrmMain.Main_ids = ids;
            this.Hide();
        }
    }
}
