using ExpenseTracker.Web.Api.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ExpenseTracker.Web.Api
{
    public class DeleteExpenseFunction
    {
        private readonly ILogger _logger;

        public DeleteExpenseFunction(ILoggerFactory loggerFactory, ExpensesApiService service)
        {
            _logger = loggerFactory.CreateLogger<DeleteExpenseFunction>();
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public ExpensesApiService Service { get; }

        [Function("DeleteExpense")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "DeleteExpense/{id}")] HttpRequestData req,
            [Required] Guid id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (id == Guid.Empty)
            {
                var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                return errorResponse;
            }

            try
            {
                await Service.DeleteExpenseAsync(id);

                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/json");

                return response;
            }
            catch (Exception ex)
            {
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteAsJsonAsync(ex);

                return error;
            }
        }
    }
}
