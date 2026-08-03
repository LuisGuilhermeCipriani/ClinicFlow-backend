namespace ClinicFlow.Application.Authentication;

public interface IUserPasswordHasher
{
    string HashPassword(string password);

    bool VerifyPassword(string password, string passwordHash);
}
