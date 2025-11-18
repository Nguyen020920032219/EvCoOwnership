using ContractGroupService.Business.Models;
using ContractGroupService.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ContractGroupService.Business.Services.Group;

public class GroupService : IGroupService
{
    private readonly ContractGroupDbContext _dbContext;

    public GroupService(ContractGroupDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<CoOwnerGroupDto>> GetGroupsByUserAsync(int userId)
    {
        // join CoOwnershipMember + CoOwnershipGroup để lấy các nhóm mà user tham gia
        var query =
            from m in _dbContext.CoOwnershipMembers
            join g in _dbContext.CoOwnershipGroups
                on m.CoOwnerGroupId equals g.CoOwnerGroupId
            where m.UserId == userId
            select new CoOwnerGroupDto
            {
                CoOwnerGroupId = g.CoOwnerGroupId,
                GroupName      = g.GroupName,
                ContractId     = g.ContractId,
                CreatedAt      = g.CreatedAt,
                IsGroupAdmin   = m.IsGroupAdmin
            };

        var list = await query.ToListAsync();
        return list;
    }
}