using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
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
            
            Console.WriteLine("Host started @" + DateTime.Now.ToString());
            Console.WriteLine(host.BaseAddresses[0].ToString() + '\n' + host.BaseAddresses[1].ToString());
            Console.ReadLine();
            host.Close();

        }
    }
}
