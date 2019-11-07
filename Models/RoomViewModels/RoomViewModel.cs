using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelPortal.Models.RoomViewModels
{
    public class RoomViewModel
    {
        public int HotelId;
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int Price { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int Capacity { get; set; }

        public List<IFormFile> Images { get; set; }
    }
}
