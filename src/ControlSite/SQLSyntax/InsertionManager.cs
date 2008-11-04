using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Dictionary;
using DistDBMS.Common.Table;

namespace DistDBMS.ControlSite.SQLSyntax
{
    class InsertionManager
    {
        GlobalDataDictionary gdd;
        TableScheme currentScheme;
        public InsertionManager(GlobalDataDictionary gdd)
        {
            this.gdd = gdd;
        }

        private void SetTable(string tablename)
        { 
            currentScheme = gdd.Schemes[tablename];
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
            queue.Enqueue(gdd.Fragments.GetFragmentByName(currentScheme.TableName));

            while(queue.Count>0)
            {
                Fragment f = queue.Dequeue();
                if (f.Children.Count == 0)//叶子分片
                { 

                }
            }
            return result;
        }

        //private bool MatchFragment(Tuple t, Fragment f)
        //{
        //    if (f.Type == FragmentType.Vertical)
        //        return true;
        //    else if (f.Type == FragmentType.Horizontal)
        //    { 
        //        f.
        //    }

        //}

        private Tuple GenerateTuple(string data)
        {
            Tuple result = new Tuple();
            string[] values = data.Split('\t');

            result.Data.AddRange(values);
            return result;
        }
    }
}
