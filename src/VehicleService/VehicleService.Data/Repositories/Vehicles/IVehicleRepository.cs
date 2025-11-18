using EvCoOwnership.Shared.BaseRepository;
using VehicleService.Data.Entities;

namespace VehicleService.Data.Repositories.Vehicles;

public interface IVehicleRepository : IBaseRepository<Vehicle, int>
{
    Task<bool> IsLicensePlateExistsAsync(string plate);

    Task<bool> IsVinExistsAsync(string vin);

    Task<IReadOnlyList<Vehicle>> GetByGroupIdAsync(int groupId);
}