﻿using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Dictionary;
using DistDBMS.Common.Table;

namespace DistDBMS.ControlSite.SQLSyntax
{
    class InsertionManager
    {
        GlobalDirectory gdd;
        TableSchema currentSchema;
        public InsertionManager(GlobalDirectory gdd)
        {
            this.gdd = gdd;
        }

        private void SetTable(string tablename)
        { 
            currentSchema = gdd.Schemas[tablename];
        }

        public void Init()
        { 

        }

        public void InsertData(string data)
        { 
            
        }

        public List<Fragment> FindFragment(Tuple t)
        { 
            List<Fragment> result = new List<Fragment>();
            Queue<Fragment> queue = new Queue<Fragment>();
            queue.Enqueue(gdd.Fragments.GetFragmentByName(currentSchema.TableName));

            while(queue.Count>0)
            {
                Fragment f = queue.Dequeue();
                if (f.Children.Count == 0)//叶子分片
                { 

                }
            }
            return result;
        }

        private Tuple GenerateTuple(string data)
        {
            Tuple result = new Tuple();
            string[] values = data.Split('\t');

            result.Data.AddRange(values);
            return result;
        }
    }
}
