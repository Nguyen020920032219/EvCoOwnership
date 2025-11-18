using ContractGroupService.Data.Entities;
using EvCoOwnership.Shared.BaseRepository;

namespace ContractGroupService.Data.Repositories.Contracts;

public interface IContractRepository : IBaseRepository<OwnershipContract, int>
{
    // Lấy hợp đồng của một nhóm (kèm chữ ký)
    Task<OwnershipContract?> GetByGroupIdAsync(int groupId);
    
    // Kiểm tra xem user đã ký hợp đồng này chưa
    Task<bool> HasUserSignedAsync(int contractId, int userId);

    // Thêm chữ ký
    Task AddSignatureAsync(ContractSignature signature);
}