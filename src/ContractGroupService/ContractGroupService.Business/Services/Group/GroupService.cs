using ContractGroupService.Business.Models;
using ContractGroupService.Data.Entities;
using ContractGroupService.Data.Repositories.Groups;
using Microsoft.EntityFrameworkCore;

namespace ContractGroupService.Business.Services.Groups;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepo;

    public GroupService(IGroupRepository groupRepo)
    {
        _groupRepo = groupRepo;
    }

    public async Task<GroupDetailDto> CreateGroupAsync(int creatorUserId, CreateGroupRequest request)
    {
        // 1. Validate Tổng tỷ lệ sở hữu phải là 100% (1.0)
        // Người tạo cũng phải có phần, cộng với các thành viên khác
        var totalShare = request.InitialMembers.Sum(m => m.SharePercent);

        // Giả sử request chứa toàn bộ danh sách chia phần, bao gồm cả người tạo
        // Nếu request ko có người tạo, ta phải tự thêm logic. 
        // Ở đây giả định request gửi lên danh sách FULL.
        if (Math.Abs(totalShare - 1.0m) > 0.001m) // So sánh số thực
            throw new Exception($"Tổng tỷ lệ sở hữu phải là 100%. Hiện tại: {totalShare * 100}%");

        // 2. Tạo Group
        var group = new CoOwnershipGroup
        {
            GroupName = request.GroupName,
            ContractId = 0, // TODO: Sẽ update sau khi tạo Contract
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // 3. Map Members & Shares
        foreach (var mem in request.InitialMembers)
        {
            group.CoOwnershipMembers.Add(new CoOwnershipMember
            {
                UserId = mem.UserId,
                IsGroupAdmin = mem.IsAdmin || mem.UserId == creatorUserId // Người tạo auto là Admin
            });

            group.OwnershipShares.Add(new OwnershipShare
            {
                UserId = mem.UserId,
                OwnershipPercent = mem.SharePercent
            });
        }

        // 4. Save (Cascading insert: Group -> Members -> Shares)
        await _groupRepo.Add(group);
        // await _groupRepo.SaveChangesAsync(); 

        // 5. Response
        return new GroupDetailDto
        {
            GroupId = group.CoOwnerGroupId,
            GroupName = group.GroupName,
            CreatedAt = group.CreatedAt,
            Members = request.InitialMembers
        };
    }

    public async Task<List<CoOwnerGroupDto>> GetMyGroupsAsync(int userId)
    {
        var groups = await _groupRepo.GetGroupsByUserIdAsync(userId);
        return groups.Select(g => new CoOwnerGroupDto
        {
            CoOwnerGroupId = g.CoOwnerGroupId,
            GroupName = g.GroupName,
            ContractId = g.ContractId,
            CreatedAt = g.CreatedAt
        }).ToList();
    }

    public async Task<GroupDetailDto> GetGroupDetailAsync(int groupId, int userId)
    {
        // Check quyền: User có nằm trong nhóm này không?
        var group = await _groupRepo.GetGroupDetailAsync(groupId);
        if (group == null) throw new Exception("Group not found");

        var isMember = group.CoOwnershipMembers.Any(m => m.UserId == userId);
        if (!isMember) throw new Exception("Bạn không phải thành viên của nhóm này.");

        // Map data
        return new GroupDetailDto
        {
            GroupId = group.CoOwnerGroupId,
            GroupName = group.GroupName,
            CreatedAt = group.CreatedAt,
            Members = group.OwnershipShares.Select(s => new MemberShareDto
            {
                UserId = s.UserId,
                SharePercent = s.OwnershipPercent,
                IsAdmin = group.CoOwnershipMembers.First(m => m.UserId == s.UserId).IsGroupAdmin
            }).ToList()
        };
    }

    public async Task<List<CoOwnerGroupDto>> GetGroups()
    {
        return await _groupRepo.DbSet().Select(c => new CoOwnerGroupDto
        {
            ContractId = c.ContractId,
            CoOwnerGroupId = c.CoOwnerGroupId,
            CreatedAt = c.CreatedAt,
            GroupName = c.GroupName
        }).ToListAsync();
    }
}