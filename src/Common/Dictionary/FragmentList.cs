using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.Dictionary
{
    [Serializable]
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

        /// <summary>
        /// 通过分片名称获得分片
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Fragment GetFragmentByName(string name)
        {
            foreach (Fragment f in this)
            {
                Fragment result = FindFragmentByName(f, name);
                if (result != null)
                    return result;
            }
            return null;
        }

        
        /// <summary>
        /// 通过站点名称获得分片
        /// </summary>
        /// <param name="f"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private Fragment FindFragmentByName(Fragment f, string name)
        { 
            if (f.Name == name)
                return f;

            foreach(Fragment child in f.Children)
            {
                Fragment fInChild = FindFragmentByName(child, name);
                if (fInChild != null)
                    return fInChild;

            }

            return null;
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
