using ContractGroupService.Data.Entities;
using EvCoOwnership.Shared.BaseRepository;

namespace ContractGroupService.Data.Repositories.Disputes;

public interface IDisputeRepository : IBaseRepository<GroupDispute, int>
{
    // Lấy danh sách tranh chấp trong nhóm
    Task<IReadOnlyList<GroupDispute>> GetByGroupIdAsync(int groupId);

    // Lấy chi tiết kèm tin nhắn hội thoại
    Task<GroupDispute?> GetDetailAsync(int disputeId);

    // Thêm tin nhắn vào cuộc tranh chấp
    Task AddMessageAsync(GroupDisputeMessage message);
    
    // Lấy tất cả tranh chấp
    Task<IReadOnlyList<GroupDispute>> GetAllAsync();
}