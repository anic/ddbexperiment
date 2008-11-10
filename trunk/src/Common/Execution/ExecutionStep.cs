using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Dictionary;
using DistDBMS.Common.RelationalAlgebra.Entity;

namespace DistDBMS.Common.Execution
{
    public class ExecutionStep
    {
        public enum ExecuteType
        {
            CreateTable,
            CreateDatabase,
            Select,
            Insert,
            Delete,
            Update
        }

        /// <summary>
        /// 第几步
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 等待的ID
        /// </summary>
        public List<string> WaitingId { get { return waitingList; } }
        List<string> waitingList;

        /// <summary>
        /// 是否等待ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsWaiting(string id)
        {
            foreach (string s in waitingList)
                if (s == id)
                    return true;

            return false;
        }

        /// <summary>
        /// 将数据发送到的地方
        /// </summary>
        public Site TransferSite { get; set; }

        //TODO:这里需要斟酌，包括记录是否完成，事件，状态信息
        public Status Status { get; set; }

        /// <summary>
        /// 执行动作
        /// </summary>
        public ExecuteType Type { get; set; }

        /// <summary>
        /// 数据，如果包含，则放入这个部分
        /// </summary>
        public Common.Table.Table Table { get; set; }

        /// <summary>
        /// 如果是select，则把相关的Relation放入
        /// </summary>
        public ExecutionRelation Operation { get; set; }

        public ExecutionStep()
        {
            waitingList = new List<string>();
            Index = 0;
            TransferSite = null;
            Status = null;
            Type = ExecuteType.Select;
            Table = null;
            Operation = null;
        }

        public new string ToString()
        {
            string result = "";
            if (Operation != null)
            {
                Queue<Relation> queue = new Queue<Relation>();
                queue.Enqueue(Operation);

                while (queue.Count > 0)
                {
                    Relation r = queue.Dequeue();
                    result += r.ToString() + "\n";

                    foreach (Relation r1 in r.Children)
                        queue.Enqueue(r1);
                }
                
            }
            result += "Waiting: ";

            for (int i = 0; i < waitingList.Count;i++ )
            {
                if (i != 0)
                    result += ", ";

                result += waitingList[i];
            }
            result += "\n";

            result += "Transfer:";
            if (TransferSite != null)
                result += TransferSite.Name;
            result += "\n";

            return result;
        }
    }
}
