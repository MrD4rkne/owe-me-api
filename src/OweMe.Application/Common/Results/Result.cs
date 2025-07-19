using System.Text;
using System.Text.Json.Serialization;

namespace OweMe.Application.Common.Results;

public record Result<T>
{
    private readonly T _value;

    public Result(T value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null");
        IsSuccess = true;
        Error = Error.None;
    }

    public Result(Error error)
    {
        IsSuccess = false;
        Error = error ?? throw new ArgumentNullException(nameof(error), "Error cannot be null");
        _value = default!;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Value => IsSuccess ? _value : default;

    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }

    public static Result<T> Failure(Error error)
    {
        return new Result<T>(error);
    }

    public static implicit operator Result<T>(T value)
    {
        return Success(value);
    }

    public static implicit operator Result<T>(Error error)
    {
        return Failure(error);
    }

    protected virtual bool PrintMembers(StringBuilder stringBuilder)
    {
        stringBuilder.Append($"IsSuccess = {IsSuccess}, ");
        if (IsSuccess)
        {
            stringBuilder.Append($"Value = {Value}, ");
        }
        else
        {
            stringBuilder.Append($"Error = {Error.Code}");
        }

        return true;
    }
}