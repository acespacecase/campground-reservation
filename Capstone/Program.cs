using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone
{
    class Program
    {
        static void Main(string[] args)
        {
            // Sample Code to get a connection string from the
            // App.Config file
            // Use this so that you don't need to copy your connection string all over your code!
            string connectionString = ConfigurationManager.ConnectionStrings["CapstoneDatabase"].ConnectionString;

            CampgroundCLI program = new CampgroundCLI(connectionString);
            program.RunCLI();
        }
    }
}
