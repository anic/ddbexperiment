using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.Dictionary
{
    /// <summary>
    /// 状态信息
    /// </summary>
    [Serializable]
    public class Status
    {
        public bool Done { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public Status()
        {
            this.Done = false;
            StartTime = DateTime.MinValue;
            EndTime = DateTime.MaxValue;
        }
    }
}
