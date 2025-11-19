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

        // 2. Check xem nhóm đã có hợp đồng chưa
        var existing = await _contractRepo.GetByGroupIdAsync(request.CoOwnerGroupId);
        if (existing != null) throw new Exception("Nhóm này đã có hợp đồng.");

        // 3. Tạo hợp đồng
        var contract = new OwnershipContract
        {
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
            // Contract chưa liên kết vào Group ngay ở DB vì FK nằm ở bảng Group
            // Ta cần update bảng Group sau khi save contract, hoặc dùng navigation
        };

        // Lưu Contract trước để lấy ID
        await _contractRepo.Add(contract);

        // 4. Cập nhật Group trỏ về Contract này
        var group = await _groupRepo.GetByIdAsync(request.CoOwnerGroupId);
        if (group != null)
        {
            group.ContractId = contract.ContractId;
            await _groupRepo.Update(group);
        }

        return new ContractDetailDto
        {
            ContractId = contract.ContractId,
            CoOwnerGroupId = request.CoOwnerGroupId,
            Content = contract.Content,
            CreatedAt = contract.CreatedAt
        };
    }

    public async Task<ContractDetailDto> GetContractByGroupAsync(int userId, int groupId)
    {
        // Validate thành viên
        var groups = await _groupRepo.GetGroupsByUserIdAsync(userId);
        if (!groups.Any(g => g.CoOwnerGroupId == groupId))
            throw new Exception("Bạn không phải thành viên nhóm.");

        var contract = await _contractRepo.GetByGroupIdAsync(groupId);
        if (contract == null) throw new Exception("Nhóm chưa có hợp đồng.");

        // Lấy danh sách thành viên để map trạng thái ký
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
        // 1. Check đã ký chưa
        var signed = await _contractRepo.HasUserSignedAsync(contractId, userId);
        if (signed) throw new Exception("Bạn đã ký hợp đồng này rồi.");

        // 2. Tạo chữ ký
        var signature = new ContractSignature
        {
            ContractId = contractId,
            UserId = userId,
            SignedAt = DateTime.UtcNow,
            Status = 1 // 1 = Signed
        };

        await _contractRepo.AddSignatureAsync(signature);
        await _contractRepo.DbContext().SaveChangesAsync();
    }
}