using ExpenseTracker.Web.Shared.Models;

namespace ExpenseTracker.Web.Client.Services;

public class ExpensesService
{
    public Task AddNewExpenseAsync(NewExpenseViewModel model)
    {
        var expense = new ExpensesListViewModel.ExpenseListItemViewModel
        {
            Id = Guid.NewGuid(),
            Title = model.Title,
            TotalAmount = model.TotalAmount,
            ExpenseDate = model.ExpenseDate
        };

        Expensens.Items.Add(expense);
        return Task.CompletedTask;
    }

    public Task<ExpensesListViewModel> GetExpensesAsync()
    {
        var model = new ExpensesListViewModel { Items = Expensens.Items.ToArray() };
        return Task.FromResult(model);
    }
}

internal static class Expensens
{
    internal static List<ExpensesListViewModel.ExpenseListItemViewModel> Items { get; } = new();
}
