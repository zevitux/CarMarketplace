namespace CarMarketplace.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? Role { get; set; } //Admin, Seller and Buyer
        public string? RefreshToken { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public List<Car>? Cars { get; set; }
        public List<Review>? Reviews { get; set; }
    }
}
