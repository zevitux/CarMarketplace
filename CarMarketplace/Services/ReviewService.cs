using CarMarketplace.Data;
using CarMarketplace.DTOs;
using CarMarketplace.Models;
using Microsoft.EntityFrameworkCore;

namespace CarMarketplace.Services
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(AppDbContext context, ILogger<ReviewService> logger)
        {
            _context = context;
            _logger = logger;
        }

        //Create a new review
        public async Task<ReviewDto> CreateReviewAsync(CreateReviewDto request, int userId)
        {
            _logger.LogInformation("Creating a new review for user with id {UserId}.", userId);
            var review = new Review
            {
                Rating = request.Rating,
                Comment = request.Comment,
                UserId = userId,
                CarId = request.CarId
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Review created with id {ReviewId}.", review.Id);
            return new ReviewDto
            {
                Id = review.Id,
                Rating = review.Rating,
                Comment = review.Comment,
                UserId = review.UserId,
                UserName = (await _context.Users.FindAsync(userId))?.Name
            };
        }

        //Update a review
        public async Task UpdateReviewAsync(int reviewId, UpdateReviewDto request, int userId)
        {
            _logger.LogInformation("Updating review with id {ReviewId} for user with id {UserId}.", reviewId, userId);

            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId);
            if(review == null)
            {
                _logger.LogWarning("Review with id {ReviewId} not found for user with id {UserId}.", reviewId, userId);
                throw new KeyNotFoundException("Review not found.");
            }

            review.Rating = request.Rating;
            review.Comment = request.Comment;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Review with id {ReviewId} updated.", reviewId);
        }

        //Delete review
        public async Task DeleteReviewAsync(int reviewId, int userId)
        {
            _logger.LogInformation("Deleting review with id {ReviewId} for user with id {UserId}.", reviewId, userId);
            
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId);
            if (review == null)
            {
                _logger.LogWarning("Review with id {ReviewId} not found for user with id {UserId}.", reviewId, userId);
                throw new KeyNotFoundException("Review not found.");
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Review with id {ReviewId} deleted.", reviewId);
        }

        //Get reviews by car id
        public async Task<List<ReviewDto>> GetReviewsByCarIdAsync(int carId)
        {
            _logger.LogInformation("Fetching reviews for car with id {CarId}.", carId);
            
            var reviews = await _context.Reviews
                .Where(r => r.CarId == carId)
                .Include(r => r.User)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    UserId = r.UserId,
                    UserName = r.User!.Name
                })
                .ToListAsync();

            _logger.LogInformation("Fetched {Count} reviews for car with id {CarId}.", reviews.Count, carId);
            return reviews;
        }

        //Get review by id
        public async Task<ReviewDto> GetReviewByIdAsync(int reviewId)
        {
            _logger.LogInformation("Fetching review with id {ReviewId}.", reviewId);

            var review = await _context.Reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null)
            {
                _logger.LogWarning("Review with id {ReviewId} not found.", reviewId);
                return null!;
            }
            
            _logger.LogInformation("Fetched review with id {ReviewId}.", reviewId);
            return new ReviewDto
            {
                Id = review.Id,
                Rating = review.Rating,
                Comment = review.Comment,
                UserId = review.UserId,
                UserName = review.User!.Name
            };
        }
    }
}
