using Microsoft.Azure.Functions.Worker.Http;
using System.Security.Claims;

namespace ExpenseTracker.Web.Api.Extensions;

public static class HttpRequestDataExtensions
{
    public static string GetUserId(this HttpRequestData request)
    {
        var identity = ClientPrincipalBuilder.BuildFromHttpRequest(request);
        var userId = identity.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?
            .Value ?? string.Empty;

        return userId;
    }
}
