using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DistDBMS.Common.Dictionary;

namespace DistDBMS.UserInterface.Controls
{
    public partial class UscSiteViewer : UserControl
    {
        public UscSiteViewer()
        {
            InitializeComponent();
        }

        public void SetSite(Site site)
        {
            lvwSite.Items.Clear();

            if (site == null)
                return;

            lblSitename.Text = site.Name;

            ListViewItem item = new ListViewItem();
            item.Text = site.Name;

            ListViewItem.ListViewSubItem sub = new ListViewItem.ListViewSubItem();
            //TODO:站点地址
            //sub.Text = site.IP;
            item.SubItems.Add(sub);

            sub = new ListViewItem.ListViewSubItem();
            //TODO:站点地址
            //sub.Text = site.Port.ToString();
            item.SubItems.Add(sub);

            lvwSite.Items.Add(item);
        }

        
    }
}
