using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.RelationalAlgebra.Entity;
using DistDBMS.Common.Table;
using DistDBMS.Common.Execution;

namespace DistDBMS.Common.RelationalAlgebra
{
    public class RelationDebugger
    {
        public string GetDebugString(Relation root)
        {
            return GetNextLevelString(root, 0);

        }

        public string GetDebugString(ExecutionRelation root)
        {
            return GetNextLevelString(root, 0);
        }

        

        private string GetNextLevelString(Relation node, int level)
        {
            string result = "";

            string space = "     ";
            for (int i = 0; i < level; i++)
                result += space;

            result += "|_";
            result += node.ToString();
            result += "\n";

            foreach(Relation child in node.Children)
            {
                result += GetNextLevelString(child, level + 1);
            }
            
            return result;
        }

    }
}
