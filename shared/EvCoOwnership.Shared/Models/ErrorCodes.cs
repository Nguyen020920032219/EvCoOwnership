namespace EvCoOwnership.Shared.Models;

public static class ErrorCodes
{
    // Nhóm lỗi hệ thống chung
    public const string Unknown = "ERR_UNKNOWN";
    public const string ValidationFailed = "ERR_VALIDATION_FAILED";
    public const string Unauthorized = "ERR_UNAUTHORIZED";
    public const string Forbidden = "ERR_FORBIDDEN";
    public const string NotFound = "ERR_NOT_FOUND";
    public const string Conflict = "ERR_CONFLICT";
    public const string BadRequest = "ERR_BAD_REQUEST";

    // Nhóm lỗi Auth
    public const string Auth_InvalidCredential = "AUTH_INVALID_CREDENTIAL";
    public const string Auth_PhoneAlreadyExists = "AUTH_PHONE_ALREADY_EXISTS";
    public const string Auth_UserNotFound = "AUTH_USER_NOT_FOUND";

    // Nhóm lỗi Group / Co-ownership
    public const string Group_NotFound = "GROUP_NOT_FOUND";
    public const string Group_UserNotMember = "GROUP_USER_NOT_MEMBER";
    public const string Group_UserNotAdmin = "GROUP_USER_NOT_ADMIN";
    public const string Group_OwnershipShareInvalid = "GROUP_OWNERSHIP_SHARE_INVALID";

    // Nhóm lỗi Booking
    public const string Booking_NotFound = "BOOKING_NOT_FOUND";
    public const string Booking_Conflict = "BOOKING_CONFLICT";
    public const string Booking_NotOwnerOrMember = "BOOKING_NOT_OWNER_OR_MEMBER";

    // Nhóm lỗi Finance
    public const string Finance_NotFound = "FINANCE_NOT_FOUND";
    public const string Finance_InvalidShare = "FINANCE_INVALID_SHARE";
    public const string Finance_PaymentFailed = "FINANCE_PAYMENT_FAILED";
}