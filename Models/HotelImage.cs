using System;
using System.ComponentModel.DataAnnotations;

namespace HotelPortal.Models
{
    public class HotelImage
    {
        public int Id { get; set; }

        public int HotelId { get; set; }

        public byte[] Content { get; set; }

        [Display(Name = "File Name")]
        public string UntrustedName { get; set; }

        public Boolean IsMain { get; set; }

        public Hotel Hotel { get; set; }
    }
}
