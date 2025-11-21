using ContractGroupService.Business.Models;
using ContractGroupService.Data.Entities;
using ContractGroupService.Data.Repositories.Groups;
using ContractGroupService.Data.Repositories.Votes;
using EvCoOwnership.Shared.Models; // Để dùng SystemRoles

namespace ContractGroupService.Business.Services.Votes;

public class VoteService : IVoteService
{
    private readonly IGroupRepository _groupRepo;
    private readonly IVoteRepository _voteRepo;

    public VoteService(IVoteRepository voteRepo, IGroupRepository groupRepo)
    {
        _voteRepo = voteRepo;
        _groupRepo = groupRepo;
    }

    // --- Helper Check Quyền (ĐÃ CẬP NHẬT) ---
    private async Task ValidateGroupMember(int groupId, int userId, string? userRole)
    {
        // Nếu là Admin hoặc Operator thì cho qua luôn (Bypass check member)
        if (userRole == SystemRoles.Admin || userRole == SystemRoles.Operator) 
        {
            return;
        }

        // Logic cũ: User thường thì phải check
        var groups = await _groupRepo.GetGroupsByUserIdAsync(userId);
        if (!groups.Any(g => g.CoOwnerGroupId == groupId))
            throw new Exception("Bạn không phải là thành viên của nhóm này.");
    }

    // --- Các hàm Public (Đã thêm tham số userRole) ---

    public async Task<VoteDetailDto> CreateVoteAsync(int userId, string? userRole, CreateVoteRequest request)
    {
        await ValidateGroupMember(request.CoOwnerGroupId, userId, userRole);

        var vote = new GroupVote
        {
            CoOwnerGroupId = request.CoOwnerGroupId,
            UserId = userId, // Người tạo vote
            Topic = request.Topic,
            Description = request.Description,
            Status = 0, // Open
            CreatedAt = DateTime.UtcNow
        };

        await _voteRepo.Add(vote);
        // Lưu ý: Cần SaveChanges nếu BaseRepo chưa có
        // await _voteRepo.DbContext().SaveChangesAsync();

        return MapToDto(vote);
    }

    public async Task CastVoteAsync(int userId, string? userRole, int voteId, CastVoteRequest request)
    {
        var vote = await _voteRepo.GetVoteDetailAsync(voteId);
        if (vote == null) throw new Exception("Không tìm thấy cuộc bỏ phiếu.");

        // Check quyền
        await ValidateGroupMember(vote.CoOwnerGroupId, userId, userRole);

        if (vote.Status != 0) throw new Exception("Cuộc bỏ phiếu đã kết thúc.");

        var hasVoted = await _voteRepo.HasUserVotedAsync(voteId, userId);
        if (hasVoted) throw new Exception("Bạn đã bỏ phiếu rồi.");

        var choice = new VoteChoice
        {
            VoteId = voteId,
            UserId = userId,
            Choice = request.Agree,
            VotedAt = DateTime.UtcNow
        };

        await _voteRepo.AddVoteChoiceAsync(choice);
        await _voteRepo.DbContext().SaveChangesAsync();
    }

    public async Task<List<VoteDetailDto>> GetVotesByGroupAsync(int userId, string? userRole, int groupId)
    {
        await ValidateGroupMember(groupId, userId, userRole);
        
        var votes = await _voteRepo.GetVotesByGroupIdAsync(groupId);
        return votes.Select(MapToDto).ToList();
    }

    public async Task<VoteDetailDto> GetVoteResultAsync(int userId, string? userRole, int voteId)
    {
        var vote = await _voteRepo.GetVoteDetailAsync(voteId);
        if (vote == null) throw new Exception("Không tìm thấy cuộc bỏ phiếu.");

        await ValidateGroupMember(vote.CoOwnerGroupId, userId, userRole);

        return MapToDto(vote);
    }

    private static VoteDetailDto MapToDto(GroupVote v)
    {
        return new VoteDetailDto
        {
            VoteId = v.VoteId,
            Topic = v.Topic,
            Description = v.Description,
            Status = v.Status == 0 ? "Open" : "Closed",
            CreatedAt = v.CreatedAt,
            TotalYes = v.VoteChoices.Count(c => c.Choice),
            TotalNo = v.VoteChoices.Count(c => !c.Choice),
            Choices = v.VoteChoices.Select(c => new VoteChoiceDto
            {
                UserId = c.UserId,
                Choice = c.Choice ? "Yes" : "No",
                VotedAt = c.VotedAt
            }).ToList()
        };
    }
}