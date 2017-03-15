using Client.ServiceReference;
using System;
using System.Threading;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            bool work = true;
            while (work)
            {
                var c = Console.ReadKey().KeyChar;
                if (c == 's')
                {
                    StartCache();
                }
                else if (c == 'v')
                {
                    ViewCache();
                }
                else work = false;
            }
            Console.WriteLine("press any keys to exit...");
            Console.ReadLine();
        }
        static int Count = 10;
        static void StartCache()
        {
            using (ServiceClient client = new ServiceClient())
            {
                client.Open();
                
                Console.WriteLine("please type the names to fill cache...");

                var name = Console.ReadLine();

                for (int i = 1; i <= Count; i++)
                {
                    client.StartProcess(name + i);
                    Thread.Sleep(1000);
                    Console.WriteLine(name + i);
                }

                Console.WriteLine("Filling is closed.");

                client.Close();
            }
        }

        static void ViewCache()
        {
            string exitKey = "q";
            using (ServiceClient client = new ServiceClient())
            {
                client.Open();

                Console.WriteLine("below has cached values, for updating press enter, to exit press \"" + exitKey + "\" and enter...");
                bool isShow = true;
                while (isShow)
                {
                    Console.WriteLine("please type the name to get cache value...");
                    var name = Console.ReadLine();
                    for (int i = 1; i <= Count; i++)
                    {
                        Console.WriteLine(name + i + ": " + client.GetTaskState(name + i));
                    }
                    isShow = exitKey != Console.ReadLine();
                    Console.WriteLine();
                }
                Console.WriteLine("Viewing is closed.");

                client.Close();
            }
        }
    }
}
