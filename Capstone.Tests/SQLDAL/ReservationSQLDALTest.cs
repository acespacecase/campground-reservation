using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Transactions;
using System.Configuration;
using Capstone.DAL;
using Capstone.Models;

namespace Capstone.Tests.SQLDAL
{
    [TestClass]
    public class ReservationSQLDALTest
    {
        TransactionScope tr;
        readonly string connectionString = ConfigurationManager.ConnectionStrings["CapstoneDatabase"].ConnectionString;
        int parkID;
        int campgroundID;
        int siteID;

        DateTime start = new DateTime(2017, 06, 01);
        DateTime end = new DateTime(2017, 06, 03);
        

        [TestInitialize]
        public void Initialize()
        {
            tr = new TransactionScope();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO park VALUES('sample_name', 'USA', '2011-01-01', 500000, 100, 'Literally the greatest description ever'); SELECT SCOPE_IDENTITY();", connection);
                parkID = Convert.ToInt32(cmd.ExecuteScalar());

                cmd = new SqlCommand("INSERT INTO campground VALUES(@parkID, 'name',10, 12, 9.00); SELECT SCOPE_IDENTITY();", connection);
                cmd.Parameters.AddWithValue("@parkID", parkID);
                campgroundID = Convert.ToInt32(cmd.ExecuteScalar());

                cmd = new SqlCommand("INSERT INTO site VALUES(@campgroundID, 1001, 6, 1, 10, 0);SELECT SCOPE_IDENTITY();", connection);
                cmd.Parameters.AddWithValue("@campgroundID", campgroundID);
                siteID = Convert.ToInt32(cmd.ExecuteScalar());

                cmd = new SqlCommand("INSERT INTO reservation VALUES(@siteID,'no_conflicts','2017-04-02','2017-05-01', @dateNow);", connection);
                cmd.Parameters.AddWithValue("@siteID", siteID);
                cmd.Parameters.AddWithValue("@dateNow", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            tr.Dispose();
        }

        [TestMethod]
        public void SearchForAvailableSitesTest()
        {
            ReservationSQLDAL dal = new ReservationSQLDAL(connectionString);
            List<Site> validSites = dal.SearchReservationByCampground(campgroundID, start, end);

            Assert.IsNotNull(validSites);
            Assert.IsTrue(validSites.Count > 0);
        }
    }
}
