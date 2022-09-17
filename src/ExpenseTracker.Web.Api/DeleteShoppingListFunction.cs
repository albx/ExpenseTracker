using ExpenseTracker.Web.Api.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ExpenseTracker.Web.Api
{
    public class DeleteShoppingListFunction
    {
        private readonly ILogger _logger;

        public DeleteShoppingListFunction(ILoggerFactory loggerFactory, ShoppingListApiService service)
        {
            _logger = loggerFactory.CreateLogger<DeleteShoppingListFunction>();
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public ShoppingListApiService Service { get; }

        [Function("DeleteShoppingList")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "DeleteShoppingList/{id}")] HttpRequestData req,
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
                await Service.DeleteShoppingListAsync(id);

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
