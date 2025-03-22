using Daric.Domain.Shared.Extensions;

namespace Daric.Domain.Shared;

public enum ErrorCode
{
    InternalError,
    InvalidData,
    InvalidOperation,
    NullOrEmpty,
    NotFound
}
public record Error(ErrorCode Code, string Message)
{
    public string GetErrorMessage()
    {
        return string.Format("An error occurred due to {0}({1})", Code, Message);
    }
}
public record NullOrEmptyError(string Message) : Error(ErrorCode.NullOrEmpty, Message);
public record InvalidDataError(string Message) : Error(ErrorCode.InvalidData, Message);
public record InternalError(string Message) : Error(ErrorCode.InternalError, Message)
{
    public InternalError(Exception ex) : this(ex.GetMessage())
    { }
}
public record InvalidOperationError(string Message) : Error(ErrorCode.InvalidOperation, Message);
public record ResourceNotFoundError(string Message) : Error(ErrorCode.NotFound, Message);
public record ErrorOr<TResult>(TResult? Result, Error[] Errors, bool Success)
{
    public ErrorOr(TResult Result, Error[] Errors) : this(Result, Errors, Errors.Length == 0) { }
    public ErrorOr(Error[] Errors) : this(default, Errors, false) { }
    public ErrorOr(Exception ex) : this(default, [new InternalError(ex)], false) { }
    public ErrorOr(TResult Result) : this(Result, [], true) { }

    public static implicit operator bool(ErrorOr<TResult>? errorOr) => errorOr is not null && errorOr.Success;
    public static implicit operator ErrorOr<TResult>(Error error) => new([error]);
    public static implicit operator ErrorOr<TResult>(Exception ex) => new(ex);
    public static implicit operator TResult?(ErrorOr<TResult> errorOr) => errorOr is null ? default : errorOr.Result;
    public static implicit operator ErrorOr<TResult>(TResult result) => new(result);
    public static implicit operator ErrorOr<TResult>(List<Error> errors) => new(default, [.. errors], false);
    public static implicit operator ErrorOr<TResult>(Error[] errors) => new(default, [.. errors], false);
}
