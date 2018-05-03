using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTUClient
{
    class Program
    {
        static void Main(string[] args)
        {
            int broj = int.Parse(Console.ReadLine());
            while (broj != 0)
            {
                Client cl = new Client(broj);
                broj = int.Parse(Console.ReadLine());
            }
            

            // Client cl1 = new Client(1);
            // Client cl2 = new Client(2);
            //Console.ReadKey();
        }
    }
}
