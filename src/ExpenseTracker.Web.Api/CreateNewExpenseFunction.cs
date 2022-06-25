using ExpenseTracker.Web.Api.Services;
using ExpenseTracker.Web.Shared.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Security.Claims;

namespace ExpenseTracker.Web.Api;

public class CreateNewExpenseFunction
{
    private readonly ILogger _logger;

    public CreateNewExpenseFunction(ILoggerFactory loggerFactory, ExpensesApiService service)
    {
        _logger = loggerFactory.CreateLogger<CreateNewExpenseFunction>();
        Service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public ExpensesApiService Service { get; }

    [Function("CreateNewExpense")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var model = await req.ReadFromJsonAsync<ExpenseModel>();
        if (model is null)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        var identity = ClientPrincipalBuilder.BuildFromHttpRequest(req);
        var userId = identity.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?
            .Value ?? string.Empty;

        await Service.CreateNewExpenseAsync(model, userId);

        var response = req.CreateResponse(HttpStatusCode.Created);
        //response.Headers.Add("Location", "http://localhost");

        await response.WriteAsJsonAsync(model, HttpStatusCode.Created);

        return response;
    }
}
