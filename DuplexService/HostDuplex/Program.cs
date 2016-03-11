using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Microsoft.VisualStudio.Text;
namespace HostDuplex
{
    class Program
    {
        static void Main(string[] args)
        {
            
            ServiceHost host = new ServiceHost(typeof(DuplexService.EditService));
            host.Open();
            Console.WriteLine("Host started @" + DateTime.Now.ToString());
            Console.ReadLine();
        }
    }
    
}
