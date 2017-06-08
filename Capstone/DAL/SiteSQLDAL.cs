using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Capstone.Models;

namespace Capstone.DAL
{
    public class SiteSQLDAL 
    {
        private string connectionString;

        public SiteSQLDAL(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public List<Site> SearchForSites(Campground campground, DateTime start, DateTime end)
        {
            List<Site> output = new List<Site>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //SqlCommand cmd = new SqlCommand("SELECT * FROM site INNER JOIN reservation ON reservation.site_id = site.site_id WHERE reservation.from_date", connection);
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("There was an error connecting to the database: " + ex.Message);
            }

            return output;
        }
        
    }
}
