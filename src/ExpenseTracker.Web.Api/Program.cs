using ExpenseTracker.Data;
using ExpenseTracker.Web.Api.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddExpenseDataContext(options =>
        {
            options.ConnectionString = context.Configuration["AzureWebJobsStorage"];
            options.TableName = context.Configuration["ExpensesTableName"];
        });

        services.AddScoped<ExpensesApiService>();
    })
    .Build();

host.Run();