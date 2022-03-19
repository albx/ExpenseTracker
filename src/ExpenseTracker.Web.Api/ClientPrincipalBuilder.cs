using Microsoft.Azure.Functions.Worker.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ExpenseTracker.Web.Api;

public static class ClientPrincipalBuilder
{
    public static ClaimsPrincipal BuildFromHttpRequest(HttpRequestData request)
    {
        var principal = new ClientPrincipal();

        if (request.Headers.TryGetValues("x-ms-client-principal", out var header))
        {
            var data = header.First();
            var decoded = Convert.FromBase64String(data);
            var json = Encoding.ASCII.GetString(decoded);
            principal = JsonSerializer.Deserialize<ClientPrincipal>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        }

        principal.UserRoles = principal.UserRoles.Except(new string[] { "anonymous" }, StringComparer.CurrentCultureIgnoreCase) ?? Array.Empty<string>();

        if (!principal.UserRoles.Any())
        {
            return new ClaimsPrincipal();
        }

        var identity = new ClaimsIdentity(principal.IdentityProvider);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, principal.UserId));
        identity.AddClaim(new Claim(ClaimTypes.Name, principal.UserDetails));
        identity.AddClaims(principal.UserRoles.Select(r => new Claim(ClaimTypes.Role, r)));

        return new ClaimsPrincipal(identity);
    }

    private class ClientPrincipal
    {
        public string IdentityProvider { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserDetails { get; set; } = string.Empty;
        public IEnumerable<string> UserRoles { get; set; } = Array.Empty<string>();
    }
}
