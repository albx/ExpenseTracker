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

    public async Task DeleteExpenseAsync(ExpensesListModel.ExpenseListItemModel model)
    {
        var response = await Client.DeleteAsync($"/api/DeleteExpense/{model.Id}");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error deleting expense {model.Title}");
        }
    }

    public async Task<ExpenseDetailsModel> GetExpenseDetailsAsync(Guid expenseId)
    {
        var model = await Client.GetFromJsonAsync<ExpenseDetailsModel>($"api/ExpenseDetails/{expenseId}");
        return model ?? new ExpenseDetailsModel();
    }

    public async Task UpdateExpenseAsync(Guid expenseId, ExpenseDetailsModel model)
    {
        var response = await Client.PutAsJsonAsync($"api/UpdateExpense/{expenseId}", model);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error updating expense {model.Title}");
        }
    }
}
