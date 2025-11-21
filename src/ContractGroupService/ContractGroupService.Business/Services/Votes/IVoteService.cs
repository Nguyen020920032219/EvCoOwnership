using ContractGroupService.Business.Models;

namespace ContractGroupService.Business.Services.Votes;

public interface IVoteService
{
    // Tạo cuộc bỏ phiếu (Có check Role để bypass validation nếu là Admin)
    Task<VoteDetailDto> CreateVoteAsync(int userId, string? userRole, CreateVoteRequest request);

    // Bỏ phiếu (Có check Role)
    Task CastVoteAsync(int userId, string? userRole, int voteId, CastVoteRequest request);

    // Lấy danh sách cuộc bỏ phiếu theo nhóm (Có check Role)
    Task<List<VoteDetailDto>> GetVotesByGroupAsync(int userId, string? userRole, int groupId);

    // Xem kết quả chi tiết (Có check Role)
    Task<VoteDetailDto> GetVoteResultAsync(int userId, string? userRole, int voteId);
}