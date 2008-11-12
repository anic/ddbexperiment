using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DistDBMS.Common.Dictionary;
using DistDBMS.Common.Table;
using DistDBMS.UserInterface.Controls;

namespace DistDBMS.UserInterface.Handler
{
    class MenuTreeSwitcher
    {
        TreeView tree;
        TreeNode queryNode;
        TreeNode gddNode;
        TreeNode sitesNode;
        TreeNode tablesNode;
        TreeNode fragmentsNode;

        UscExecuteQuery uscQuery;
        UscTableSchemaViewer uscSchemaViewer;

        public MenuTreeSwitcher(TreeView tree)
        {
            this.tree = tree;

            queryNode = tree.Nodes[0];
            gddNode = tree.Nodes[1];

            sitesNode = gddNode.Nodes[0];
            tablesNode = gddNode.Nodes[1];
            fragmentsNode = gddNode.Nodes[2];

            tree.AfterSelect += new TreeViewEventHandler(tree_AfterSelect);
        }

        public void SetControl(Control ctr)
        {
            if (ctr == null)
                return;

            if (ctr is UscExecuteQuery)
                uscQuery = ctr as UscExecuteQuery;
            else if (ctr is UscTableSchemaViewer)
                uscSchemaViewer = ctr as UscTableSchemaViewer;
        }

        void tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tree.SelectedNode != null)
            {
                if (tree.SelectedNode.Tag is TableSchema)
                {
                    uscQuery.Visible = false;

                    uscSchemaViewer.Visible = true;
                    uscSchemaViewer.ShowTableSchema(tree.SelectedNode.Tag as TableSchema);
                    uscSchemaViewer.Dock = DockStyle.Fill;
                }
                else if (tree.SelectedNode == queryNode)
                {
                    uscQuery.Visible = true;
                    uscQuery.Dock = DockStyle.Fill;
                    uscSchemaViewer.Visible = false;
                    
                }
                else if (tree.SelectedNode.Tag is Fragment)
                {
                    uscQuery.Visible = false;

                    uscSchemaViewer.Visible = true;
                    uscSchemaViewer.ShowFragment(tree.SelectedNode.Tag as Fragment);
                    uscSchemaViewer.Dock = DockStyle.Fill;
                }
            }
        }

        private void SetControlVisible(Control ctr)
        {
        }


        public void SetGlobalDirectory(GlobalDirectory gdd)
        {
            //添加站点
            foreach (Site s in gdd.Sites)
            {
                TreeNode node = sitesNode.Nodes.Add(s.Name);
                node.Tag = s;
            }

            //添加逻辑表
            foreach (TableSchema t in gdd.Schemas)
            {
                TreeNode node = tablesNode.Nodes.Add(t.TableName);
                node.Tag = t;
            }

            //添加分片
            foreach (Fragment f in gdd.Fragments)
            {
                AddFragment(fragmentsNode, f);
            }
        }

        private void AddFragment(TreeNode node, Fragment fragment)
        {
            TreeNode childNode = node.Nodes.Add(fragment.Name);
            childNode.Tag = fragment;

            foreach (Fragment f in fragment.Children)
                AddFragment(childNode, f);
        }
    }
}
