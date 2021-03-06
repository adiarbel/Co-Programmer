﻿using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using NetFwTypeLib;
namespace CoProServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            // Located in FirewallAPI.dll

            bool flag = true;
            int port = 8080;
            NetTcpBinding mybinding;
            ServiceMetadataBehavior smb;
            ServiceHost host;
            INetFwPolicy2 fwPolicy2;
            host = new ServiceHost(typeof(CoProService.CoProService), new Uri[] { new Uri("http://localhost:" + port), new Uri("net.tcp://localhost:" + (port + 10)) });
            try
            {
                mybinding = new NetTcpBinding();
                mybinding.PortSharingEnabled = true;
                mybinding.Security.Mode = SecurityMode.None;
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
                    host.Open();
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
            Console.WriteLine("Host started @" + DateTime.Now.ToString());
            Console.WriteLine(host.BaseAddresses[0].ToString() + '\n' + host.BaseAddresses[1].ToString());
            Console.ReadLine();
            host.Close();
            fwPolicy2.Rules.Remove("PortCoProgrammer");

        }
    }
}
