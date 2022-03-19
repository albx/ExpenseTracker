using ExpenseTracker.Web.Shared.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ExpenseTracker.Web.Api
{
    public class ExpensesListFunction
    {
        private readonly ILogger _logger;

        public ExpensesListFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ExpensesListFunction>();
        }

        [Function("ExpensesList")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            var model = GetExpenses();

            await response.WriteAsJsonAsync(model);

            return response;
        }

        private ExpensesListModel GetExpenses()
        {
            return new ExpensesListModel
            {
                Items = new[]
                {
                    new ExpensesListModel.ExpenseListItemModel { Id = Guid.NewGuid(), Title = "test", ExpenseDate = DateTime.Today, TotalAmount = 10 }
                }
            };
        }
    }
}
