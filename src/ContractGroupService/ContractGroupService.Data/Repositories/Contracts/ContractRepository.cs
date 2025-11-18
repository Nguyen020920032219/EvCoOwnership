using ContractGroupService.Data.Configurations;
using ContractGroupService.Data.Entities;
using EvCoOwnership.Shared.BaseRepository;
using Microsoft.EntityFrameworkCore;

namespace ContractGroupService.Data.Repositories.Contracts;

public class ContractRepository : BaseRepository<ContractGroupDbContext, OwnershipContract, int>, IContractRepository
{
    public ContractRepository(ContractGroupDbContext context) : base(context)
    {
    }

    public async Task<OwnershipContract?> GetByGroupIdAsync(int groupId)
    {
        // Tìm Contract dựa vào quan hệ ngược từ Group hoặc query trực tiếp nếu có FK
        // Trong DB schema: Group có ContractId, nhưng Contract cũng có navigation CoOwnershipGroup
        // Cách an toàn nhất là join từ bảng Group
        var group = await _context.CoOwnershipGroups
            .Include(g => g.Contract)
            .ThenInclude(c => c.ContractSignatures)
            .FirstOrDefaultAsync(g => g.CoOwnerGroupId == groupId);

        return group?.Contract;
    }

    public async Task<bool> HasUserSignedAsync(int contractId, int userId)
    {
        return await _context.ContractSignatures
            .AnyAsync(s => s.ContractId == contractId && s.UserId == userId && s.Status == 1); // 1 = Signed
    }

    public async Task AddSignatureAsync(ContractSignature signature)
    {
        await _context.ContractSignatures.AddAsync(signature);
    }
}