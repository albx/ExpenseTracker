using ExpenseTracker.Web.Api.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ExpenseTracker.Web.Api
{
    public class ShoppingListFunction
    {
        private readonly ILogger _logger;

        public ShoppingListApiService Service { get; }

        public ShoppingListFunction(ILoggerFactory loggerFactory, ShoppingListApiService service)
        {
            _logger = loggerFactory.CreateLogger<ShoppingListFunction>();
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [Function("ShoppingList")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var model = Service.GetShoppingListItems();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(model);

            return response;
        }
    }
}
