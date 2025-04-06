using CarMarketplace.DTOs;

namespace CarMarketplace.Services
{
    public interface ICarService
    {
        Task<List<CarResponseDto>> GetAllCarsAsync();
        Task<CarResponseDto> GetCarByIdAsync(int id);
        Task<CarResponseDto> CreateCarAsync(CarCreateRequestDto request, int userId);
        Task UpdateCarAsync(int carId, UpdateCarRequestDto request, int userId);
        Task DeleteCarAsync(int carId, int userId);
        Task<List<CarResponseDto>> SearchCarsAsync(string? brand, int? minYear, decimal? maxPrice);
    }
}