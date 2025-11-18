using Microsoft.EntityFrameworkCore;
using VehicleService.Business.Models;
using VehicleService.Business.Services;
using VehicleService.Data.Configurations;

namespace VehicleService.Business;

public class VehicleService : IVehicleService
{
    private readonly VehicleDbContext _dbContext;

    public VehicleService(VehicleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<VehicleDto>> GetVehiclesByGroupAsync(int coOwnerGroupId)
    {
        var query = _dbContext.Vehicles
            .Where(v => v.CoOwnerGroupId == coOwnerGroupId)
            .Select(v => new VehicleDto
            {
                VehicleId     = v.VehicleId,
                LicensePlate  = v.LicensePlate,
                Brand         = v.Make,
                Model         = v.Model,
                Color         = v.Color
            });

        return await query.ToListAsync();
    }
}