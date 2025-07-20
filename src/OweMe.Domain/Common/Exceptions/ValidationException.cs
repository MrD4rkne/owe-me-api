namespace OweMe.Domain.Common.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
    }

    public ValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public ValidationException(string message, IEnumerable<KeyValuePair<string, string>> errors) : base(message)
    {
        Errors = errors;
    }

    public IEnumerable<KeyValuePair<string, string>> Errors { get; } = [];
}