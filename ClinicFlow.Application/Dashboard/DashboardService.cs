namespace ClinicFlow.Application.Dashboard;

public sealed class DashboardService(IDashboardRepository repository) : IDashboardService
{
    public Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetSummaryAsync(cancellationToken);
    }
}
