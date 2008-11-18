using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Dictionary;

namespace DistDBMS.ControlSite
{
    class LocalDirectory
    {
        public FragmentList Fragments { get{return fragments;} }
        FragmentList fragments;

        public SiteList Sites { get { return sites; } }
        SiteList sites;

        public LocalDirectory(GlobalDirectory gdd,string name)
        {
            fragments = new FragmentList();
            sites = new SiteList();

            fragments.AddRange(gdd.Fragments.GetFragmentsBySiteName(name));
        }
    }
}
