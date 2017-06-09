using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Transactions;
using System.Configuration;
using System.Data.SqlClient;
using Capstone.DAL;
using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.Tests.SQLDAL
{
    [TestClass]
    public class CampgroundSQLDALTest
    {
        TransactionScope tr;
        readonly string connectionString = ConfigurationManager.ConnectionStrings["CapstoneDatabase"].ConnectionString;
        int parkID;
        int campgroundID;

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

                cmd = new SqlCommand("INSERT INTO campground VALUES(@parkID, 'campground name', 1, 12, 100.00); SELECT SCOPE_IDENTITY();", connection);
                cmd.Parameters.AddWithValue("@parkID", parkID);
                campgroundID = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            tr.Dispose();
        }

        [TestMethod]
        public void GetCampgroundsTest()
        {
            CampgroundSQLDAL dal = new CampgroundSQLDAL(connectionString, parkID);
            List<Campground> allCampgroundsInPark = dal.GetCampgrounds();

            Assert.IsNotNull(allCampgroundsInPark);
            Assert.IsTrue(allCampgroundsInPark.Count > 0);
        }

        [TestMethod]
        public void GetCampgroundDailyRateTest()
        {
            CampgroundSQLDAL dal = new CampgroundSQLDAL(connectionString, parkID);
            decimal campgroundDailyRate = dal.GetCampgroundDailyRate(campgroundID);

            Assert.IsNotNull(campgroundDailyRate);
            Assert.IsTrue(campgroundDailyRate == 100.00M);
        }
    }
}
