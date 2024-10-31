using SofarBMS.Model;
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
    public partial class FrmAlarm : Form
    {
        public FrmAlarm()
        {
            InitializeComponent();
        }

        // 2.定义委托方法
        public void ToReceived(List<AlarmInfo> infos)
        {
            foreach (var item in infos)
            {
                ListViewItem listViewItem = new ListViewItem(item.DataTime);
                listViewItem.SubItems.Add(item.Id.ToString());
                listViewItem.SubItems.Add(item.Type.ToString());
                listViewItem.SubItems.Add(item.Content.ToString());

                this.listView1.Items.Add(listViewItem);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.listView1.Items.Clear();
        }
    }
}
