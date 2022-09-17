using ExpenseTracker.Web.Api.Extensions;
using ExpenseTracker.Web.Api.Services;
using ExpenseTracker.Web.Shared.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ExpenseTracker.Web.Api
{
    public class MergeShoppingListFunction
    {
        private readonly ILogger _logger;

        public ShoppingListApiService Service { get; }

        public MergeShoppingListFunction(ILoggerFactory loggerFactory, ShoppingListApiService service)
        {
            _logger = loggerFactory.CreateLogger<MergeShoppingListFunction>();
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [Function("MergeShoppingList")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            var model = await req.ReadFromJsonAsync<IEnumerable<ShoppingListModel>>();
            if (model is null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            var userId = req.GetUserId();
            await Service.MergeShoppingListItemsAsync(model, userId);

            var response = req.CreateResponse(HttpStatusCode.NoContent);
            response.Headers.Add("Content-Type", "application/json");

            return response;
        }
    }
}
