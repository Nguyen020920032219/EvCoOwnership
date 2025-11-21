using ContractGroupService.Business.Models;

namespace ContractGroupService.Business.Services.Contracts;

public interface IContractService
{
    // Thêm userRole vào
    Task<ContractDetailDto> GenerateContractAsync(int userId, string? userRole, GenerateContractRequest request);

    // Thêm userRole vào
    Task<ContractDetailDto> GetContractByGroupAsync(int userId, string? userRole, int groupId);

    // Hàm ký giữ nguyên (Hành động cá nhân)
    Task SignContractAsync(int userId, int contractId);
}