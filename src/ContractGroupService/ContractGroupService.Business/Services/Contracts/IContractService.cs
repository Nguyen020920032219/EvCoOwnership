using ContractGroupService.Business.Models;

namespace ContractGroupService.Business.Services.Contracts;

public interface IContractService
{
    // Tạo hợp đồng mới cho nhóm (thường do Admin nhóm làm)
    Task<ContractDetailDto> GenerateContractAsync(int userId, GenerateContractRequest request);
    
    // Lấy thông tin hợp đồng
    Task<ContractDetailDto> GetContractByGroupAsync(int userId, int groupId);
    
    // Ký hợp đồng
    Task SignContractAsync(int userId, int contractId);
}