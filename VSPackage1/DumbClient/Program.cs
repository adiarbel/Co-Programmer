using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DumbClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Callbacks c = new Callbacks();
            for (int i = 0; i < 10; i++)
            {
                int line = 12+i;
                c.callService("sdfsd",300-i*30,0);
                System.Threading.Thread.Sleep(1000);
            }
            c.getChange();
            Console.ReadLine();
        }
    }
}
