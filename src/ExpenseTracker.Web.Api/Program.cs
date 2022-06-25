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
            options.ConnectionString = context.Configuration["StorageConnectionString"];
            options.TableName = context.Configuration["ExpensesTableName"];
        });

        services
            .AddScoped<ExpensesApiService>()
            .AddScoped<ShoppingListApiService>();
    })
    .Build();

host.Run();