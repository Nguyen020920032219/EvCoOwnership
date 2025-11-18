using ContractGroupService.Business.Models;

namespace ContractGroupService.Business.Services.Groups;

public interface IGroupService
{
    Task<GroupDetailDto> CreateGroupAsync(int creatorUserId, CreateGroupRequest request);
    Task<List<CoOwnerGroupDto>> GetMyGroupsAsync(int userId);
    Task<GroupDetailDto> GetGroupDetailAsync(int groupId, int userId); // Cần userId để check quyền xem
}