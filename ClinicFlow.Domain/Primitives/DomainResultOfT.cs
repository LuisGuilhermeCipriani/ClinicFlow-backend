namespace ClinicFlow.Domain.Primitives;

public sealed class DomainResult<T>
{
    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public T? Value { get; }

    public string? ErrorMessage { get; }

    private DomainResult(bool isSuccess, T? value, string? errorMessage)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorMessage = errorMessage;
    }

    public static DomainResult<T> Success(T value)
    {
        return new DomainResult<T>(true, value, null);
    }

    public static DomainResult<T> Failure(string errorMessage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);

        return new DomainResult<T>(false, default, errorMessage.Trim());
    }
}
