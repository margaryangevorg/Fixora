namespace Fixora.API.Models;

public class ServiceResult<T>
{
    public bool IsSuccess { get; private init; }
    public T? Data { get; private init; }
    public string? ErrorMessage { get; private init; }
    public bool IsNotFound { get; private init; }

    public static ServiceResult<T> Ok(T data) => new() { IsSuccess = true, Data = data };
    public static ServiceResult<T> NotFound(string message) => new() { IsNotFound = true, ErrorMessage = message };
    public static ServiceResult<T> Error(string message) => new() { ErrorMessage = message };
}
