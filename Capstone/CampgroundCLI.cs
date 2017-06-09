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
                    Console.WriteLine("--" + parks[i].ToString());
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
            string userPark = CLIHelper.GetString("Please select the Park Name: ");
            ParkSQLDAL dal = new ParkSQLDAL(DatabaseConnection);
            List<Park> foundParks = dal.FindPark(userPark);

            // Display park information, if valid
            if (foundParks.Count > 0)
            {
                foreach (Park p in foundParks)
                {
                    Console.WriteLine("Park Name: " + p.ToString());
                    Console.WriteLine("Park Location: " + p.ParkLocation);
                    Console.WriteLine("Park Area: " + p.ParkArea.ToString("N0") + " sq km");
                    Console.WriteLine("Establish Date: " + p.ParkEstablishDate.ToString("y"));
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
        private void DisplayParkSubMenu(Park userParkChoice)
        {
            const string Command_ViewCampgrounds = "1";
            const string Command_SearchReservations = "2";
            const string ReturnToMainMenu = "q";

            while (true)
            {
                Console.WriteLine("What would you like to do? ");
                Console.WriteLine("(1) View campgrounds for " + userParkChoice.ToString());
                Console.WriteLine("(2) Search and Book Reservations at " + userParkChoice.ToString());
                Console.WriteLine("(Q) Quit ");

                string userCommand = Console.ReadLine();
                switch (userCommand.ToLower())
                {
                    case Command_ViewCampgrounds:
                        ViewCampgrounds(userParkChoice);
                        break;
                    case Command_SearchReservations:
                        SearchReservations(userParkChoice);
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

        private void SearchReservations(Park userParkChoice)
        {
            int userChoiceSiteNumber;
            string userReservationName;

            ViewCampgrounds(userParkChoice);
            int userChoiceCampgroundID = CLIHelper.GetInteger("Enter the desired campground ID: ");
            DateTime userChoiceStartDate = CLIHelper.GetDateTime("Enter the desired start date: (YYYY/MM/DD) ");
            DateTime userChoiceEndDate = CLIHelper.GetDateTime("Enter the desired end date: (YYYY/MM/DD) ");

            ReservationSQLDAL dal = new ReservationSQLDAL(DatabaseConnection);
            List<Site> availableSites = dal.SearchForAvailableReservations(userChoiceCampgroundID, userChoiceStartDate, userChoiceEndDate);

            CampgroundSQLDAL cgDal = new CampgroundSQLDAL(DatabaseConnection, userParkChoice.ParkID);
            decimal totalCampingCost = cgDal.GetCampgroundDailyRate(userChoiceCampgroundID);
            int totalDays = Convert.ToInt32((userChoiceEndDate - userChoiceStartDate).TotalDays);

            if (availableSites.Count > 0)
            {
                Console.WriteLine("Showing First Five Available Sites:");
                Console.WriteLine("Site No.     Max Occupancy    Accessible?    Max RV Length    Utilites?   Total Cost");
                foreach (Site site in availableSites)
                {
                    Console.WriteLine("#" + site.SiteNumber + "  " + site.MaxOccupancy + "   " + site.IsAccessible + "   " + site.MaxRVLength + "   " + site.HasUtilities + "   " + (totalCampingCost * totalDays).ToString("C2"));
                }

                userChoiceSiteNumber = CLIHelper.GetInteger("Which site should be reserved (enter 0 to cancel)? ");

                if (userChoiceSiteNumber == 0)
                {
                    return;
                }

                userReservationName = CLIHelper.GetString("What name should the reservation be made under? ");

                BookReservation(userChoiceCampgroundID, userChoiceStartDate, userChoiceEndDate, userChoiceSiteNumber, userReservationName);
                
            }
            else
            {
                Console.WriteLine("**** NO RESULTS ****");
            }
            
            
        }

        private void BookReservation(int userChoiceCampgroundID, DateTime userChoiceStartDate, DateTime userChoiceEndDate, int userChoiceSiteNumber, string userReservationName)
        {
            ReservationSQLDAL dal = new ReservationSQLDAL(DatabaseConnection);
            int userReservationID = dal.BookReservation(userChoiceCampgroundID, userChoiceStartDate, userChoiceEndDate, userChoiceSiteNumber, userReservationName);

            Console.WriteLine("Your reservation has been made! Your reservation number is: " + userReservationID.ToString());
        }

        private void ViewCampgrounds(Park userParkChoice)
        {
            CampgroundSQLDAL dal = new CampgroundSQLDAL(DatabaseConnection, userParkChoice.ParkID);
            List<Campground> allCampgrounds = dal.GetCampgrounds();
            Dictionary<int, string> allMonths = new Dictionary<int, string>
            {
                {1, "January" },
                {2, "February" },
                {3, "March" },
                {4, "April" },
                {5, "May" },
                {6, "June" },
                {7, "July" },
                {8, "August" },
                {9, "September" },
                {10, "October" },
                {11, "November" },
                {12, "December" }
            };

            Console.WriteLine("Showing all campgrounds for " + userParkChoice.ToString() + "\n");
            Console.WriteLine("Campground ID    Name    Open    Close   Daily Fee");

            foreach(Campground camp in allCampgrounds)
            {
                Console.WriteLine("#" + camp.CampgroundID + camp.CampgroundName + allMonths[camp.OpenFromMonth] + allMonths[camp.OpenToMonth] + camp.DailyFee.ToString("C2") );
            }
        }
    }
}
