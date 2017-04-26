using System;
using System.Configuration;
using System.Data.SqlClient;

namespace AddressBook
{
    class Program
    {
        static void Main(string[] args)
        {
            //I love this stuff so much
            string name = ConfigurationManager.AppSettings["ApplicationName"];
            Console.WriteLine("WELCOME TO:");
            Console.WriteLine(name);
            Console.WriteLine(new string('-', Console.WindowWidth - 4));
            Console.WriteLine();
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();

            

            Rolodex rolodex = new Rolodex();
            rolodex.DoStuff();
        }
    }
}
