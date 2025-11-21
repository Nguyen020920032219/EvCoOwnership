using ContractGroupService.Business.Models;
using ContractGroupService.Data.Entities;
using ContractGroupService.Data.Repositories.Contracts;
using ContractGroupService.Data.Repositories.Groups;

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

    public async Task<ContractDetailDto> GenerateContractAsync(int userId, GenerateContractRequest request)
    {
        // 1. Check quyền Admin nhóm
        var isAdmin = await _groupRepo.IsGroupAdminAsync(request.CoOwnerGroupId, userId);
        if (!isAdmin) throw new Exception("Chỉ Admin nhóm mới được tạo hợp đồng.");

        // 2. Lấy hợp đồng hiện tại (Chắc chắn có vì đã tạo nháp lúc tạo nhóm)
        var contract = await _contractRepo.GetByGroupIdAsync(request.CoOwnerGroupId);

        if (contract == null)
        {
            // Trường hợp hy hữu nếu data cũ chưa có, ta tạo mới (Fallback)
            contract = new OwnershipContract
            {
                CreatedAt = DateTime.UtcNow
            };
            await _contractRepo.Add(contract);

            // Update lại group
            var group = await _groupRepo.GetByIdAsync(request.CoOwnerGroupId);
            if (group != null)
            {
                group.ContractId = contract.ContractId;
                await _groupRepo.Update(group);
            }
        }

        // 3. Cập nhật nội dung hợp đồng (Thay vì báo lỗi tồn tại)
        contract.Content = request.Content;
        contract.UpdatedAt = DateTime.UtcNow;

        // Reset lại chữ ký nếu sửa hợp đồng (Tùy logic nghiệp vụ, ở đây mình giữ nguyên hoặc xóa chữ ký cũ)
        // contract.ContractSignatures.Clear(); 

        await _contractRepo.Update(contract);

        return new ContractDetailDto
        {
            ContractId = contract.ContractId,
            CoOwnerGroupId = request.CoOwnerGroupId,
            Content = contract.Content,
            CreatedAt = contract.CreatedAt
        };
    }

    // ... (Các hàm GetContractByGroupAsync, SignContractAsync GIỮ NGUYÊN) ...
    public async Task<ContractDetailDto> GetContractByGroupAsync(int userId, int groupId)
    {
        var groups = await _groupRepo.GetGroupsByUserIdAsync(userId);
        if (!groups.Any(g => g.CoOwnerGroupId == groupId))
            throw new Exception("Bạn không phải thành viên nhóm.");

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