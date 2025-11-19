using ContractGroupService.Business.Models;
using ContractGroupService.Data.Entities;
using ContractGroupService.Data.Repositories.Groups;
using ContractGroupService.Data.Repositories.Votes;

// Cần check quyền thành viên

namespace ContractGroupService.Business.Services.Votes;

public class VoteService : IVoteService
{
    private readonly IGroupRepository _groupRepo; // Để check user có thuộc nhóm không
    private readonly IVoteRepository _voteRepo;

    public VoteService(IVoteRepository voteRepo, IGroupRepository groupRepo)
    {
        _voteRepo = voteRepo;
        _groupRepo = groupRepo;
    }

    public async Task<VoteDetailDto> CreateVoteAsync(int userId, CreateVoteRequest request)
    {
        await ValidateGroupMember(request.CoOwnerGroupId, userId);

        var vote = new GroupVote
        {
            CoOwnerGroupId = request.CoOwnerGroupId,
            UserId = userId, // Người tạo vote
            Topic = request.Topic,
            Description = request.Description,
            Status = 0, // 0 = Open
            CreatedAt = DateTime.UtcNow
        };

        await _voteRepo.Add(vote);

        return MapToDto(vote);
    }

    public async Task CastVoteAsync(int userId, int voteId, CastVoteRequest request)
    {
        var vote = await _voteRepo.GetVoteDetailAsync(voteId);
        if (vote == null) throw new Exception("Không tìm thấy cuộc bỏ phiếu.");

        // 1. Check quyền thành viên
        await ValidateGroupMember(vote.CoOwnerGroupId, userId);

        // 2. Check trạng thái vote
        if (vote.Status != 0) throw new Exception("Cuộc bỏ phiếu đã kết thúc.");

        // 3. Check đã vote chưa
        var hasVoted = await _voteRepo.HasUserVotedAsync(voteId, userId);
        if (hasVoted) throw new Exception("Bạn đã bỏ phiếu rồi, không thể vote lại.");

        // 4. Lưu vote
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

    public async Task<List<VoteDetailDto>> GetVotesByGroupAsync(int userId, int groupId)
    {
        await ValidateGroupMember(groupId, userId);
        var votes = await _voteRepo.GetVotesByGroupIdAsync(groupId);

        // Lưu ý: Để tối ưu, GetVotesByGroupIdAsync nên Include VoteChoices nếu muốn show kết quả ngay
        // Ở đây map đơn giản
        return votes.Select(MapToDto).ToList();
    }

    public async Task<VoteDetailDto> GetVoteResultAsync(int userId, int voteId)
    {
        var vote = await _voteRepo.GetVoteDetailAsync(voteId);
        if (vote == null) throw new Exception("Không tìm thấy cuộc bỏ phiếu.");

        await ValidateGroupMember(vote.CoOwnerGroupId, userId);

        return MapToDto(vote);
    }

    // Helper check thành viên nhóm
    private async Task ValidateGroupMember(int groupId, int userId)
    {
        var groups = await _groupRepo.GetGroupsByUserIdAsync(userId);
        if (!groups.Any(g => g.CoOwnerGroupId == groupId))
            throw new Exception("Bạn không phải là thành viên của nhóm này.");
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
    
    public async Task<VoteDetailDto> CreateVoteAsync(int userId, string userRole, CreateVoteRequest request)
    {
        // Logic mới:
        // Nếu Role là Admin hoặc Staff (Operator) -> Cho phép luôn (Bỏ qua check member)
        // Nếu là CoOwner (User thường) -> Phải check xem có trong nhóm không
        
        bool isSystemAdmin = userRole == "Admin" || userRole == "Operator"; // String khớp với SystemRoles.cs

        if (!isSystemAdmin)
        {
            // User thường thì phải check
            await ValidateGroupMember(request.CoOwnerGroupId, userId);
        }

        var vote = new GroupVote
        {
            CoOwnerGroupId = request.CoOwnerGroupId,
            UserId = userId,
            Topic = request.Topic,
            Description = request.Description,
            Status = 0, // Open
            CreatedAt = DateTime.UtcNow
        };

        await _voteRepo.Add(vote);

        return MapToDto(vote);
    }
}