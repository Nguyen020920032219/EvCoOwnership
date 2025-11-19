namespace AuthService.Business.Models;

public class UpdateUserByAdminRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int? RoleId { get; set; } // Cho phép đổi quyền
}