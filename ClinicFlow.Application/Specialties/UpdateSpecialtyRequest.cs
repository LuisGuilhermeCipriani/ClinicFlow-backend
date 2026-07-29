namespace ClinicFlow.Application.Specialties;

public sealed record UpdateSpecialtyRequest(string Name, string? Description, ClinicFlow.Domain.Specialties.SpecialtyStatus Status);
