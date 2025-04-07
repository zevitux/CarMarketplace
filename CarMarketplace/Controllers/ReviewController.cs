using System.Security.Claims;
using CarMarketplace.DTOs;
using CarMarketplace.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarMarketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewController> _logger;
        public ReviewController(IReviewService reviewService, ILogger<ReviewController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReviewDto>> CreateReview(CreateReviewDto request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            _logger.LogInformation("Creating a new review for user with id {UserId}.", userId);
            var review = await _reviewService.CreateReviewAsync(request, userId);
            return CreatedAtAction(nameof(GetReviewById), new { id = review.Id }, review);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(int id, UpdateReviewDto request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            _logger.LogInformation("Updating review with id {ReviewId} for user with id {UserId}.", id, userId);
            await _reviewService.UpdateReviewAsync(id, request, userId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            _logger.LogInformation("Deleting review with id {ReviewId} for user with id {UserId}.", id, userId);
            await _reviewService.DeleteReviewAsync(id, userId);
            return NoContent();
        }

        [HttpGet("car/{carId}")]
        [Authorize]
        public async Task<ActionResult<List<ReviewDto>>> GetReviewsByCarId(int carId)
        {
            _logger.LogInformation("Fetching reviews for car with id {CarId}.", carId);
            var reviews = await _reviewService.GetReviewsByCarIdAsync(carId);
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ReviewDto>> GetReviewById(int id)
        {
            _logger.LogInformation("Fetching review with id {Id}.", id);       
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null)
            {
                _logger.LogWarning("Review with id {Id} not found.", id);
                return NotFound();
            }

            return Ok(review);
        }
    }
}
