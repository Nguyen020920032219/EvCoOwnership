namespace ContractGroupService.Business.Models;

public class SignatureStatusDto
{
    public int UserId { get; set; }
    public bool HasSigned { get; set; }
    public DateTime? SignedAt { get; set; }
}