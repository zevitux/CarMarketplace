using System.ComponentModel.DataAnnotations;

namespace CarMarketplace.DTOs
{
    public class UpdateCarRequestDto
    {
        public string? Brand { get; set; }
        public string? Model { get; set; }
        [Range(1900, 2025)]
        public int? Year { get; set; }
        public decimal? Price { get; set; }
        public int? Mileage { get; set; }
        public bool? IsUsed { get; set; }
        public string? Description { get; set; }
    }
}