using System;
using System.ComponentModel.DataAnnotations;

namespace HotelPortal.Models
{
    public class RoomImage
    {
        public int Id { get; set; }

        public int RoomId { get; set; }

        public byte[] Content { get; set; }

        [Display(Name = "File Name")]
        public string UntrustedName { get; set; }

        public Boolean IsMain { get; set; }

        public Room Hotel { get; set; }
    }
}
