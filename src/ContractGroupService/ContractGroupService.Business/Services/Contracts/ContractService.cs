using ContractGroupService.Business.Models;
using ContractGroupService.Data.Entities;
using ContractGroupService.Data.Repositories.Contracts;
using ContractGroupService.Data.Repositories.Groups;
using EvCoOwnership.Shared.Models; // Cần namespace này để dùng SystemRoles

namespace ContractGroupService.Business.Services.Contracts;

public class ContractService : IContractService
{
    private readonly IContractRepository _contractRepo;
    private readonly IGroupRepository _groupRepo;

    public ContractService(IContractRepository contractRepo, IGroupRepository groupRepo)
    {
        _contractRepo = contractRepo;
        _groupRepo = groupRepo;
    }

    public async Task<ContractDetailDto> GenerateContractAsync(int userId, string? userRole, GenerateContractRequest request)
    {
        // 1. Check quyền: Admin hệ thống/Operator HOẶC Admin nhóm
        bool isSystemAdmin = userRole == SystemRoles.Admin || userRole == SystemRoles.Operator;

        if (!isSystemAdmin)
        {
            // Nếu không phải Admin hệ thống, bắt buộc phải là Admin nhóm
            var isGroupAdmin = await _groupRepo.IsGroupAdminAsync(request.CoOwnerGroupId, userId);
            if (!isGroupAdmin) 
                throw new Exception("Chỉ Admin nhóm hoặc Quản trị viên hệ thống mới được tạo/sửa hợp đồng.");
        }

        // 2. Lấy hợp đồng hiện tại
        var contract = await _contractRepo.GetByGroupIdAsync(request.CoOwnerGroupId);
        
        if (contract == null)
        {
            contract = new OwnershipContract
            {
                CreatedAt = DateTime.UtcNow
            };
            await _contractRepo.Add(contract);
            
            var group = await _groupRepo.GetByIdAsync(request.CoOwnerGroupId);
            if (group != null) {
                group.ContractId = contract.ContractId;
                await _groupRepo.Update(group);
            }
        }

        // 3. Cập nhật nội dung
        contract.Content = request.Content;
        contract.UpdatedAt = DateTime.UtcNow;
        
        await _contractRepo.Update(contract);

        return new ContractDetailDto
        {
            ContractId = contract.ContractId,
            CoOwnerGroupId = request.CoOwnerGroupId,
            Content = contract.Content,
            CreatedAt = contract.CreatedAt
        };
    }

    public async Task<ContractDetailDto> GetContractByGroupAsync(int userId, string? userRole, int groupId)
    {
        // 1. Check quyền xem: Admin hệ thống/Operator HOẶC Thành viên nhóm
        bool isSystemAdmin = userRole == SystemRoles.Admin || userRole == SystemRoles.Operator;

        if (!isSystemAdmin)
        {
            var groups = await _groupRepo.GetGroupsByUserIdAsync(userId);
            if (!groups.Any(g => g.CoOwnerGroupId == groupId))
                throw new Exception("Bạn không phải thành viên nhóm, không thể xem hợp đồng.");
        }

        var contract = await _contractRepo.GetByGroupIdAsync(groupId);
        if (contract == null) throw new Exception("Nhóm chưa có hợp đồng.");

        var groupDetail = await _groupRepo.GetGroupDetailAsync(groupId);
        var signatures = new List<SignatureStatusDto>();
        
        if (groupDetail != null)
            foreach (var member in groupDetail.CoOwnershipMembers)
            {
                var sig = contract.ContractSignatures.FirstOrDefault(s => s.UserId == member.UserId);
                signatures.Add(new SignatureStatusDto
                {
                    UserId = member.UserId,
                    HasSigned = sig != null && sig.Status == 1,
                    SignedAt = sig?.SignedAt
                });
            }

        return new ContractDetailDto
        {
            ContractId = contract.ContractId,
            CoOwnerGroupId = groupId,
            Content = contract.Content,
            CreatedAt = contract.CreatedAt,
            Signatures = signatures
        };
    }

    public async Task SignContractAsync(int userId, int contractId)
    {
        // Hàm này giữ nguyên, vì chỉ thành viên sở hữu cổ phần mới cần ký.
        // Admin hệ thống không nên ký thay.
        var signed = await _contractRepo.HasUserSignedAsync(contractId, userId);
        if (signed) throw new Exception("Bạn đã ký hợp đồng này rồi.");

        var signature = new ContractSignature
        {
            ContractId = contractId,
            UserId = userId,
            SignedAt = DateTime.UtcNow,
            Status = 1
        };

        await _contractRepo.AddSignatureAsync(signature);
        await _contractRepo.DbContext().SaveChangesAsync();
    }
}