using System.Security.Claims;
using CarMarketplace.DTOs;
using CarMarketplace.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarMarketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly ICarService _carService;
        private readonly ILogger<CarController> _logger;

        public CarController(ICarService carService, ILogger<CarController> logger)
        {
            _carService = carService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<CarResponseDto>>> GetAllCars()
        {
            _logger.LogInformation("Fetching all cars.");
            var cars = await _carService.GetAllCarsAsync();
            return Ok(cars);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CarResponseDto>> GetCarById(int id)
        {
            _logger.LogInformation("Fetching car with id {Id}.", id);
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null)
            {
                _logger.LogWarning("Car with id {Id} not found.", id);
                return NotFound();
            }

            return Ok(car);
        }

        [HttpPost]
        [Authorize(Roles = "Seller")] // Restrict to Seller role
        public async Task<ActionResult<CarResponseDto>> CreateCar(CarCreateRequestDto request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            _logger.LogInformation("Creating a new car for user with id {UserId}.", userId);
            var car = await _carService.CreateCarAsync(request, userId);
            return CreatedAtAction(nameof(GetCarById), new { id = car.Id }, car);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Seller")] // Restrict to Seller role
        public async Task<IActionResult> UpdateCar(int id, UpdateCarRequestDto request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            _logger.LogInformation("Updating car with id {CarId} for user with id {UserId}.", id, userId);
            await _carService.UpdateCarAsync(id, request, userId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Seller")] // Restrict to Seller role
        public async Task<IActionResult> DeleteCar(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            _logger.LogInformation("Deleting car with id {CarId} for user with id {UserId}.", id, userId);
            await _carService.DeleteCarAsync(id, userId);
            return NoContent();
        }
    }
}