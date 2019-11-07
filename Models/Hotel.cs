using System.Collections.Generic;

namespace HotelPortal.Models
{
    public class Hotel
    {
        public Hotel()
        {
            Rooms = new HashSet<Room>();
            Reviews = new HashSet<Review>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int CityId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Description { get; set; }
        public int Stars { get; set; }

        public ICollection<Room> Rooms { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public City City { get; set; }
    }
}
