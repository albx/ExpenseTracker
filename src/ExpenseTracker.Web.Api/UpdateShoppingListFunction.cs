using ExpenseTracker.Web.Api.Services;
using ExpenseTracker.Web.Shared.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ExpenseTracker.Web.Api
{
    public class UpdateShoppingListFunction
    {
        private readonly ILogger _logger;

        public ShoppingListApiService Service { get; }

        public UpdateShoppingListFunction(ILoggerFactory loggerFactory, ShoppingListApiService service)
        {
            _logger = loggerFactory.CreateLogger<UpdateShoppingListFunction>();
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [Function("UpdateShoppingList")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "UpdateShoppingList/{id}")] HttpRequestData req,
            [Required] Guid id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (id == Guid.Empty)
            {
                var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                return errorResponse;
            }

            var model = await req.ReadFromJsonAsync<ShoppingListModel>();
            if (model is null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            try
            {
                await Service.UpdateShoppingListAsync(id, model);

                var response = req.CreateResponse(HttpStatusCode.NoContent);
                return response;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogError(ex, "Shopping list {Id} not found: {ErrorMessage}", id, ex.Message);
                return req.CreateResponse(HttpStatusCode.NotFound);
            }
        }
    }
}
