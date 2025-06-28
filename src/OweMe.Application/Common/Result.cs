namespace OweMe.Application.Common;

public readonly struct Result
{
    private Result(bool isSuccess, Error error)
    {
        if ((isSuccess && !error.Equals(Error.None)) ||
            (!isSuccess && error.Equals(Error.None)))
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; private init; }

    public static Result Success()
    {
        return new Result(true, Error.None);
    }

    public static Result Failure(Error error)
    {
        return new Result(false, error);
    }
}

public readonly struct Result<T>
{
    private readonly T _value;

    private Result(bool isSuccess, Error error, T value)
    {
        if ((isSuccess && !error.Equals(Error.None)) ||
            (!isSuccess && error.Equals(Error.None)))
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

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
}