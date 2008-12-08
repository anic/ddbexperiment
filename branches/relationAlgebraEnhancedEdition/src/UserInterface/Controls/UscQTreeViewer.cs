using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DistDBMS.Common.RelationalAlgebra.Entity;

namespace DistDBMS.UserInterface.Controls
{
    public partial class UscQTreeViewer : UserControl
    {
        public UscQTreeViewer()
        {
            InitializeComponent();
        }

        private TreeNode CreateNode(Relation relation)
        {
            TreeNode node = new TreeNode();
            node.Text = relation.ToString();
            node.ToolTipText = relation.ToString();

            return node;
        }

        public void ShowRelation(Relation relation)
        {
            tvwRelation.Nodes.Clear();

            if (relation == null)
                return;

            TreeNode node = CreateNode(relation);
            tvwRelation.Nodes.Add(node);

            foreach (Relation child in relation.Children)
                Visit(node, child);
        }

        private void Visit(TreeNode node, Relation relation)
        {
            TreeNode n = CreateNode(relation);
            node.Nodes.Add(n);

            foreach (Relation child in relation.Children)
                Visit(n, child);
        }
    }
}
