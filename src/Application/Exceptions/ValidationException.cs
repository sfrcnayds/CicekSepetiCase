using FluentValidation.Results;

namespace Application.Exceptions;

public class ValidationException : Exception
{
    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IDictionary<string, string[]>? failures)
        : this()
    {
        Errors = failures;
    }

    public IDictionary<string, string[]>? Errors { get; }
}