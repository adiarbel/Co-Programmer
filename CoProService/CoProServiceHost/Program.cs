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
            INetFwRule firewallRule = (INetFwRule)Activator.CreateInstance(
    Type.GetTypeFromProgID("HNetCfg.FWRule"));
            firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            firewallRule.Description = "USADASDess.";
            firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
            firewallRule.Enabled = true;
            firewallRule.InterfaceTypes = "All";
            firewallRule.Name = "";
            firewallRule.Protocol = 6;
            firewallRule.LocalPorts = "8090";

            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
                Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            firewallPolicy.Rules.Add(firewallRule);
            firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
    Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
firewallPolicy.Rules.Remove("Block Internet");
            Console.WriteLine("Host started @" + DateTime.Now.ToString());
            Console.WriteLine(host.BaseAddresses[0].ToString() + '\n' + host.BaseAddresses[1].ToString());
            Console.ReadLine();
            host.Close();

        }
    }
}
