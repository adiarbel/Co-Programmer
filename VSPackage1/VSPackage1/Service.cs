using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetFwTypeLib;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Company.VSPackage1
{
    public class Service
    {
        ServiceHost host;
        INetFwPolicy2 fwPolicy2;
        int port;
        public Service()
        {
            bool flag = true;
            port = 8080;
            NetTcpBinding mybinding;
            ServiceMetadataBehavior smb;
            host = new ServiceHost(typeof(CoProService.CoProService), new Uri[] { new Uri("http://localhost:" + port), new Uri("net.tcp://localhost:" + (port + 10)) });
            try
            {
                mybinding = new NetTcpBinding();
                mybinding.PortSharingEnabled = true;
                mybinding.Security.Mode = SecurityMode.None;
                TimeSpan minutes10 = new TimeSpan(0, 10, 0);
                mybinding.SendTimeout = minutes10;
                mybinding.ReceiveTimeout = minutes10;

                // Step 3 Add a service endpoint.
                host.AddServiceEndpoint(typeof(CoProService.ICoProService), mybinding, "CoProService");

                // Step 4 Enable metadata exchange. 
                smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                host.Description.Behaviors.Add(smb);
            }
            catch (Exception e)
            {

            }
            while (flag)
            {
                try
                {
                    Task.Factory.StartNew(() =>
                        {
                            host.Open();
                        });
                    flag = false;
                }
                catch (Exception e)
                {
                    port++;
                    host = new ServiceHost(typeof(CoProService.CoProService), new Uri[] { new Uri("http://localhost:" + port), new Uri("net.tcp://localhost:" + (port + 10)) });
                    mybinding = new NetTcpBinding();
                    mybinding.PortSharingEnabled = true;
                    // Step 3 Add a service endpoint.
                    host.AddServiceEndpoint(typeof(CoProService.ICoProService), mybinding, "CoProService");

                    // Step 4 Enable metadata exchange. 
                    smb = new ServiceMetadataBehavior();
                    smb.HttpGetEnabled = true;
                    host.Description.Behaviors.Add(smb);
                }
            }
            Type tNetFwPolicy2 = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
            fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(tNetFwPolicy2);
            var currentProfiles = fwPolicy2.CurrentProfileTypes;

            // Let's create a new rule

            INetFwRule2 inboundRule = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
            inboundRule.Enabled = true;
            inboundRule.Protocol = 6; // TCP
            inboundRule.LocalPorts = (port + 10).ToString();
            inboundRule.Profiles = currentProfiles;
            inboundRule.Name = "PortCoProgrammer";
            inboundRule.Description = "opens it";
            inboundRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            inboundRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;

            fwPolicy2.Rules.Add(inboundRule);
        }
        public void Close()
        {
            host.Close();
            fwPolicy2.Rules.Remove("PortCoProgrammer");
        }
        public System.Collections.ObjectModel.ReadOnlyCollection<Uri> GetAddresses()
        {
            return host.BaseAddresses;
        }
        public int PortOfService()
        {
            return port;
        }
    }
}
