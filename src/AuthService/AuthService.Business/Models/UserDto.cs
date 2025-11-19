namespace AuthService.Business.Models;

public class UserDto
{
    public int UserId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsActive { get; set; }
}