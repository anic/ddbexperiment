using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.ServerSite.RelationalAlgebra.Entity;
using DistDBMS.Common.Table;

namespace DistDBMS.ServerSite.RelationalAlgebra
{
    class RelationDebugger
    {
        public string GetDebugString(Relation root)
        {
            return GetNextLevelString(root, 0);

        }

        private string GetSingleString(Relation node)
        {
            string result = node.Type.ToString() + ": ";
            if (node.IsDirectTableScheme)
                result += " " + node.DirectTableScheme.ToString();

            if (node.Predication.Content != "")
                result += " Predication: " + node.Predication.Content;

            if (node.RelativeAttributes.Fields.Count > 0 || node.RelativeAttributes.TableName != "")
                result += " Attributes: " + node.RelativeAttributes.ToString();
            
            return result;

        }

        private string GetNextLevelString(Relation node, int level)
        {
            string result = "";

            string space = "     ";
            for (int i = 0; i < level; i++)
                result += space;

            result += "|_";
            result += GetSingleString(node);
            result += "\n";

            if (node.LeftRelation!=null)
            {
                result += GetNextLevelString(node.LeftRelation, level + 1);
            }

            if (node.RightRelation != null)
            {
                result += GetNextLevelString(node.RightRelation, level + 1);
            }
            return result;
        }

    }
}
