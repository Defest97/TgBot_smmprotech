using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Data
{
    public class Photo
    {
        public int Id { get; set; }
        public byte[] Img { get; set; }
        public int AdvertisementId { get; set; }
        public Advertisement Advertisement { get; set; }
    }

}
