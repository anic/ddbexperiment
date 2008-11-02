using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.Dictionary
{
    public class FragmentList:List<Fragment>
    {
        /// <summary>
        /// 查找满足站点名称的分片
        /// </summary>
        /// <param name="sitename">站点名称</param>
        /// <returns>分片</returns>
        public List<Fragment> GetFragmentsBySiteName(string sitename)
        {
            List<Fragment> result = new List<Fragment>();
            foreach (Fragment f in this)
                FindFragmentBySiteName(sitename, f, ref result);
            return result;
        }

        private void FindFragmentBySiteName(string sitename,Fragment f,ref List<Fragment> result)
        { 
            if (f.Site != null && f.Site.Name == sitename)
                    result.Add(f);

            foreach (Fragment f1 in f.Children)
                FindFragmentBySiteName(sitename, f1, ref result);

            
        }
    }
}
