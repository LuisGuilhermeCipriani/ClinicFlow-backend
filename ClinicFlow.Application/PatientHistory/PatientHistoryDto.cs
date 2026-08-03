namespace ClinicFlow.Application.PatientHistory;

public sealed record PatientHistoryDto(
    long PatientId,
    string PatientName,
    IReadOnlyCollection<PatientHistoryEntryDto> Entries);
