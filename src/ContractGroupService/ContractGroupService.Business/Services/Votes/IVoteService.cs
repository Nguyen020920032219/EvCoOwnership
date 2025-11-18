using ContractGroupService.Business.Models;

namespace ContractGroupService.Business.Services.Votes;

public interface IVoteService
{
    Task<VoteDetailDto> CreateVoteAsync(int userId, CreateVoteRequest request);
    Task CastVoteAsync(int userId, int voteId, CastVoteRequest request);
    Task<List<VoteDetailDto>> GetVotesByGroupAsync(int userId, int groupId);
    Task<VoteDetailDto> GetVoteResultAsync(int userId, int voteId);
}