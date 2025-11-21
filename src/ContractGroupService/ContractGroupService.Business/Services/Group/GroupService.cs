using ContractGroupService.Business.Models;
using ContractGroupService.Business.Services.Groups;
using ContractGroupService.Data.Entities;
using ContractGroupService.Data.Repositories.Groups;
using Microsoft.EntityFrameworkCore;

namespace ContractGroupService.Business.Services.Group;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepo;

    public GroupService(IGroupRepository groupRepo)
    {
        _groupRepo = groupRepo;
    }

    public async Task<GroupDetailDto> CreateGroupAsync(int creatorUserId, CreateGroupRequest request)
    {
        // 1. Kiểm tra danh sách thành viên (Thêm người tạo nếu thiếu)
        var membersList = request.InitialMembers.ToList();
        if (!membersList.Any(m => m.UserId == creatorUserId))
        {
            // Logic tự thêm hoặc báo lỗi tùy bạn. Ở đây mình để nguyên logic cũ.
        }

        // 2. Validate 100% share
        var totalShare = membersList.Sum(m => m.SharePercent);
        if (Math.Abs(totalShare - 1.0m) > 0.001m)
            throw new Exception($"Tổng tỷ lệ sở hữu phải là 100%. Hiện tại: {totalShare * 100}%.");

        // 3. Tạo Group KÈM Hợp đồng nháp (Quan trọng)
        // Vì DB bắt buộc có ContractId, ta tạo luôn object Contract để EF tự insert
        var group = new CoOwnershipGroup
        {
            GroupName = request.GroupName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,

            // --- KHỞI TẠO HỢP ĐỒNG NHÁP TẠI ĐÂY ---
            Contract = new OwnershipContract
            {
                Content = "Hợp đồng nháp - Đang chờ soạn thảo", // Nội dung tạm
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        // 4. Map Members & Shares
        foreach (var mem in membersList)
        {
            group.CoOwnershipMembers.Add(new CoOwnershipMember
            {
                UserId = mem.UserId,
                IsGroupAdmin = mem.IsAdmin || mem.UserId == creatorUserId
            });

            group.OwnershipShares.Add(new OwnershipShare
            {
                UserId = mem.UserId,
                OwnershipPercent = mem.SharePercent
            });
        }

        // 5. Save
        // EF Core sẽ tự động: Insert Contract -> Lấy ID -> Insert Group với ContractId đó
        await _groupRepo.Add(group);

        // 6. Response
        return new GroupDetailDto
        {
            GroupId = group.CoOwnerGroupId,
            GroupName = group.GroupName,
            CreatedAt = group.CreatedAt,
            Members = membersList
        };
    }

    // ... (Các hàm GetMyGroupsAsync, GetGroupDetailAsync, GetGroups GIỮ NGUYÊN) ...
    public async Task<List<CoOwnerGroupDto>> GetMyGroupsAsync(int userId)
    {
        var groups = await _groupRepo.GetGroupsByUserIdAsync(userId);
        return groups.Select(g => new CoOwnerGroupDto
        {
            CoOwnerGroupId = g.CoOwnerGroupId,
            GroupName = g.GroupName,
            ContractId = g.ContractId, // Không bao giờ null nữa
            CreatedAt = g.CreatedAt
        }).ToList();
    }

    public async Task<GroupDetailDto> GetGroupDetailAsync(int groupId, int userId)
    {
        var group = await _groupRepo.GetGroupDetailAsync(groupId);
        if (group == null) throw new Exception("Group not found");

        var isMember = group.CoOwnershipMembers.Any(m => m.UserId == userId);
        if (!isMember) throw new Exception("Bạn không phải thành viên của nhóm này.");

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