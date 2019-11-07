using HotelPortal.Models.AccountViewModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace HotelPortal.Models.BookingViewModels
{
    public class BookingViewModel : SiteUserViewModel
    {

        public Room Room { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FirstDay { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime LastDay { get; set; }

        [DataType(DataType.Currency)]
        public double Total { get; set; }
        public int GuestCount { get; set; }
    }
}
