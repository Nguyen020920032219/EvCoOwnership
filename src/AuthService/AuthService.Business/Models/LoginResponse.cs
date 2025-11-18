namespace AuthService.Business.Models;

public class LoginResponse
{
    public int UserId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
}