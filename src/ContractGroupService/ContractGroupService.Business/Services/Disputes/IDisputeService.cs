using ContractGroupService.Business.Models;

namespace ContractGroupService.Business.Services.Disputes;

public interface IDisputeService
{
    Task<DisputeDetailDto> CreateDisputeAsync(int userId, CreateDisputeRequest request);
    Task<DisputeDetailDto> GetDisputeDetailAsync(int userId, int disputeId);
    Task AddMessageAsync(int userId, int disputeId, string message);
    Task ResolveDisputeAsync(int staffId, int disputeId, string resolutionNote);
    Task<List<DisputeDetailDto>> GetAllDisputesAsync();
}