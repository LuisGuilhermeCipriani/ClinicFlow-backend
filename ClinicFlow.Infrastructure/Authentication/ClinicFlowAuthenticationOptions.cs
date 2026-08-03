namespace ClinicFlow.Infrastructure.Authentication;

public sealed class ClinicFlowAuthenticationOptions
{
    public const string SectionName = "Authentication";

    public string Issuer { get; set; } = "ClinicFlow";

    public string Audience { get; set; } = "ClinicFlow.Web";

    public string SigningKey { get; set; } = string.Empty;

    public int TokenLifetimeMinutes { get; set; } = 480;

    public List<ClinicFlowAuthenticationUserOptions> Users { get; set; } = [];
}
