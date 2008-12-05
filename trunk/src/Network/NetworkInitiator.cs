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
                    }
                    else if (param[0] == "LocalSite" && param.Length == 5)
                    {
                        clusterConfig.Hosts[param[1]]["Host"] = param[2];
                        clusterConfig.Hosts[param[1]]["Port"] = Int32.Parse(param[3]);
                        clusterConfig.Hosts[param[1]]["P2PPort"] = Int32.Parse(param[4]);
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
