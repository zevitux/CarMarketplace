using CarMarketplace.DTOs;

namespace CarMarketplace.Services
{
    public interface ICarService
    {
        List<CarResponseDto> GetAllCars();
        CarResponseDto GetCarById(int id);
        CarResponseDto CreateCar(CarCreateRequestDto request, int userId);
        void UpdateCar(int carId, UpdateCarRequestDto request, int userId);
        void DeleteCar(int carId, int userId);
        List<CarResponseDto> SearchCars(string? brand, int? minYear, decimal? maxPrice);
    }
}
