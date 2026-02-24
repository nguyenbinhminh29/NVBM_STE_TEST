namespace NVBM.Application.DTOs;

public record APIResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? ErrorMessage { get; init; }
    public string? Message { get; init; }
    public IEnumerable<string>? Errors { get; init; }

    public static APIResponse<T> Ok(T data, string? message = "Success") => new() { Success = true, Data = data, Message = message };
    public static APIResponse<T> Fail(string message) => new() { Success = false, ErrorMessage = message, Message = message };
    public static APIResponse<T> FailWithErrors(IEnumerable<string> errors, string message = "Validation Failed") => 
        new() { Success = false, Message = message, Errors = errors };
}
