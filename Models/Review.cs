using System.ComponentModel.DataAnnotations;

namespace HotelPortal.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int HotelId { get; set; }

        public int UserId { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public int Stars { get; set; }

        public Hotel Hotel { get; set; }
        public SiteUser SiteUser { get; set; }
    }
}
