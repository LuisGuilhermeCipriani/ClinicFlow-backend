namespace ClinicFlow.Domain.Primitives;

public sealed class DomainResult
{
    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public string? ErrorMessage { get; }

    private DomainResult(bool isSuccess, string? errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static DomainResult Success()
    {
        return new DomainResult(true, null);
    }

    public static DomainResult Failure(string errorMessage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);

        return new DomainResult(false, errorMessage.Trim());
    }
}
