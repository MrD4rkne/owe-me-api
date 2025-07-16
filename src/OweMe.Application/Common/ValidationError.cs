using FluentValidation.Results;

namespace OweMe.Application.Common;

public record ValidationError(Error[] Errors) : Error("ValidationError", "One or more validation errors occurred.")
{
    public Error[] Errors { get; } = Errors ?? [];

    public virtual bool Equals(ValidationError? other)
    {
        if (!base.Equals(other)) return false;

        return Errors.SequenceEqual(other.Errors);
    }

    public override string ToString()
    {
        return $"{Code}: {Description} - Errors: {string.Join(", ", Errors.Select(e => $"{e.Code}: {e.Description}"))}";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Errors);
    }

    public static ValidationError From(ValidationFailure[] failures)
    {
        ArgumentNullException.ThrowIfNull(failures);
        if (failures.Length == 0) throw new ArgumentException("Validation failures cannot be empty.", nameof(failures));

        var errors = failures
            .Select(f => new Error(f.PropertyName, f.ErrorMessage))
            .ToArray();
        return new ValidationError(errors);
    }
}