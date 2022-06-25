using ExpenseTracker.Web.Api.Services;
using ExpenseTracker.Web.Shared.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Security.Claims;

namespace ExpenseTracker.Web.Api;

public class CreateShoppingListFunction
{
    private readonly ILogger _logger;

    public ShoppingListApiService Service { get; }

    public CreateShoppingListFunction(ILoggerFactory loggerFactory, ShoppingListApiService service)
    {
        _logger = loggerFactory.CreateLogger<CreateShoppingListFunction>();
        Service = service ?? throw new ArgumentNullException(nameof(service));
    }

    [Function("CreateShoppingList")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var model = await req.ReadFromJsonAsync<ShoppingListModel>();
        if (model is null)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        var identity = ClientPrincipalBuilder.BuildFromHttpRequest(req);
        var userId = identity.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?
            .Value ?? string.Empty;

        await Service.CreateShoppingListAsync(model, userId);

        var response = req.CreateResponse(HttpStatusCode.Created);
        response.Headers.Add("Content-Type", "application/json");

        return response;
    }
}
