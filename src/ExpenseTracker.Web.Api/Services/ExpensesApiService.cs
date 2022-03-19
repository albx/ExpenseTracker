using ExpenseTracker.Web.Shared.Models;

namespace ExpenseTracker.Web.Api.Services
{
    public class ExpensesApiService
    {
        public Task CreateNewExpenseAsync(NewExpenseModel model, string userId)
        {
            var expense = new Expense(
                Guid.NewGuid(),
                model.Title,
                model.ExpenseDate,
                model.TotalAmount,
                model.Items.Select(i => new ExpenseItem(i.Name)));

            Expenses.Add(expense);
            return Task.CompletedTask;
        }

        public Task<ExpensesListModel> GetAllExpensesAsync()
        {
            var expenses = Expenses
                .Select(e => new ExpensesListModel.ExpenseListItemModel
                {
                    Id = e.Id,
                    Title = e.Title,
                    ExpenseDate = e.ExpenseDate,
                    TotalAmount = e.TotalAmount,
                });

            var model = new ExpensesListModel { Items = expenses };
            return Task.FromResult(model);
        }

        public static List<Expense> Expenses { get; } = new();
    }

    public record Expense(Guid Id, string Title, DateTime ExpenseDate, decimal TotalAmount, IEnumerable<ExpenseItem> Items);

    public record ExpenseItem(string Name);
}
