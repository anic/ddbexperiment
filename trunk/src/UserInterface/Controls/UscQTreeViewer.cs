using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DistDBMS.Common.RelationalAlgebra.Entity;
using DistDBMS.Common.Execution;
using DistDBMS.Common.Dictionary;
using DistDBMS.UserInterface.Handler;

namespace DistDBMS.UserInterface.Controls
{
    public partial class UscQTreeViewer : UserControl
    {
        public UscQTreeViewer()
        {
            InitializeComponent();

            for (int i = 0; i < colors.Length; ++i)
                imgList.Images.Add(CalendarIconUtil.Instance.DrawCalendarImage(colors[i]));
        }

        Color[] colors = new Color[] { Color.Red, Color.Blue, Color.Green, Color.Orange };
        SiteList sites = new SiteList();

        private TreeNode CreateNode(string text,int index)
        {
            TreeNode node = new TreeNode();
            node.Text = text;
            node.ToolTipText = text;
            if (index != -1)
            {
                //node.BackColor = colors[index];
                //node.ImageIndex = index;
                //node.SelectedImageIndex = index;
            }
            node.ImageIndex = index;
            node.SelectedImageIndex = index;
            
            return node;
        }

        public void ShowExecutionRelation(ExecutionRelation relation)
        {
            sites.Clear();

            tvwRelation.Nodes.Clear();

            if (relation == null)
                return;

            TreeNode n;
            if (relation.ExecutionSite != null)
            {
                string strSite = relation.ExecutionSite.Name;
                if (sites[relation.ExecutionSite.Name] == null)
                    sites.Add(relation.ExecutionSite);

                n = CreateNode(relation.ToSimpleString() + " " + strSite, sites.GetIndexOf(relation.ExecutionSite) % colors.Length);
                
            }
            else
            {
                n = CreateNode(relation.ToSimpleString(), -1);
            }
            tvwRelation.Nodes.Add(n);
            foreach (ExecutionRelation child in relation.Children)
                Visit(n, child);
        }

        private void Visit(TreeNode node, ExecutionRelation relation)
        {
            TreeNode n;
            if (relation.ExecutionSite != null)
            {
                string strSite = relation.ExecutionSite.Name;
                if (sites[relation.ExecutionSite.Name] == null)
                    sites.Add(relation.ExecutionSite);

                n = CreateNode(relation.ToSimpleString() + " " + strSite, sites.GetIndexOf(relation.ExecutionSite) % colors.Length);
            }
            else
            {
                n = CreateNode(relation.ToSimpleString(), -1);
            } 
            node.Nodes.Add(n);

            foreach (ExecutionRelation child in relation.Children)
                Visit(n, child);
        }

        public void ShowRelation(Relation relation)
        {
            tvwRelation.Nodes.Clear();

            if (relation == null)
                return;

            TreeNode node = CreateNode(relation.ToString(),-1);
            tvwRelation.Nodes.Add(node);

            foreach (Relation child in relation.Children)
                Visit(node, child);
        }

        private void Visit(TreeNode node, Relation relation)
        {
            TreeNode n = CreateNode(relation.ToString(), -1);
            node.Nodes.Add(n);

            foreach (Relation child in relation.Children)
                Visit(n, child);
        }
    }
}
