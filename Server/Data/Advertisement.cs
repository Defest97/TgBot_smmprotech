using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Data
{
    public class Advertisement
    {
        public int Id { get; set; }
        public int RoomCount { get; set; }
        public double Area { get; set; }
        public int Floor { get; set; }
        public int TotalFloors { get; set; }
        public string Bank { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string MetroStation { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public ICollection<Photo> Photos { get; set; }
    }

}
