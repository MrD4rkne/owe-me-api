namespace OweMe.Application.Common.Results;

public record Result<T>
{
    private readonly T _value;

    private Result(bool isSuccess, Error error, T value)
    {
        if ((isSuccess && !error.Equals(Error.None)) ||
            (!isSuccess && error.Equals(Error.None)))
            throw new ArgumentException("Invalid error", nameof(error));

        IsSuccess = isSuccess;
        Error = error;
        _value = value;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; private init; }

    public T Value =>
        IsSuccess ? _value : throw new InvalidOperationException("Cannot access Value on a failed result.");

    public static Result<T> Success(T value)
    {
        return new Result<T>(true, Error.None, value);
    }

    public static Result<T> Failure(Error error)
    {
        return new Result<T>(false, error, default!);
    }

    public static implicit operator Result<T>(T value)
    {
        return Success(value);
    }

    public static implicit operator Result<T>(Error error)
    {
        return Failure(error);
    }
}