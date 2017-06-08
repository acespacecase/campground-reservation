using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Models
{
    public class Park
    {
        public int ParkID { get; set; }
        public string ParkName { get; set; }
        public string ParkDescription { get; set; }
        public string ParkLocation { get; set; }
        public int ParkArea { get; set; }
        public DateTime ParkEstablishDate { get; set; }
        public int VisitorCount { get; set; }

        public override string ToString()
        {
            return (this.ParkName);
        }
    }
}
