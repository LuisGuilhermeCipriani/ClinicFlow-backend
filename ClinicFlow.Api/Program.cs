using ClinicFlow.Application;
using ClinicFlow.Application.Authentication;
using ClinicFlow.Infrastructure;
using ClinicFlow.Api.Authentication;
using ClinicFlow.Infrastructure.Persistence.HealthChecks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks().AddCheck<OracleDatabaseHealthCheck>("oracle_database");
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddAuthentication(ClinicFlowAuthenticationDefaults.AuthenticationScheme)
    .AddScheme<AuthenticationSchemeOptions, ClinicFlowBearerAuthenticationHandler>(
        ClinicFlowAuthenticationDefaults.AuthenticationScheme,
        options => { });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(ClinicFlowAuthorizationPolicies.ViewClinicData, policy =>
    {
        policy.RequireRole(ClinicFlowRoles.Admin, ClinicFlowRoles.Receptionist, ClinicFlowRoles.Doctor);
    });

    options.AddPolicy(ClinicFlowAuthorizationPolicies.ManageClinicData, policy =>
    {
        policy.RequireRole(ClinicFlowRoles.Admin, ClinicFlowRoles.Receptionist);
    });

    options.AddPolicy(ClinicFlowAuthorizationPolicies.ManageUsers, policy =>
    {
        policy.RequireRole(ClinicFlowRoles.Admin);
    });

    options.AddPolicy(ClinicFlowAuthorizationPolicies.ViewClinicalRecords, policy =>
    {
        policy.RequireRole(ClinicFlowRoles.Admin, ClinicFlowRoles.Doctor);
    });

    options.AddPolicy(ClinicFlowAuthorizationPolicies.ManageClinicalRecords, policy =>
    {
        policy.RequireRole(ClinicFlowRoles.Admin, ClinicFlowRoles.Doctor);
    });

    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        if (allowedOrigins.Length == 0)
        {
            policy.AllowAnyHeader().AllowAnyMethod();
            return;
        }

        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health").AllowAnonymous();

app.Run();
