using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Capstone.Models;

namespace Capstone.DAL
{
    public class ParkSQLDAL 
    {
        private string connectionString;

        public ParkSQLDAL(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }
        public List<Park> GetParks()
        {
            List<Park> output = new List<Park>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM park;", connection);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        output.Add(PopulatePark(reader));
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error connecting to database. Check your values. ");
            }

            return output;
        }
        public List<Park> FindPark(string userPark)
        {
            List<Park> output = new List<Park>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM park WHERE name = @parkName", connection);
                    cmd.Parameters.AddWithValue("@parkName", userPark);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        output.Add(PopulatePark(reader));
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error connecting to database. Check your values!");
            }

            return output;
        }
        public Park PopulatePark(SqlDataReader reader)
        {
            return new Park
            {
                ParkID = Convert.ToInt32(reader["park_id"]),
                ParkName = Convert.ToString(reader["name"]),
                ParkDescription = Convert.ToString(reader["description"]),
                ParkLocation = Convert.ToString(reader["location"]),
                ParkArea = Convert.ToInt32(reader["area"]),
                ParkEstablishDate = Convert.ToDateTime(reader["establish_date"]),
                VisitorCount = Convert.ToInt32(reader["visitors"])
            };
        }
        public void GetNextMonthsReservations(Park userParkChoice, ref List<int> siteNumbers, ref List<string> campgroundNames, ref List<Reservation> nextMonthsReservations)
        {
            DateTime today = DateTime.Now;
            DateTime thirtyDaysFromToday = today.AddDays(30);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // We want to know, Reservation ID, Reservation Name, Reservation From Date/To Date, Site Name, Campground Name

                    string sqlQuery = "SELECT reservation.reservation_id, reservation.name AS resname, reservation.from_date, reservation.to_date, " +
                                      "site.site_number, site.site_id, campground.name AS name, reservation.create_date FROM reservation " +
                                      "INNER JOIN site on site.site_id = reservation.site_id " +
                                      "INNER JOIN campground on campground.campground_id = site.campground_id " +
                                      "WHERE campground.park_id = @userParkID AND " +
                                      "reservation.from_date BETWEEN @today AND @thirty_days_from_today";
                    SqlCommand cmd = new SqlCommand(sqlQuery, connection);
                    cmd.Parameters.AddWithValue("@userParkID", userParkChoice.ParkID);
                    cmd.Parameters.AddWithValue("@today", today);
                    cmd.Parameters.AddWithValue("@thirty_days_from_today", thirtyDaysFromToday);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        siteNumbers.Add(Convert.ToInt32(reader["site_number"]));
                        campgroundNames.Add(Convert.ToString(reader["name"]));
                        nextMonthsReservations.Add(PopulateReservation(reader));
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("There was an error connecting to the database: " + ex.Message);
                throw;
            }

            
        }
        private Reservation PopulateReservation(SqlDataReader reader)
        {
            return new Reservation
            {
                ReservationID = Convert.ToInt32(reader["reservation_id"]),
                SiteID = Convert.ToInt32(reader["site_id"]),
                ReservationName = Convert.ToString(reader["resname"]),
                FromDate = Convert.ToDateTime(reader["from_date"]),
                ToDate = Convert.ToDateTime(reader["to_date"]),
                CreateDate = Convert.ToDateTime(reader["create_date"])             
            };
        }
    }
}
