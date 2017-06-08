using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.Models;
using System.Data.SqlClient;

namespace Capstone.DAL
{
    public class CampgroundSQLDAL
    {
        private string connectionString;
        private int parkID;
        private int campgroundID;

        public CampgroundSQLDAL(string dbConnectionString, int parkID)
        {
            connectionString = dbConnectionString;
            this.parkID = parkID;
        }

        public CampgroundSQLDAL(int campgroundID, string dbConnectionString)
        {
            this.campgroundID = campgroundID;
            connectionString = dbConnectionString;
        }

        public List<Campground> GetCampgrounds()
        {
            List<Campground> output = new List<Campground>();

            try
            {
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM campground WHERE park_id = @parkID;", connection);
                    cmd.Parameters.AddWithValue("@parkID", parkID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        output.Add(PopulateCampground(reader));
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("There was an error connecting to the database: " + ex.Message);
            }

            return output;
        }

        public decimal GetCampgroundDailyRate()
        {
            decimal dailyRate = 0.0M;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT campground.daily_fee FROM campground WHERE campground_id = @campgroundID;", connection);
                    cmd.Parameters.AddWithValue("@campgroundID", campgroundID);
                    dailyRate = Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("There was an error connecting to the database: " + ex.Message);
            }

            return dailyRate;
        }

        private Campground PopulateCampground(SqlDataReader reader)
        {
            return new Campground
            {
                CampgroundID = Convert.ToInt32(reader["campground_id"]),
                ParkID = Convert.ToInt32(reader["park_id"]),
                CampgroundName = Convert.ToString(reader["name"]),
                OpenFromMonth = Convert.ToInt32(reader["open_from_mm"]),
                OpenToMonth = Convert.ToInt32(reader["open_to_mm"]),
                DailyFee = Convert.ToDecimal(reader["daily_fee"])
            };
        }
    }
}
