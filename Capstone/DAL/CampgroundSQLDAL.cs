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

        public CampgroundSQLDAL(string dbConnectionString, int parkID)
        {
            connectionString = dbConnectionString;
            this.parkID = parkID;
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
            catch
            {
                Console.WriteLine("There was an error connecting to the database. Check your values!");
            }

            return output;
        }

        public decimal GetCampgroundDailyRate(int userChoiceCampgroundID)
        {
            decimal dailyRate = 0.0M;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT campground.daily_fee FROM campground WHERE campground_id = @campgroundID;", connection);
                    cmd.Parameters.AddWithValue("@campgroundID", userChoiceCampgroundID);
                    dailyRate = Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
            catch
            {
                Console.WriteLine("There was an error connecting to the database. Check your values!");
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
        public Dictionary<int, Campground> PopulateCampgroundMap(int userParkID)
        {
            Dictionary<int, Campground> campgroundMap = new Dictionary<int, Campground>();

            CampgroundSQLDAL dal = new CampgroundSQLDAL(connectionString, userParkID);
            List<Campground> allCampgrounds = dal.GetCampgrounds();

            foreach (Campground c in allCampgrounds)
            {
                campgroundMap.Add(c.CampgroundID, c);
            }
            return campgroundMap;
        }

        public int GetCampgroundIDFromName(string name)
        {
            int campgroundID = 0;

            try
            {
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand("SELECT campground_id FROM campground WHERE name = @name;", connection);
                    cmd.Parameters.AddWithValue("@name", name);
                    campgroundID = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch
            {
                Console.WriteLine("There was an error connecting to the database. Check your values!");
            }

            if (campgroundID == 0)
            {
                throw new Exception();
            }

            return campgroundID;
        }
    }
}
