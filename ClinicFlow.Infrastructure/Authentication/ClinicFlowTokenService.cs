using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ClinicFlow.Application.Authentication;
using Microsoft.Extensions.Options;

namespace ClinicFlow.Infrastructure.Authentication;

public sealed class ClinicFlowTokenService(IOptions<ClinicFlowAuthenticationOptions> options) : IAuthenticationTokenService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly ClinicFlowAuthenticationOptions authenticationOptions = options.Value;

    public string CreateToken(AuthenticatedUser user, DateTimeOffset issuedAtUtc, DateTimeOffset expiresAtUtc)
    {
        ArgumentNullException.ThrowIfNull(user);

        ValidateOptions();

        var header = new Dictionary<string, object?>
        {
            ["alg"] = "HS256",
            ["typ"] = "JWT"
        };

        var payload = new Dictionary<string, object?>
        {
            ["iss"] = authenticationOptions.Issuer,
            ["aud"] = authenticationOptions.Audience,
            ["sub"] = user.Username,
            ["name"] = user.DisplayName,
            ["role"] = user.Role,
            ["iat"] = issuedAtUtc.ToUnixTimeSeconds(),
            ["exp"] = expiresAtUtc.ToUnixTimeSeconds(),
            ["jti"] = Guid.NewGuid().ToString("N")
        };

        var encodedHeader = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(header, SerializerOptions));
        var encodedPayload = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(payload, SerializerOptions));
        var data = $"{encodedHeader}.{encodedPayload}";
        var signature = Sign(data);

        return $"{data}.{Base64UrlEncode(signature)}";
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        ValidateOptions();

        var parts = token.Split('.');
        if (parts.Length != 3)
        {
            return null;
        }

        byte[] actualSignature;
        Dictionary<string, JsonElement>? payload;

        try
        {
            var data = $"{parts[0]}.{parts[1]}";
            var expectedSignature = Sign(data);
            actualSignature = Base64UrlDecode(parts[2]);

            if (!CryptographicOperations.FixedTimeEquals(expectedSignature, actualSignature))
            {
                return null;
            }

            payload = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(Base64UrlDecode(parts[1]), SerializerOptions);
        }
        catch
        {
            return null;
        }

        if (payload is null)
        {
            return null;
        }

        if (!TryGetString(payload, "iss", out var issuer) || !string.Equals(issuer, authenticationOptions.Issuer, StringComparison.Ordinal))
        {
            return null;
        }

        if (!TryGetString(payload, "aud", out var audience) || !string.Equals(audience, authenticationOptions.Audience, StringComparison.Ordinal))
        {
            return null;
        }

        if (!TryGetLong(payload, "exp", out var expiresAtSeconds))
        {
            return null;
        }

        DateTimeOffset expiresAt;
        try
        {
            expiresAt = DateTimeOffset.FromUnixTimeSeconds(expiresAtSeconds);
        }
        catch
        {
            return null;
        }

        if (expiresAt <= DateTimeOffset.UtcNow)
        {
            return null;
        }

        if (!TryGetString(payload, "sub", out var subject) || string.IsNullOrWhiteSpace(subject))
        {
            return null;
        }

        var name = TryGetString(payload, "name", out var displayName) ? displayName : subject;
        var role = TryGetString(payload, "role", out var userRole) ? userRole : "User";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, subject),
            new(ClaimTypes.Name, name),
            new(ClaimTypes.Role, role),
            new("iss", issuer),
            new("aud", audience)
        };

        var identity = new ClaimsIdentity(claims, ClinicFlowAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
        return new ClaimsPrincipal(identity);
    }

    private byte[] Sign(string data)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(authenticationOptions.SigningKey));
        return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
    }

    private void ValidateOptions()
    {
        if (string.IsNullOrWhiteSpace(authenticationOptions.SigningKey))
        {
            throw new InvalidOperationException("A chave de assinatura de autenticação não foi configurada.");
        }

        if (string.IsNullOrWhiteSpace(authenticationOptions.Issuer))
        {
            throw new InvalidOperationException("O issuer de autenticação não foi configurado.");
        }

        if (string.IsNullOrWhiteSpace(authenticationOptions.Audience))
        {
            throw new InvalidOperationException("A audience de autenticação não foi configurada.");
        }
    }

    private static bool TryGetString(IReadOnlyDictionary<string, JsonElement> payload, string key, out string value)
    {
        if (payload.TryGetValue(key, out var element) && element.ValueKind == JsonValueKind.String)
        {
            value = element.GetString() ?? string.Empty;
            return true;
        }

        value = string.Empty;
        return false;
    }

    private static bool TryGetLong(IReadOnlyDictionary<string, JsonElement> payload, string key, out long value)
    {
        if (payload.TryGetValue(key, out var element) && element.TryGetInt64(out value))
        {
            return true;
        }

        value = default;
        return false;
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static byte[] Base64UrlDecode(string value)
    {
        var padded = value.Replace('-', '+').Replace('_', '/');
        switch (padded.Length % 4)
        {
            case 2:
                padded += "==";
                break;
            case 3:
                padded += "=";
                break;
        }

        return Convert.FromBase64String(padded);
    }
}
