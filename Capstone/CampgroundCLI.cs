using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.Models;
using Capstone.DAL;
using System.Configuration;

namespace Capstone
{
    public class CampgroundCLI
    {
        const string Command_ViewParks = "1";
        const string Command_SelectPark = "2";
        const string Command_Quit = "q";
        readonly string DatabaseConnection;

        public CampgroundCLI(string connection)
        {
            DatabaseConnection = connection;
        }
        public void RunCLI()
        {
            PrintHeader();

            while (true)
            {
                PrintMenu();
                string command = Console.ReadLine();
                Console.Clear();

                switch (command.ToLower())
                {
                    case Command_ViewParks:
                        GetParks();
                        break;
                    case Command_SelectPark:
                        SelectPark();
                        break;
                    case Command_Quit:
                        Console.WriteLine("Thank you for using our database!");
                        return;
                    default:
                        Console.WriteLine("Please insert a valid value.");
                        break;
                }
            }
        }
        public void GetParks()
        {
            ParkSQLDAL dal = new ParkSQLDAL(DatabaseConnection);
            List<Park> parks = dal.GetParks();

            if (parks.Count > 0)
            {
                for (int i = 0; i < parks.Count; i++)
                {
                    Console.WriteLine("(" + (i + 1) + ") " + parks[i].ToString());
                }
            }
            else
            {
                Console.WriteLine("No results.");
            }
        }
        public void PrintHeader()
        {
            Console.WriteLine("**********************************");
            Console.WriteLine("Parks Database: By Kevin and Stacy");
            Console.WriteLine("**********************************\n");
        }
        public void PrintMenu()
        {
            Console.WriteLine("\nSelect your option: ");
            Console.WriteLine("(1) View All Parks");
            Console.WriteLine("(2) Select a Park");
            Console.WriteLine("(Q) Quit \n");
        }
        public void SelectPark()
        {
            // Prompt user for park, and have the ParkSQLDAL return the relevant park(s) and their information.
            string userPark = CLIHelper.GetString("Please select the Park ID: ");
            ParkSQLDAL dal = new ParkSQLDAL(DatabaseConnection);
            List<Park> foundParks = dal.FindPark(userPark);

            // Display park information, if valid
            if (foundParks.Count > 0)
            {
                foreach (Park p in foundParks)
                {
                    Console.WriteLine("Park Name: " + p.ToString());
                    Console.WriteLine("Park Location: " + p.ParkLocation);
                    Console.WriteLine("Park Area: " + p.ParkArea.ToString("N0"));
                    Console.WriteLine("Establish Date: " + p.ParkEstablishDate + " sq km");
                    Console.WriteLine("Annual Visitors: " + p.VisitorCount.ToString("N0") + "\n");
                    Console.WriteLine(p.ParkDescription);
                }
            }
            else
            {
                Console.WriteLine("No Results Found.");
            }

            // Open submenu where user can search for and book reservations, or back out to main menu
            DisplayParkSubMenu(foundParks[0]);
        }
        public void DisplayParkSubMenu(Park userParkChoice)
        {
            const string Command_ViewCampgrounds = "1";
            const string Command_SearchReservations = "2";
            const string Command_BookReservation = "3";
            const string ReturnToMainMenu = "q";

            while (true)
            {
                Console.WriteLine("What would you like to do? ");
                Console.WriteLine("(1) View campgrounds for " + userParkChoice.ToString());
                Console.WriteLine("(2) Search Reservations at " + userParkChoice.ToString());
                Console.WriteLine("(3) Book Reservation");
                Console.WriteLine("(Q) Quit ");

                string userCommand = Console.ReadLine();
                switch (userCommand.ToLower())
                {
                    case Command_ViewCampgrounds:
                        // ViewCampgrounds()
                        break;
                    case Command_SearchReservations:
                        // SearchReservations()
                        break;
                    case Command_BookReservation:
                        // BookReservation()
                        break;
                    case ReturnToMainMenu:
                        Console.Clear();
                        Console.WriteLine("Returning to main menu...");
                        return;
                    default:
                        Console.WriteLine("That's not a valid option.\n");
                        break;
                }
            }
        }
    }
}
