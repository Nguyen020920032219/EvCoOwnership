using VehicleService.Business.Models;

namespace VehicleService.Business.Services.Vehicles;

public interface IVehicleService
{
    Task<IReadOnlyList<VehicleDto>> GetVehiclesByGroupAsync(int coOwnerGroupId);
    Task<VehicleDto> CreateVehicleAsync(CreateVehicleRequest request);
    Task<List<VehicleDto>> GetVehicles();
    Task<VehicleDto> GetById(int id);
}