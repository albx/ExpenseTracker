using ExpenseTracker.Data.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExpenseDataContext(
        this IServiceCollection services,
        Action<ExpenseDataContextOptions> configureExpensesOptions,
        Action<ShoppingListDataContextOptions> configureShoppingListOptions)
    {
        services.Configure(configureExpensesOptions);
        services.Configure(configureShoppingListOptions);
        services.AddScoped<ExpensesDataContext>();
        services.AddScoped<ShoppingListDataContext>();

        return services;
    }
}
