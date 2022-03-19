using ExpenseTracker.Web.Shared.Models;
using System.Net.Http.Json;

namespace ExpenseTracker.Web.Client.Services;

public class ExpensesService
{
    public ExpensesService(HttpClient client)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public HttpClient Client { get; }

    public async Task AddNewExpenseAsync(NewExpenseModel model)
    {
        var response = await Client.PostAsJsonAsync("api/CreateNewExpense", model);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error creating new expense");
        }
    }

    public async Task<ExpensesListModel> GetExpensesAsync()
    {
        var model = await Client.GetFromJsonAsync<ExpensesListModel>("api/ExpensesList");
        return model ?? new ExpensesListModel();
    }
}

internal static class Expensens
{
    internal static List<ExpensesListModel.ExpenseListItemModel> Items { get; } = new();
}
