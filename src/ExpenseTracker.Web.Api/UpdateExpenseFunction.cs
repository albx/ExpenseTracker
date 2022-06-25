using ExpenseTracker.Web.Api.Services;
using ExpenseTracker.Web.Shared.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace ExpenseTracker.Web.Api;

public class UpdateExpenseFunction
{
    private readonly ILogger _logger;

    public UpdateExpenseFunction(ILoggerFactory loggerFactory, ExpensesApiService service)
    {
        _logger = loggerFactory.CreateLogger<UpdateExpenseFunction>();
        Service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public ExpensesApiService Service { get; }

    [Function("UpdateExpense")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "UpdateExpense/{id}")] HttpRequestData req,
        [Required] Guid id)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var model = await req.ReadFromJsonAsync<ExpenseModel>();

        if (id == Guid.Empty || model is null)
        {
            var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            return errorResponse;
        }

        try
        {
            await Service.UpdateExpenseAsync(id, model);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");

            return response;
        }
        catch (ArgumentOutOfRangeException)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            var error = req.CreateResponse(HttpStatusCode.InternalServerError);
            await error.WriteAsJsonAsync(ex);

            return error;
        }
    }

    private async Task<ExpenseDetailsModel?> ParseRequestAsync(HttpRequestData req)
    {
        using var reader = new StreamReader(req.Body);
        var requestBody = await reader.ReadToEndAsync();

        var model = JsonSerializer.Deserialize<ExpenseDetailsModel>(requestBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return model;
    }
}
