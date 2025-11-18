using EvCoOwnership.Shared.BaseRepository;
using Microsoft.EntityFrameworkCore;
using VehicleService.Data.Configurations;
using VehicleService.Data.Entities;

namespace VehicleService.Data.Repositories.Vehicles;

public class VehicleRepository : BaseRepository<VehicleDbContext, Vehicle, int>, IVehicleRepository
{
    public VehicleRepository(VehicleDbContext context) : base(context)
    {
    }

    public async Task<bool> IsLicensePlateExistsAsync(string plate)
    {
        return await DbSet().AnyAsync(v => v.LicensePlate == plate);
    }

    public async Task<bool> IsVinExistsAsync(string vin)
    {
        return await DbSet().AnyAsync(v => v.Vin == vin);
    }

    public async Task<IReadOnlyList<Vehicle>> GetByGroupIdAsync(int groupId)
    {
        return await Where(v => v.CoOwnerGroupId == groupId).ToListAsync();
    }
}