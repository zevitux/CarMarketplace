using CarMarketplace.DTOs;

namespace CarMarketplace.Services
{
    public interface IReviewService
    {
        Task<ReviewDto> CreateReviewAsync(CreateReviewDto request, int userId);
        Task UpdateReviewAsync(int reviewId, UpdateReviewDto request, int userId);
        Task DeleteReviewAsync(int reviewId, int userId);
        Task<List<ReviewDto>> GetReviewsByCarIdAsync(int carId);
        Task<ReviewDto> GetReviewByIdAsync(int reviewId);
    }
}