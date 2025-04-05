using System.ComponentModel.DataAnnotations;

namespace CarMarketplace.DTOs
{
    public class UpdateCarRequestDto
    {
        public string? Brand { get; set; }
        public string? Model { get; set; }
        [Range(1900, 2025)]
        public int? Year { get; set; }
    }
}
