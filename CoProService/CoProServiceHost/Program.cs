using System;
using System.ServiceModel;

namespace CoProServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(CoProService.CoProService));
            host.Open();
            Console.WriteLine("Host started @" + DateTime.Now.ToString());
            Console.ReadLine();
        }
    }
}
