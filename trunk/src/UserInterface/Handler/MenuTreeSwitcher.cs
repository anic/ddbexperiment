using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DistDBMS.Common.Dictionary;
using DistDBMS.Common.Table;
using DistDBMS.UserInterface.Controls;
using DistDBMS.Common.Syntax;

namespace DistDBMS.UserInterface.Handler
{
    /// <summary>
    /// 控制菜单转换的类
    /// </summary>
    class MenuTreeSwitcher
    {
        TreeView tree;
        TreeNode queryNode;
        TreeNode gddNode;
        TreeNode sitesNode;
        TreeNode tablesNode;
        TreeNode fragmentsNode;

        UscExecuteQuery uscQuery;
        UscFragmentViewer uscFragmentViewer;
        UscSiteViewer uscSiteViewer;

        FrmApp frmApp;

        public MenuTreeSwitcher(TreeView tree,FrmApp frmApp)
        {
            this.tree = tree;
            this.frmApp = frmApp;

            queryNode = tree.Nodes[0];
            gddNode = tree.Nodes[1];

            sitesNode = gddNode.Nodes[0];
            tablesNode = gddNode.Nodes[1];
            fragmentsNode = gddNode.Nodes[2];


            queryNode.SelectedImageKey = queryNode.ImageKey = "magnifier";
            gddNode.SelectedImageKey = gddNode.ImageKey = "dictionary";
            sitesNode.SelectedImageKey = sitesNode.ImageKey = "site";
            tablesNode.SelectedImageKey = tablesNode.ImageKey = "table";
            fragmentsNode.SelectedImageKey = fragmentsNode.ImageKey = "hfragment";

            tree.AfterSelect += new TreeViewEventHandler(tree_AfterSelect);
        }



        public void SetControl(Control ctr)
        {
            if (ctr == null)
                return;

            if (ctr is UscExecuteQuery)
                uscQuery = ctr as UscExecuteQuery;
            else if (ctr is UscFragmentViewer)
                uscFragmentViewer = ctr as UscFragmentViewer;
            else if (ctr is UscSiteViewer)
                uscSiteViewer = ctr as UscSiteViewer;

        }

        private string GenerateSQL(TableSchema schema, Condition condition)
        { return GenerateSQL(schema, condition, schema.TableName); }

        private string GenerateSQL(TableSchema schema,Condition condition,string logicTablename)
        {
            string result = "select ";
            for (int i = 0; i < schema.Fields.Count;i++ )
            {
                if (i != 0)
                    result += ", ";

                result += schema.Fields[i].AttributeName;
            }

            result += " from " + logicTablename;

            if (condition != null && !condition.IsEmpty)
                result += " where " + condition.ToString();

            return result;
        }

        void tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tree.SelectedNode != null)
            {
                if (tree.SelectedNode.Tag is TableSchema)
                {
                    uscFragmentViewer.Visible = true;
                    uscSiteViewer.Visible = false;
                    uscQuery.Visible = true;

                    uscFragmentViewer.ShowSchema(tree.SelectedNode.Tag as TableSchema);
                    
                    string sql  =GenerateSQL(tree.SelectedNode.Tag as TableSchema,null);
                    bool oldValue = uscQuery.ShowTip;

                    uscQuery.SqlTextReadOnly = true;
                    uscQuery.ShowTip = false;
                    uscQuery.SQLText = sql;
                    uscQuery.ShowTip = oldValue;
                    uscQuery.Tab = UscExecuteQuery.ResultTab.Table;
                    
                    
                }
                else if (tree.SelectedNode == queryNode)
                {
                    
                    
                    
                    uscQuery.Visible = true;
                    uscFragmentViewer.Visible = false;
                    uscSiteViewer.Visible = false;

                    uscQuery.SqlTextReadOnly = false;
                    uscQuery.Tab = UscExecuteQuery.ResultTab.Console;
                    
                }
                else if (tree.SelectedNode.Tag is Fragment)
                {

                    uscQuery.Visible = true;
                    uscFragmentViewer.Visible = true;
                    uscSiteViewer.Visible = false;

                    uscFragmentViewer.ShowFragment(tree.SelectedNode.Tag as Fragment, gdd);
                    
                    string sql = GenerateSQL((tree.SelectedNode.Tag as Fragment).Schema, 
                        (tree.SelectedNode.Tag as Fragment).Condition, 
                        (tree.SelectedNode.Tag as Fragment).LogicSchema.TableName);

                    bool oldValue = uscQuery.ShowTip;

                    uscQuery.SqlTextReadOnly = true;
                    uscQuery.ShowTip = false;
                    uscQuery.SQLText = sql;
                    uscQuery.ShowTip = oldValue;
                    uscQuery.Tab = UscExecuteQuery.ResultTab.Table;

                    
                }
                else if (tree.SelectedNode.Tag is Site)
                {
                    uscQuery.Visible = false;
                    uscFragmentViewer.Visible = false;
                    uscSiteViewer.Visible = true;

                    uscSiteViewer.SetSite(tree.SelectedNode.Tag as Site);
                }
                tree.Focus();
            }
        }

        GlobalDirectory gdd;
        public void SetGlobalDirectory(GlobalDirectory gdd)
        {
            sitesNode.Nodes.Clear();
            fragmentsNode.Nodes.Clear();
            tablesNode.Nodes.Clear();

            if (gdd == null)
                return;

            //添加站点
            foreach (Site s in gdd.Sites)
            {
                TreeNode node = sitesNode.Nodes.Add(s.Name);
                node.SelectedImageKey = node.ImageKey = "site";
                node.Tag = s;
            }

            //添加逻辑表
            foreach (TableSchema t in gdd.Schemas)
            {
                TreeNode node = tablesNode.Nodes.Add(t.TableName);
                node.SelectedImageKey = node.ImageKey = "table";
                node.Tag = t;
            }

            //添加分片
            foreach (Fragment f in gdd.Fragments)
            {
                AddFragment(fragmentsNode, f);
            }

            this.gdd = gdd;
        }

        private void AddFragment(TreeNode node, Fragment fragment)
        {
            TreeNode childNode = node.Nodes.Add(fragment.Name);
            childNode.Tag = fragment;
            switch(fragment.Type)
            {
                case FragmentType.None:
                    childNode.SelectedImageKey = childNode.ImageKey = "table";
                    break;
                case FragmentType.Horizontal:
                    childNode.SelectedImageKey = childNode.ImageKey = "hfragment";
                    break;
                case FragmentType.Vertical:
                    childNode.SelectedImageKey = childNode.ImageKey = "vfragment";
                    break;

            }
            
            foreach (Fragment f in fragment.Children)
                AddFragment(childNode, f);
        }
    }
}
