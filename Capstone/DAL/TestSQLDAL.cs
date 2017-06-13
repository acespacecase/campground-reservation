//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Capstone.Models;
//using System.Data.SqlClient;

//namespace Capstone.DAL
//{
//    public class TestSQLDAL
//    {
//        private string connectionString;

//        public TestSQLDAL(string DatabaseConnection)
//        {
//            this.connectionString = DatabaseConnection;
//        }

//        public List<Tuple<int, string, Reservation>> GetNextMonthsReservations(Park userParkChoice, ref List<int> siteNumbers, ref List<string> campgroundNames, ref List<Reservation> nextMonthsReservations)
//        {
//            DateTime today = DateTime.Now;
//            DateTime thirtyDaysFromToday = today.AddDays(30);
//            List<Tuple<int, string, Reservation>> output = new List<Tuple<int, string, Reservation>>();


//            try
//            {
//                using (SqlConnection connection = new SqlConnection(connectionString))
//                {
//                    connection.Open();
//                    // We want to know, Reservation ID, Reservation Name, Reservation From Date/To Date, Site Name, Campground Name

//                    string sqlQuery = "SELECT reservation.reservation_id, reservation.name AS resname, reservation.from_date, reservation.to_date, " +
//                                      "site.site_number, site.site_id, campground.name AS name, reservation.create_date FROM reservation " +
//                                      "INNER JOIN site on site.site_id = reservation.site_id " +
//                                      "INNER JOIN campground on campground.campground_id = site.campground_id " +
//                                      "WHERE campground.park_id = @userParkID AND " +
//                                      "reservation.from_date BETWEEN @today AND @thirty_days_from_today";
//                    SqlCommand cmd = new SqlCommand(sqlQuery, connection);
//                    cmd.Parameters.AddWithValue("@userParkID", userParkChoice.ParkID);
//                    cmd.Parameters.AddWithValue("@today", today);
//                    cmd.Parameters.AddWithValue("@thirty_days_from_today", thirtyDaysFromToday);
//                    SqlDataReader reader = cmd.ExecuteReader();

//                    while (reader.Read())
//                    {
//                        output.Add(new Tuple<int, string, Reservation> { })
//                        siteNumbers.Add(Convert.ToInt32(reader["site_number"]));
//                        campgroundNames.Add(Convert.ToString(reader["name"]));
//                        nextMonthsReservations.Add(PopulateReservation(reader));
//                    }
//                }
//            }
//            catch (SqlException ex)
//            {
//                Console.WriteLine("There was an error connecting to the database: " + ex.Message);
//                throw;
//            }
//        }


//        private Reservation PopulateReservation(SqlDataReader reader)
//        {
//            return new Reservation
//            {
//                ReservationID = Convert.ToInt32(reader["reservation_id"]),
//                SiteID = Convert.ToInt32(reader["site_id"]),
//                ReservationName = Convert.ToString(reader["resname"]),
//                FromDate = Convert.ToDateTime(reader["from_date"]),
//                ToDate = Convert.ToDateTime(reader["to_date"]),
//                CreateDate = Convert.ToDateTime(reader["create_date"])
//            };
//        }

//    }
//}
