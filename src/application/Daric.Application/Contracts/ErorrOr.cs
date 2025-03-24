using Mapster;

namespace Daric.Application.Contracts
{
    public enum ErrorCode
    {
        InternalError,
        InvalidData,
        InvalidOperation,
        NullOrEmpty,
        NotFound,
        Lock
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
    public record LockError(string Message) : Error(ErrorCode.Lock, Message)
    {
        public LockError() : this("Can not do the operation! please try again later")
        {
        }
    }
    public record InternalError(string Message) : Error(ErrorCode.InternalError, Message)
    {
        public InternalError(Exception ex) : this(ex.Message)
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

        public static implicit operator bool(ErrorOr<TResult> errorOr) => errorOr.Success;
        public static implicit operator ErrorOr<TResult>(Error error) => new([error]);
        public static implicit operator ErrorOr<TResult>(Exception ex) => new(ex);
        public static implicit operator TResult?(ErrorOr<TResult> errorOr) => errorOr.Result;
        public static implicit operator ErrorOr<TResult>(TResult result) => new(result);
        public static implicit operator ErrorOr<TResult>(List<Error> errors) => new(default, [.. errors], false);
        public static implicit operator ErrorOr<TResult>(Error[] errors) => new(default, [.. errors], false);

        public static implicit operator ErrorOr<TResult>(List<Domain.Shared.Error> errors) => new(default, [.. errors.Adapt<List<Error>>()], false);
        public static implicit operator ErrorOr<TResult>(Domain.Shared.Error[] errors) => new(default, [.. errors.Adapt<Error[]>()], false);
    }

}
