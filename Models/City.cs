using System.Collections.Generic;

namespace HotelPortal.Models
{
    public class City
    {
        public City()
        {
            Hotels = new HashSet<Hotel>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Hotel> Hotels { get; set; }
    }
}
