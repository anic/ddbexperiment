using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Network;
using DistDBMS.Common.Dictionary;

namespace DistDBMS.ControlSite
{
    class PackageProcessor
    {
        GlobalDirectory gdd;

        public bool IsReady { get { return gdd != null; } }


        public void PackageProcess(ControlSiteServerConnection conn, ServerClientPacket packet)
        {
            //packet
            if (packet is ServerClientPacket) 
            {
                if (packet is ServerClientTextObjectPacket)
                {
                    if ((packet as ServerClientTextObjectPacket).Text == Common.NetworkCommon.GDDSCRIPT)
                    {
                        string[] gddScript = (packet as ServerClientTextObjectPacket).Object as string[];
                        if (ImportScript(gddScript))
                        {
                            //初始化每个二级接口
                            foreach (Site site in gdd.Sites)
                                conn.GetLocalSiteClient(site.Name).SendStepTextObjectPacket(conn.SessionId, 
                                    Network.SessionStepPacket.StepIndexNone, 
                                    Network.SessionStepPacket.StepIndexNone, 
                                    Common.NetworkCommon.GDDSCRIPT, gdd);
                            
                            foreach (Site site in gdd.Sites)
                                conn.GetLocalSiteClient(site.Name).Packets.WaitAndRead();
                            
                            string result = "Import gdd script successfully.";

                            conn.SendServerClientTextPacket(result);
                        }
                        else
                        {
                            string result = "Fail to import Script";
                            conn.SendServerClientTextPacket(result);
                        }
                    }

                }
            }


            
        }

        public bool ImportScript(string[] file)
        {
            //初始化GDD
            GDDCreator gddCreator = new GDDCreator();
            foreach (string line in file)
                gddCreator.InsertCommand(line);

            gdd = gddCreator.CreateGDD();
            return gdd != null;

        }
    }
}
