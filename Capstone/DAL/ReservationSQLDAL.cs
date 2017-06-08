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

        public List<Site> SearchForAvailableReservations(int campgroundID, DateTime startDate, DateTime endDate)
        {
            List<Site> availableSites = new List<Site>();

            string sqlQueryForAvailableSites = "SELECT TOP 5 site.site_id, site.campground_id, site.max_occupancy, " +
                "site.max_rv_length, site.accessible, site.site_number, site.utilities FROM site LEFT JOIN reservation ON reservation.site_id = site.site_id " +
                "WHERE site.campground_id = @campgroundID AND (reservation.reservation_id IS NULL OR " +
                " (reservation.to_date < @startDate OR reservation.from_date > @endDate)) " +
                "GROUP BY site.site_id, site.campground_id, site.max_occupancy, site.max_rv_length, site.accessible, site.site_number, site.utilities;";
            
            try
            {
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(sqlQueryForAvailableSites, connection);
                    cmd.Parameters.AddWithValue("@campgroundID", campgroundID);
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while(reader.Read())
                    {
                        availableSites.Add(PopulateSite(reader));
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("There was an error connecting to the database: " + ex.Message);
            }


            return availableSites;
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
