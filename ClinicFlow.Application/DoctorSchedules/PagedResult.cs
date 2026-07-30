namespace ClinicFlow.Application.DoctorSchedules;

public sealed record PagedResult<T>(IReadOnlyCollection<T> Items, int Page, int PageSize, int TotalCount);
