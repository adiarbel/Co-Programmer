using System;
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
            ServiceHost host = new ServiceHost(typeof(CoProService.CoProService), new Uri[] { new Uri("http://localhost:" + port), new Uri("net.tcp://localhost:" + (port + 10)) });

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
                }
            }
            Type tNetFwPolicy2 = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
            INetFwPolicy2 fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(tNetFwPolicy2);
            var currentProfiles = fwPolicy2.CurrentProfileTypes;

            // Let's create a new rule

            INetFwRule2 inboundRule = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
            inboundRule.Enabled = true;
            inboundRule.Protocol = 6; // TCP
            inboundRule.LocalPorts = (port+10).ToString();
            // ...
            inboundRule.Profiles = currentProfiles;
            inboundRule.Name = "8090";
            inboundRule.Description = "opens it";
            inboundRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            inboundRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;

            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            firewallPolicy.Rules.Add(inboundRule);
            Console.WriteLine("Host started @" + DateTime.Now.ToString());
            Console.WriteLine(host.BaseAddresses[0].ToString() + '\n' + host.BaseAddresses[1].ToString());
            Console.ReadLine();
            host.Close();

        }
    }
}
