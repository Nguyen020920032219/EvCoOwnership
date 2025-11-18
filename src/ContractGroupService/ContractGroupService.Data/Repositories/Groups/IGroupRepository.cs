using ContractGroupService.Data.Entities;
using EvCoOwnership.Shared.BaseRepository;

namespace ContractGroupService.Data.Repositories.Groups;

public interface IGroupRepository : IBaseRepository<CoOwnershipGroup, int>
{
    // Lấy nhóm theo User
    Task<IReadOnlyList<CoOwnershipGroup>> GetGroupsByUserIdAsync(int userId);

    // Lấy chi tiết nhóm (kèm Members và Shares)
    Task<CoOwnershipGroup?> GetGroupDetailAsync(int groupId);

    // Check xem User có phải là Admin của nhóm không
    Task<bool> IsGroupAdminAsync(int groupId, int userId);
}