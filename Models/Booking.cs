using System;

namespace HotelPortal.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int SiteUserId { get; set; }
        public int RoomId { get; set; }
        public int GuestCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Room Room { get; set; }
        public SiteUser SiteUser { get; set; }

    }
}
