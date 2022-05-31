using ExpenseTracker.Data.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExpenseDataContext(this IServiceCollection services, Action<ExpenseDataContextOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddScoped<ExpensesDataContext>();

        return services;
    }
}
