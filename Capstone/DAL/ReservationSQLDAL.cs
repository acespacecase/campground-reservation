using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Capstone.Models;

namespace Capstone.DAL
{
    public class ReservationSQLDAL
    {
        private string connectionString;

        public ReservationSQLDAL(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public List<Site> SearchReservationByCampground(int campgroundID, DateTime startDate, DateTime endDate)
        {
            List<Site> availableSites = new List<Site>();

            string sqlQueryForAvailableSites = "SELECT TOP 5 site.site_id, site.campground_id, site.max_occupancy, " +
                "site.max_rv_length, site.accessible, site.site_number, site.utilities FROM site LEFT JOIN reservation ON reservation.site_id = site.site_id " +
                "WHERE site.campground_id = @campgroundID AND site.site_id NOT IN (SELECT reservation.site_id FROM reservation " +
                "WHERE (reservation.reservation_id IS NULL OR ((reservation.from_date BETWEEN @startDate AND @endDate) " +
                "AND (reservation.to_date BETWEEN @startDate AND @endDate)) OR (reservation.from_date < @startDate AND reservation.to_date > @endDate)))" +
                "GROUP BY site.site_id, site.campground_id, site.max_occupancy, site.max_rv_length, site.accessible, site.site_number, site.utilities;";


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(sqlQueryForAvailableSites, connection);
                    cmd.Parameters.AddWithValue("@campgroundID", campgroundID);
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        availableSites.Add(PopulateSite(reader));
                    }
                }
            }
            catch
            {
                Console.WriteLine("There was an error connecting to the database. Check your values!");
            }


            return availableSites;
        }
        public Dictionary<Site, Campground> SearchReservationByDate(int userParkChoice, DateTime startDate, DateTime endDate)
        {
            Dictionary<Site, Campground> allOpenSites = new Dictionary<Site, Campground>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlQueryForAvailableSites = 
                     "SELECT campground.name, site.site_id, site.campground_id, site.max_occupancy, " +
                     "site.max_rv_length, site.accessible, site.site_number, site.utilities FROM site " + 
                         "LEFT JOIN reservation ON reservation.site_id = site.site_id " +
                         "INNER JOIN campground ON site.campground_id = campground.campground_id " +
                         "INNER JOIN park ON campground.park_id = park.park_id " +
                     "WHERE park.park_id = @parkID AND site.site_id NOT IN (SELECT reservation.site_id FROM reservation " +
                     "WHERE (reservation.reservation_id IS NULL OR ((reservation.from_date BETWEEN @startDate AND @endDate) " +
                     "AND (reservation.to_date BETWEEN @startDate AND @endDate)) OR (reservation.from_date < @startDate AND reservation.to_date > @endDate)))" +
                     "GROUP BY campground.name, site.site_id, site.campground_id, site.max_occupancy, site.max_rv_length, site.accessible, site.site_number, site.utilities;";
                    SqlCommand cmd = new SqlCommand(sqlQueryForAvailableSites, connection);
                    cmd.Parameters.AddWithValue("@parkID", userParkChoice);
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);
                    SqlDataReader reader = cmd.ExecuteReader();

                    CampgroundSQLDAL dal = new CampgroundSQLDAL(connectionString, userParkChoice);
                    Dictionary<int, Campground> campgroundMap = dal.PopulateCampgroundMap(userParkChoice);

                    while (reader.Read())
                    {
                        allOpenSites.Add(PopulateSite(reader), campgroundMap[Convert.ToInt32(reader["campground_id"])]);
                    }
                }
            }
            catch
            {
                Console.WriteLine("There was an error connecting to the database. Check your values!");
            }

            return allOpenSites;
        }

        public int BookReservation(int userChoiceCampgroundID, DateTime userChoiceStartDate, DateTime userChoiceEndDate, int userChoiceSiteNumber, string userReservationName)
        {
            int userReservationID = 0;
            int userChoiceSiteID;

            if (userChoiceStartDate > userChoiceEndDate)
            {
                throw new Exception("Invalid dates!");
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM site WHERE campground_id = @campgroundID AND site_number = @siteNumber;", connection);
                    cmd.Parameters.AddWithValue("@campgroundID", userChoiceCampgroundID);
                    cmd.Parameters.AddWithValue("@siteNumber", userChoiceSiteNumber);
                    userChoiceSiteID = Convert.ToInt32(cmd.ExecuteScalar());

                    cmd = new SqlCommand("INSERT INTO reservation VALUES(@siteID, @resName, @startDate, @endDate, @createDate); SELECT SCOPE_IDENTITY();", connection);
                    cmd.Parameters.AddWithValue("@siteID", userChoiceSiteID);
                    cmd.Parameters.AddWithValue("@resName", userReservationName);
                    cmd.Parameters.AddWithValue("@startDate", userChoiceStartDate);
                    cmd.Parameters.AddWithValue("@endDate", userChoiceEndDate);
                    cmd.Parameters.AddWithValue("@createDate", DateTime.Now);
                    userReservationID = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch
            {
                Console.WriteLine("There was an error connecting to the database. Check your values!");
            }

            if (userReservationID == 0)
            {
                throw new Exception("There was an error making the reservation.");
            }
            return userReservationID;
        }

        private Site PopulateSite(SqlDataReader reader)
        {
            return new Site
            {
                SiteID = Convert.ToInt32(reader["site_id"]),
                CampgroundID = Convert.ToInt32(reader["campground_id"]),
                SiteNumber = Convert.ToInt32(reader["site_number"]),
                MaxOccupancy = Convert.ToInt32(reader["max_occupancy"]),
                IsAccessible = Convert.ToInt32(reader["accessible"]),
                MaxRVLength = Convert.ToInt32(reader["max_rv_length"]),
                HasUtilities = Convert.ToInt32(reader["utilities"])
            };
        }

    }
}
