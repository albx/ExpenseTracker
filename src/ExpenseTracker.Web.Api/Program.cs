using ExpenseTracker.Data;
using ExpenseTracker.Web.Api.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddExpenseDataContext(
            expenses => expenses.Configure(context.Configuration["StorageConnectionString"], context.Configuration["ExpensesTableName"]),
            shoppingList => shoppingList.Configure(context.Configuration["StorageConnectionString"], context.Configuration["ShoppingListTableName"]));

        services
            .AddScoped<ExpensesApiService>()
            .AddScoped<ShoppingListApiService>();
    })
    .Build();

host.Run();