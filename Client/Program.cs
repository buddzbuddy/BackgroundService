using Client.ServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            using(ServiceClient client = new ServiceClient())
            {
                client.Open();

                Console.WriteLine(client.GetData(5));

                Console.WriteLine(client.GetData(5));

                client.Close();
            }

            Console.WriteLine("press any keys to exit...");
            Console.ReadLine();
        }
    }
}
