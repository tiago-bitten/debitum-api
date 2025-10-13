using System.Diagnostics.CodeAnalysis;

namespace Domain.Shared.Results;

public class Result
{
    protected Result(bool isSuccess, Error error, bool mustComplete = false)
    {
        if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
        MustComplete = mustComplete;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }
    public bool MustComplete { get; }

    public int StatusCode => IsSuccess ? 200 : Error.StatusCode;
    public static Result Ok() => new(true, Error.None);
    public static Result<TValue> Failure<TValue>(Error error) => new(false, default!, error);
    public static Result<TValue> Failure<TValue>(Error error, bool mustComplete) => new(false, default!, error, mustComplete);

    public static implicit operator Result(Error error) => new(false, error);
}

public class Result<T>(bool isSuccess, T? value, Error error, bool mustComplete = false)
    : Result(isSuccess, error, mustComplete)
{
    [NotNull]
    public T Value => IsSuccess
        ? value!
        : throw new InvalidOperationException("The value is not available.");

    private static Result<T> Ok(T value) => new(true, value, Error.None);
    private static Result<T> Failure(Error error) => new(false, default!, error);
    private static Result<T> Failure(Error error, bool mustComplete) => new(false, default!, error, mustComplete);

    public static implicit operator Result<T>(Error error) => Failure(error);
    public static implicit operator Result<T>(T value) => Ok(value);

    public static Result<T> ValidationFailure(Error error) => new(false, default, error);
}
