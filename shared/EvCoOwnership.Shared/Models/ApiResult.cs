namespace EvCoOwnership.Shared.Models;

public class ApiResult<T>
{
    public bool Success { get; set; }

    public string? ErrorCode { get; set; }

    public string? Message { get; set; }

    public T? Data { get; set; }

    public static ApiResult<T> Ok(T data, string? message = null)
    {
        return new ApiResult<T>
        {
            Success = true,
            Data = data,
            Message = message,
            ErrorCode = null
        };
    }

    public static ApiResult<T> Fail(string message, string? errorCode = null)
    {
        return new ApiResult<T>
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode
        };
    }
}