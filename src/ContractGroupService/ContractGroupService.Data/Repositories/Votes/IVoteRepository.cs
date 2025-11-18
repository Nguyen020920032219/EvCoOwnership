using ContractGroupService.Data.Entities;
using EvCoOwnership.Shared.BaseRepository;

namespace ContractGroupService.Data.Repositories.Votes;

public interface IVoteRepository : IBaseRepository<GroupVote, int>
{
    // Lấy danh sách cuộc bỏ phiếu trong nhóm
    Task<IReadOnlyList<GroupVote>> GetVotesByGroupIdAsync(int groupId);

    // Lấy chi tiết cuộc bỏ phiếu kèm các lựa chọn đã vote
    Task<GroupVote?> GetVoteDetailAsync(int voteId);

    // Kiểm tra xem user đã bỏ phiếu cho cuộc này chưa
    Task<bool> HasUserVotedAsync(int voteId, int userId);

    // Lưu lựa chọn của user
    Task AddVoteChoiceAsync(VoteChoice choice);
}