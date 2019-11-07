using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace HotelPortal.Models
{
    public class SiteUser : IdentityUser<int>
    {
        public SiteUser()
        {
            Bookings = new HashSet<Booking>();
            Reviews = new HashSet<Review>();
        }
        public string Name { get; set; }
        public string Address { get; set; }
        public string IDNumber { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Review> Reviews { get; set; }

    }
}
