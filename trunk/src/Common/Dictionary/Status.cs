using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.Dictionary
{
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
