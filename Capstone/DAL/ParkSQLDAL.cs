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
    }
}
