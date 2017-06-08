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
    public class ParkSQLDALTest
    {
        TransactionScope tr;
        readonly string connectionString = ConfigurationManager.ConnectionStrings["CapstoneDatabase"].ConnectionString;
        const string parkName = "sample_name";
        

        [TestInitialize]
        public void Initialize()
        {
            tr = new TransactionScope();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO park VALUES(@sample_name, 'USA', '2011-01-01', 500000, 100, 'Literally the greatest description ever');", connection);
                cmd.Parameters.AddWithValue("@sample_name", parkName);
                cmd.ExecuteNonQuery();
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            tr.Dispose();
        }


        [TestMethod]
        public void GetParksTest()
        {
            ParkSQLDAL dal = new ParkSQLDAL(connectionString);
            List<Park> parksList = dal.GetParks();

            Assert.IsNotNull(parksList);
            Assert.IsTrue(parksList.Count > 0);
        }

        [TestMethod]
        public void FindParkTest()
        {
            ParkSQLDAL dal = new ParkSQLDAL(connectionString);
            List<Park> parksList = dal.FindPark(parkName);

            Assert.IsNotNull(parksList);
            Assert.IsTrue(parksList.Count > 0);
            Assert.AreEqual(parkName, parksList[0].ParkName);
        }
    }
}
