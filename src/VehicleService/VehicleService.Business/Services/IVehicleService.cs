using VehicleService.Business.Models;

namespace VehicleService.Business.Services;

public interface IVehicleService
{
    Task<IReadOnlyList<VehicleDto>> GetVehiclesByGroupAsync(int coOwnerGroupId);
}