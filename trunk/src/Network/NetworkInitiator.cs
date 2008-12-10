using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace DistDBMS.Network
{
    /// <summary>
    /// 用于读取网络配置的初始文件
    /// </summary>
    public class NetworkInitiator
    {
        public List<string> LocalSiteNames { get { return localSite; } }
        List<string> localSite = new List<string>();

        public List<string> ControlSiteNames { get { return controlSite; } }
        List<string> controlSite = new List<string>();

        public ClusterConfiguration GetConfiguration(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException("File not found", filename);

            ClusterConfiguration clusterConfig = new ClusterConfiguration();
            StreamReader sr = new StreamReader(filename, System.Text.Encoding.Default);

            while (!sr.EndOfStream)
            {
                try
                {
                    string[] param = sr.ReadLine().Split(' ');
                    if (param[0] == "ControlSite" && param.Length == 4)
                    {
                        clusterConfig.Hosts[param[1]]["Host"] = param[2];
                        clusterConfig.Hosts[param[1]]["Port"] = Int32.Parse(param[3]);
                        controlSite.Add(param[1]);
                    }
                    else if (param[0] == "LocalSite" && param.Length == 5)
                    {
                        clusterConfig.Hosts[param[1]]["Host"] = param[2];
                        clusterConfig.Hosts[param[1]]["Port"] = Int32.Parse(param[3]);
                        clusterConfig.Hosts[param[1]]["P2PPort"] = Int32.Parse(param[4]);
                        localSite.Add(param[1]);
                    }
                }
                catch
                { }
            }
            sr.Close();

            return clusterConfig;
        }
    }
}
