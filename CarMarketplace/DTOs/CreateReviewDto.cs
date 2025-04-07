using System.ComponentModel.DataAnnotations;

namespace CarMarketplace.DTOs
{
    public class CreateReviewDto
    {
        [Range(1, 5)]
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public int CarId { get; set; }
    }
}
