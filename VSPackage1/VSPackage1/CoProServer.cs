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
    public class CoProServer
    {
        ServiceHost host;
        int port;

        /// <summary>
        /// Initialization of the service
        /// </summary>
        public CoProServer()
        {
            bool flag = true;
            port = 8080;
            NetTcpBinding mybinding;
            ServiceMetadataBehavior smb;
            host = new ServiceHost(typeof(CoProService.CoProService), new Uri[] { new Uri("http://localhost:" + port), new Uri("net.tcp://localhost:" + (port + 10)) });
            try
            {
                mybinding = new NetTcpBinding();// BINDING CONFIGURATION
                mybinding.PortSharingEnabled = true;
                mybinding.Security.Mode = SecurityMode.None;
                TimeSpan minutes10 = new TimeSpan(0, 10, 0);//TIMEOUTS CONFIGURATION
                mybinding.ReliableSession.InactivityTimeout = minutes10;
                mybinding.SendTimeout = minutes10;
                mybinding.ReceiveTimeout = minutes10;
                mybinding.CloseTimeout = minutes10;
                mybinding.OpenTimeout = minutes10;
                // Step 3 Add a service endpoint.
                host.AddServiceEndpoint(typeof(CoProService.ICoProService), mybinding, "CoProService");// ADD ENDPOINTS TO THE LIST
                // Step 4 Enable metadata exchange. 
                smb = new ServiceMetadataBehavior();//SERVICE BEHAVIORS
                smb.HttpGetEnabled = true;
                host.Description.Behaviors.Add(smb);
            }
            catch (Exception e)
            {
                //Future exception handler
            }
            while (flag)
            {
                try
                {
                    Task.Factory.StartNew(() =>
                        {
                            host.Open();//OPEN THE SERVER
                        });
                    flag = false;
                }
                catch (Exception e)
                {
                    port++;//IF FAILS CHANGES THE PORT AND RE-CONFIGURES
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
        }

        /// <summary>
        /// Closing the service
        /// </summary>
        public void Close()
        {
            host.Close();
        }

        /// <summary>
        /// getter for addresses
        /// </summary>
        /// <returns>addresses that the server listens to</returns>
        public System.Collections.ObjectModel.ReadOnlyCollection<Uri> GetAddresses()
        {
            return host.BaseAddresses;
        }

        /// <summary>
        /// getter for the port
        /// </summary>
        /// <returns>the current port that the service runs on</returns>
        public int PortOfService()
        {
            return port;
        }
    }
}
