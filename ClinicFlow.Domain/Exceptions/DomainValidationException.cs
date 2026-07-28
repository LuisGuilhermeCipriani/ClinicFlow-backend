namespace ClinicFlow.Domain.Exceptions;

public sealed class DomainValidationException : DomainException
{
    public IReadOnlyCollection<string> Errors { get; }

    public DomainValidationException(IEnumerable<string> errors)
        : base("Uma ou mais validações de domínio falharam.")
    {
        ArgumentNullException.ThrowIfNull(errors);

        var normalizedErrors = errors
            .Where(error => !string.IsNullOrWhiteSpace(error))
            .Select(error => error.Trim())
            .ToArray();

        if (normalizedErrors.Length == 0)
        {
            throw new ArgumentException("Ao menos um erro de validação deve ser informado.", nameof(errors));
        }

        Errors = normalizedErrors;
    }
}
