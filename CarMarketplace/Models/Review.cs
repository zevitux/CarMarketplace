namespace CarMarketplace.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public int CarId { get; set; }
        public Car? Car { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
