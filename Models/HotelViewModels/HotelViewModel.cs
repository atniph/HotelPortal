using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelPortal.Models.HotelViewModels
{
    public class HotelViewModel
    {
        [Required]
        public string Name { get; set; }
        public int CityId { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Stars { get; set; }

        public List<IFormFile> Images { get; set; }
    }
}
