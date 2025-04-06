namespace CarMarketplace.DTOs
{
    public class CarResponseDto
    {
        private List<ReviewDto>? reviews;

        public int Id { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public int Mileage { get; set; }
        public bool IsUsed { get; set; }
        public string? Description { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public List<ReviewDto>? Reviews { get => reviews; set => reviews = value; }
    }
}
