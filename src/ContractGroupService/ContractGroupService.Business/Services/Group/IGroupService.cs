using ContractGroupService.Business.Models;

namespace ContractGroupService.Business.Services.Group;

public interface IGroupService
{
    Task<IReadOnlyList<CoOwnerGroupDto>> GetGroupsByUserAsync(int userId);

}