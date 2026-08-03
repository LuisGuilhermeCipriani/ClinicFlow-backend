namespace ClinicFlow.Infrastructure.Authentication;

public sealed class ClinicFlowAuthenticationUserOptions
{
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Role { get; set; } = "User";
}
