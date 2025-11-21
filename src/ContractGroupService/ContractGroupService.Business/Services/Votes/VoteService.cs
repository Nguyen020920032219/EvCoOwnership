using System.Text.RegularExpressions;
using ContractGroupService.Business.Models;
using ContractGroupService.Data.Entities;
using ContractGroupService.Data.Repositories.Groups;
using ContractGroupService.Data.Repositories.Votes;
using EvCoOwnership.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace ContractGroupService.Business.Services.Votes;

public class VoteService : IVoteService
{
    private readonly IGroupRepository _groupRepo;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IVoteRepository _voteRepo;

    public VoteService(IVoteRepository voteRepo, IGroupRepository groupRepo, IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor)
    {
        _voteRepo = voteRepo;
        _groupRepo = groupRepo;
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
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

        // 1. Logic Vote (Giữ nguyên)
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
        // Lưu vote trước
        await _voteRepo.DbContext().SaveChangesAsync();

        // 2. LOGIC TỰ ĐỘNG KIỂM TRA KẾT QUẢ (NEW)
        await CheckAndExecuteAutoDecision(voteId, vote.CoOwnerGroupId, userId);
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

    // --- Helper Check Quyền (ĐÃ CẬP NHẬT) ---
    private async Task ValidateGroupMember(int groupId, int userId, string? userRole)
    {
        // Nếu là Admin hoặc Operator thì cho qua luôn (Bypass check member)
        if (userRole == SystemRoles.Admin || userRole == SystemRoles.Operator) return;

        // Logic cũ: User thường thì phải check
        var groups = await _groupRepo.GetGroupsByUserIdAsync(userId);
        if (!groups.Any(g => g.CoOwnerGroupId == groupId))
            throw new Exception("Bạn không phải là thành viên của nhóm này.");
    }

    // Hàm xử lý tự động
    private async Task CheckAndExecuteAutoDecision(int voteId, int groupId, int triggerUserId)
    {
        // A. Lấy thông tin mới nhất
        var vote = await _voteRepo.GetVoteDetailAsync(voteId);
        var group = await _groupRepo.GetGroupDetailAsync(groupId);
        if (vote == null || group == null) return;

        var totalMembers = group.CoOwnershipMembers.Count;
        var totalVotes = vote.VoteChoices.Count;

        // B. Kiểm tra xem tất cả mọi người đã vote chưa?
        if (totalVotes >= totalMembers)
        {
            // Đóng Vote
            vote.Status = 1; // Closed
            await _voteRepo.Update(vote); // Lưu trạng thái đóng

            // C. Tính kết quả
            var yesVotes = vote.VoteChoices.Count(c => c.Choice);
            var noVotes = vote.VoteChoices.Count(c => !c.Choice);

            // Nếu số phiếu Đồng ý > 50% (hoặc logic khác tùy bạn)
            if (yesVotes > noVotes)
            {
                // D. Kiểm tra xem đây có phải là Vote cho Hóa đơn không?
                // Quy ước Description: "[AUTO_INVOICE:123]..."
                var match = Regex.Match(vote.Description ?? "", @"\[AUTO_INVOICE:(\d+)\]");
                if (match.Success)
                {
                    // Parse int chắc chắn thành công vì Regex (\d+) chỉ bắt số
                    var invoiceId = int.Parse(match.Groups[1].Value);

                    // E. GỌI FINANCE SERVICE ĐỂ TRỪ TIỀN
                    await CallFinanceServiceToPay(invoiceId, triggerUserId); // Dùng ID người kích hoạt để log
                }
            }
        }
    }

    private async Task CallFinanceServiceToPay(int invoiceId, int userId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5020"); // Port của FinanceService

            // Lấy token từ context hiện tại để chuyển tiếp (Forward) sang FinanceService
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(token))
                // Bỏ "Bearer " nếu có để add vào header cho chuẩn (hoặc add nguyên chuỗi tùy client config)
                // Thông thường DefaultRequestHeaders.Authorization nhận AuthenticationHeaderValue
                // Ở đây add trực tiếp key "Authorization" để đơn giản hóa
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);

            // Gọi API: POST /api/maintenance/{id}/pay
            var response = await client.PostAsync($"api/maintenance/{invoiceId}/pay", null);

            if (!response.IsSuccessStatusCode)
                // Log lỗi nếu cần (nhưng không throw exception để tránh rollback giao dịch Vote)
                Console.WriteLine($"Lỗi tự động thanh toán Invoice {invoiceId}: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi gọi Finance Service: {ex.Message}");
        }
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