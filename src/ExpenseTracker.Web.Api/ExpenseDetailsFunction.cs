using ExpenseTracker.Web.Api.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ExpenseTracker.Web.Api;

public class ExpenseDetailsFunction
{
    private readonly ILogger _logger;

    public ExpenseDetailsFunction(ILoggerFactory loggerFactory, ExpensesApiService service)
    {
        _logger = loggerFactory.CreateLogger<ExpenseDetailsFunction>();
        Service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public ExpensesApiService Service { get; }

    [Function("ExpenseDetails")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ExpenseDetails/{id}")] HttpRequestData req,
        [Required] Guid id)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        if (id == Guid.Empty)
        {
            var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            return errorResponse;
        }

        var expense = await Service.GetExpenseDetailsAsync(id);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(expense);

        return response;
    }
}
