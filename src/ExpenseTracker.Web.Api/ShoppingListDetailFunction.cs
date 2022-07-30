using ExpenseTracker.Web.Api.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ExpenseTracker.Web.Api
{
    public class ShoppingListDetailFunction
    {
        private readonly ILogger _logger;

        public ShoppingListApiService Service { get; }

        public ShoppingListDetailFunction(ILoggerFactory loggerFactory, ShoppingListApiService service)
        {
            _logger = loggerFactory.CreateLogger<ShoppingListDetailFunction>();
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [Function("ShoppingListDetail")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ShoppingListDetail/{id}")] HttpRequestData req,
            [Required] Guid id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (id == Guid.Empty)
            {
                var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                return errorResponse;
            }

            var model = Service.GetShoppingListDetail(id);
            if (model is null)
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(model);

            return response;
        }
    }
}
