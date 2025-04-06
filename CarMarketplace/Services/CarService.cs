using CarMarketplace.Data;
using CarMarketplace.DTOs;
using CarMarketplace.Models;
using Microsoft.EntityFrameworkCore;

namespace CarMarketplace.Services
{
    public class CarService : ICarService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CarService> _logger;

        public CarService(AppDbContext context, ILogger<CarService> logger)
        {
            _context = context;
            _logger = logger;
        }

        //Get all cars
        public async Task<List<CarResponseDto>> GetAllCarsAsync()
        {
            _logger.LogInformation("Fetching all cars from the database.");

            var cars = await _context.Cars
                .Include(u => u.User) //Include user info
                .Include(r => r.Reviews) //Include reviews
                .Select(c => new CarResponseDto
                {
                    Id = c.Id,
                    Brand = c.Brand,
                    Model = c.Model,
                    Year = c.Year,
                    Price = c.Price,
                    Mileage = c.Mileage,
                    IsUsed = c.IsUsed,
                    Description = c.Description,
                    UserId = c.UserId,
                    UserName = c.User.Name,
                    Reviews = c.Reviews.Select(r => new ReviewDto
                    {
                        Id = r.Id,
                        Rating = r.Rating,
                        Comment = r.Comment,
                        UserId = r.UserId,
                        UserName = r.User.Name
                    }).ToList()
                }).ToListAsync();

            _logger.LogInformation("Fetched all cars from the database.");
            return cars;
        }

        //Get car by id
        public async Task<CarResponseDto> GetCarByIdAsync(int id)
        {
            _logger.LogInformation("Fetching car with id {Id} from the database.", id);
            var car = await _context.Cars
                .Include(u => u.User) //Include user info
                .Include(r => r.Reviews) //Include reviews
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
            {
                _logger.LogWarning($"Car with id {id} not found.");
                return null!;
            }

            _logger.LogInformation("Fetched car with id {Id} from the database.", id);
            return new CarResponseDto
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                Price = car.Price,
                Mileage = car.Mileage,
                IsUsed = car.IsUsed,
                Description = car.Description,
                UserId = car.UserId,
                UserName = car.User.Name,
                Reviews = car.Reviews.Select(r => new ReviewDto
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    UserId = r.UserId,
                    UserName = r.User.Name
                }).ToList()
            };
        }

        //Create a new car
        public async Task<CarResponseDto> CreateCarAsync(CarCreateRequestDto request, int userId)
        {
            _logger.LogInformation("Creating a new car for user with id {UserId}.", userId);
            var car = new Car
            {
                Brand = request.Brand,
                Model = request.Model,
                Year = request.Year,
                Price = request.Price,
                Mileage = request.Mileage,
                IsUsed = request.IsUsed,
                Description = request.Description,
                UserId = userId
            };

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created a new car with id {CarId} for user with id {UserId}.", car.Id, userId);
            return new CarResponseDto
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                Price = car.Price,
                Mileage = car.Mileage,
                IsUsed = car.IsUsed,
                Description = car.Description,
                UserId = car.UserId,
                UserName = (await _context.Users.FindAsync(userId))? .Name
            };
        }

        //Update an existing car
        public async Task UpdateCarAsync(int carId, UpdateCarRequestDto request, int userId)
        {
            _logger.LogInformation("Updating car with id {CarId} for user with id {UserId}.", carId, userId);

            var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == carId && c.UserId == userId);
            if (car == null)
            {
                _logger.LogWarning("Car with id {CarId} not found or user with id {UserId} does not have permission to update it.", carId, userId);
                throw new KeyNotFoundException($"Car with id {carId} not found or you do not have permission to update it.");
            }

            //Update car props
            car.Brand = request.Brand ?? car.Brand;
            car.Model = request.Model ?? car.Model;
            car.Year = request.Year ?? car.Year;
            car.Price = request.Price ?? car.Price;
            car.Mileage = request.Mileage ?? car.Mileage;
            car.IsUsed = request.IsUsed ?? car.IsUsed;
            car.Description = request.Description ?? car.Description;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated car with id {CarId} for user with id {UserId}.", carId, userId);
        }

        //Delete car
        public async Task DeleteCarAsync(int carId, int userId)
        {
            _logger.LogInformation("Deleting car with id {CarId} for user with id {UserId}.", carId, userId);

            var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == carId && c.UserId == userId);
            if (car == null)
            {
                _logger.LogWarning("Car with id {CarId} not found or user with id {UserId} does not have permission to delete it.", carId, userId);
                throw new KeyNotFoundException($"Car with id {carId} not found or you do not have permission to delete it.");
            }
                
            
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted car with id {CarId} for user with id {UserId}.", carId, userId);
        }

        //Search cars
        public async Task<List<CarResponseDto>> SearchCarsAsync(string? brand, int? minYear, decimal? maxPrice)
        {
            _logger.LogInformation("Searching cars with brand {Brand}, minYear {MinYear}, maxPrice {MaxPrice}.", brand, minYear, maxPrice);
            
            var query = _context.Cars.AsQueryable();

            if(!string.IsNullOrEmpty(brand))
                query = query.Where(c => c.Brand!.Contains(brand));

            if (minYear.HasValue)
                query = query.Where(c => c.Year >= minYear.Value);

            if (maxPrice.HasValue)
                query = query.Where(c => c.Price <= maxPrice.Value);

            var cars = await query
                .Include(c => c.User)
                .Include(c => c.Reviews)
                .Select(c => new CarResponseDto
                {
                    Id = c.Id,
                    Brand = c.Brand,
                    Model = c.Model,
                    Year = c.Year,
                    Price = c.Price,
                    Mileage = c.Mileage,
                    IsUsed = c.IsUsed,
                    Description = c.Description,
                    UserId = c.UserId,
                    UserName = c.User.Name,
                    Reviews = c.Reviews.Select(r => new ReviewDto
                    {
                        Id = r.Id,
                        Rating = r.Rating,
                        Comment = r.Comment,
                        UserId = r.UserId,
                        UserName = r.User.Name
                    }).ToList()
                }).ToListAsync();

            _logger.LogInformation("Found {Count} cars matching the search criteria.", cars.Count);
            return cars;
        }
    }
}
