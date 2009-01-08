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

namespace DistDBMS.UserInterface.Controls
{
    public partial class UscQTreeViewer : UserControl
    {
        public UscQTreeViewer()
        {
            InitializeComponent();
        }

        Color[] colors = new Color[] { Color.Red, Color.Blue, Color.Green, Color.Orange };
        SiteList sites = new SiteList();

        private TreeNode CreateNode(string text,Color color)
        {
            TreeNode node = new TreeNode();
            node.Text = text;
            node.ToolTipText = text;
            node.BackColor = color;
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

                n = CreateNode(relation.ToSimpleString() + " " + strSite, colors[sites.GetIndexOf(relation.ExecutionSite) % colors.Length]);
                
            }
            else
            { 
                n = CreateNode(relation.ToSimpleString(),Color.Empty);
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

                n = CreateNode(relation.ToSimpleString() + " " + strSite, colors[sites.GetIndexOf(relation.ExecutionSite) % colors.Length]);
            }
            else
            {
                n = CreateNode(relation.ToSimpleString(), Color.Empty);
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

            TreeNode node = CreateNode(relation.ToString(), Color.Empty);
            tvwRelation.Nodes.Add(node);

            foreach (Relation child in relation.Children)
                Visit(node, child);
        }

        private void Visit(TreeNode node, Relation relation)
        {
            TreeNode n = CreateNode(relation.ToString(), Color.Empty);
            node.Nodes.Add(n);

            foreach (Relation child in relation.Children)
                Visit(n, child);
        }
    }
}
