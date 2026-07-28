using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ClinicFlow.Infrastructure.Persistence.HealthChecks;

public sealed class OracleDatabaseHealthCheck(ClinicFlowDbContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken).ConfigureAwait(false);

            return canConnect
                ? HealthCheckResult.Healthy("Oracle Database está acessível.")
                : HealthCheckResult.Unhealthy("Não foi possível conectar ao Oracle Database.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("A verificação de saúde do Oracle falhou.", exception);
        }
    }
}
