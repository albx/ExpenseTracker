using ExpenseTracker.Web.Api.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ExpenseTracker.Web.Api;

public class ExpensesListFunction
{
    private readonly ILogger _logger;

    public ExpensesListFunction(ILoggerFactory loggerFactory, ExpensesApiService service)
    {
        _logger = loggerFactory.CreateLogger<ExpensesListFunction>();
        Service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public ExpensesApiService Service { get; }

    [Function("ExpensesList")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var response = req.CreateResponse(HttpStatusCode.OK);
        var model = await Service.GetAllExpensesAsync();

        await response.WriteAsJsonAsync(model);

        return response;
    }
}
