namespace NVBM.Application.DTOs;

public record APIResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? ErrorMessage { get; init; }
    public string? Message { get; init; }

    public static APIResponse<T> Ok(T data, string? message = "Success") => new() { Success = true, Data = data, Message = message };
    public static APIResponse<T> Fail(string message) => new() { Success = false, ErrorMessage = message, Message = message };
}
