using ExpenseTracker.Web.Api.Extensions;
using ExpenseTracker.Web.Api.Services;
using ExpenseTracker.Web.Shared.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

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

        var userId = req.GetUserId();
        await Service.CreateShoppingListAsync(model, userId);

        var response = req.CreateResponse(HttpStatusCode.Created);
        response.Headers.Add("Content-Type", "application/json");

        return response;
    }
}
