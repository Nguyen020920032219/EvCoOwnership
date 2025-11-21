using ContractGroupService.Business.Models;
using ContractGroupService.Data.Entities;
using ContractGroupService.Data.Repositories.Disputes;
using ContractGroupService.Data.Repositories.Groups;

namespace ContractGroupService.Business.Services.Disputes;

public class DisputeService : IDisputeService
{
    private readonly IDisputeRepository _disputeRepo;
    private readonly IGroupRepository _groupRepo;

    public DisputeService(IDisputeRepository disputeRepo, IGroupRepository groupRepo)
    {
        _disputeRepo = disputeRepo;
        _groupRepo = groupRepo;
    }

    public async Task<DisputeDetailDto> CreateDisputeAsync(int userId, CreateDisputeRequest request)
    {
        // Validate member
        var groups = await _groupRepo.GetGroupsByUserIdAsync(userId);
        if (!groups.Any(g => g.CoOwnerGroupId == request.CoOwnerGroupId))
            throw new Exception("Bạn không phải thành viên của nhóm này.");

        var dispute = new GroupDispute
        {
            CoOwnershipGroupId = request.CoOwnerGroupId,
            RaisedByUserId = userId,
            Title = request.Title,
            Description = request.Description,
            RelatedBookingId = request.RelatedBookingId,
            Status = 0, // Open
            CreatedAt = DateTime.UtcNow
        };

        await _disputeRepo.Add(dispute);

        return MapToDto(dispute);
    }

    public async Task<DisputeDetailDto> GetDisputeDetailAsync(int userId, int disputeId)
    {
        var dispute = await _disputeRepo.GetDetailAsync(disputeId);
        if (dispute == null) throw new Exception("Không tìm thấy tranh chấp.");

        // Check quyền xem (là thành viên nhóm hoặc là Staff - ở đây tạm check thành viên)
        var groups = await _groupRepo.GetGroupsByUserIdAsync(userId);
        if (!groups.Any(g => g.CoOwnerGroupId == dispute.CoOwnershipGroupId))
            throw new Exception("Không có quyền truy cập.");

        return MapToDto(dispute);
    }

    public async Task AddMessageAsync(int userId, int disputeId, string message)
    {
        var dispute = await _disputeRepo.GetByIdAsync(disputeId);
        if (dispute == null) throw new Exception("Không tìm thấy tranh chấp.");
        if (dispute.Status == 2 || dispute.Status == 3) throw new Exception("Tranh chấp đã đóng.");

        var msgEntity = new GroupDisputeMessage
        {
            GroupDisputeId = disputeId,
            SenderUserId = userId,
            Message = message,
            CreatedAt = DateTime.UtcNow
        };

        await _disputeRepo.AddMessageAsync(msgEntity);
    }

    public async Task ResolveDisputeAsync(int staffId, int disputeId, string resolutionNote)
    {
        var dispute = await _disputeRepo.GetByIdAsync(disputeId);
        if (dispute == null) throw new Exception("Không tìm thấy tranh chấp.");

        if (dispute.Status == 2 || dispute.Status == 3)
            throw new Exception("Tranh chấp này đã được xử lý trước đó.");

        dispute.Status = 2;
        dispute.ResolvedAt = DateTime.UtcNow;
        dispute.ResolvedByStaffId = staffId;

        await _disputeRepo.AddMessageAsync(new GroupDisputeMessage
        {
            GroupDisputeId = disputeId,
            SenderUserId = staffId,
            Message = $"[HỆ THỐNG] Đã giải quyết: {resolutionNote}",
            CreatedAt = DateTime.UtcNow
        });

        await _disputeRepo.Update(dispute);
    }

    private static DisputeDetailDto MapToDto(GroupDispute d)
    {
        return new DisputeDetailDto
        {
            DisputeId = d.GroupDisputeId,
            Title = d.Title,
            Status = d.Status switch { 0 => "Open", 1 => "Processing", 2 => "Resolved", _ => "Closed" },
            CreatedAt = d.CreatedAt,
            RaisedByUserId = d.RaisedByUserId,
            Messages = d.GroupDisputeMessages.Select(m => new DisputeMessageDto
            {
                SenderUserId = m.SenderUserId,
                Content = m.Message,
                SentAt = m.CreatedAt
            }).ToList()
        };
    }
    
    public async Task<List<DisputeDetailDto>> GetAllDisputesAsync()
    {
        var disputes = await _disputeRepo.GetAllAsync();
        
        return disputes.Select(MapToDto).ToList();
    }
}