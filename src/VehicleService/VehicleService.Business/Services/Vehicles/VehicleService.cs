using VehicleService.Business.Models;
using VehicleService.Data.Entities;
using VehicleService.Data.Repositories.Vehicles;

namespace VehicleService.Business.Services.Vehicles;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepo;

    public VehicleService(IVehicleRepository vehicleRepo)
    {
        _vehicleRepo = vehicleRepo;
    }

    public async Task<IReadOnlyList<VehicleDto>> GetVehiclesByGroupAsync(int coOwnerGroupId)
    {
        var vehicles = await _vehicleRepo.GetByGroupIdAsync(coOwnerGroupId);

        // Sử dụng Helper function để map data
        return vehicles.Select(MapToDto).ToList();
    }

    public async Task<VehicleDto> CreateVehicleAsync(CreateVehicleRequest request)
    {
        // 1. Validate Business Rules: Check trùng biển số & VIN
        if (await _vehicleRepo.IsLicensePlateExistsAsync(request.LicensePlate))
            throw new Exception($"Biển số {request.LicensePlate} đã tồn tại trong hệ thống.");

        if (await _vehicleRepo.IsVinExistsAsync(request.Vin))
            throw new Exception($"Số khung (VIN) {request.Vin} đã tồn tại trong hệ thống.");

        // 2. Map Request to Entity
        var entity = new Vehicle
        {
            LicensePlate = request.LicensePlate,
            Vin = request.Vin,
            Make = request.Make,
            Model = request.Model,
            Year = request.Year,
            Color = request.Color,
            BatteryCapacityKwh = request.BatteryCapacityKwh,
            ChargingPortType = request.ChargingPortType,
            PurchasePrice = request.PurchasePrice,
            CoOwnerGroupId = request.CoOwnerGroupId,
            PurchaseDate = DateOnly.FromDateTime(DateTime.Now), // Mặc định ngày mua là hôm nay (demo)
            Status = 1 // Default: 1 = Active
        };

        // 3. Save to DB
        await _vehicleRepo.Add(entity);
        // BaseRepository của bạn có thể cần gọi SaveChanges thủ công hoặc đã tích hợp sẵn.
        // Ở đây tôi giả định là gọi phương thức Add xong cần SaveChanges (nếu Add chưa save)
        // await _vehicleRepo.SaveChangesAsync(); 

        return MapToDto(entity);
    }

    // Helper mapping
    private static VehicleDto MapToDto(Vehicle v)
    {
        return new VehicleDto
        {
            VehicleId = v.VehicleId,
            LicensePlate = v.LicensePlate,
            Vin = v.Vin,
            Make = v.Make ?? "N/A",
            Model = v.Model ?? "N/A",
            Color = v.Color ?? "N/A",
            BatteryCapacityKwh = v.BatteryCapacityKwh,
            CoOwnerGroupId = v.CoOwnerGroupId ?? 0,
            Status = v.Status switch
            {
                0 => "Inactive",
                1 => "Active",
                2 => "Maintenance",
                _ => "Unknown"
            }
        };
    }
}