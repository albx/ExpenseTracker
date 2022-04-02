using ExpenseTracker.Data;
using ExpenseTracker.Data.Models;
using ExpenseTracker.Web.Shared.Models;

namespace ExpenseTracker.Web.Api.Services
{
    public class ExpensesApiService
    {
        public ExpensesDataContext Context { get; }

        public ExpensesApiService(ExpensesDataContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task CreateNewExpenseAsync(NewExpenseModel model, string userId)
        {
            var expense = new Expense
            {
                Id = Guid.NewGuid(),
                ExpenseDate = model.ExpenseDate,
                Title = model.Title,
                TotalAmount = model.TotalAmount,
                UserId = userId,
                Items = model.Items.Where(i => i.IsAcquired).Select(i => new Expense.ExpenseItem(i.Name)).ToList()
            };

            await Context.SaveAsync(expense);
        }

        public Task<ExpensesListModel> GetAllExpensesAsync()
        {
            var expenses = Context.GetAll()
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

        public async Task UpdateExpenseAsync(Guid expenseId, ExpenseDetailsModel model)
        {
            var expense = Context.GetById(expenseId);
            if (expense is null)
            {
                throw new ArgumentOutOfRangeException(nameof(expenseId));
            }

            if (model.Title != expense.Title)
            {
                expense.Title = model.Title;
            }
            if (model.ExpenseDate != expense.ExpenseDate)
            {
                expense.ExpenseDate = model.ExpenseDate;
            }
            if (model.TotalAmount != expense.TotalAmount)
            {
                expense.TotalAmount = model.TotalAmount;
            }

            expense.Items = model.Items.Where(i => i.IsAcquired).Select(i => new Expense.ExpenseItem(i.Name)).ToList();

            await Context.SaveAsync(expense);
        }

        public Task<ExpenseDetailsModel?> GetExpenseDetailsAsync(Guid expenseId)
        {
            var expense = Context.GetById(expenseId);
            if (expense is null)
            {
                return Task.FromResult<ExpenseDetailsModel?>(null);
            }

            var model = new ExpenseDetailsModel
            {
                Id = expense.Id,
                Title = expense.Title,
                TotalAmount = expense.TotalAmount,
                ExpenseDate = expense.ExpenseDate,
                Items = expense.Items.Select(i => new ExpenseItemModel { Name = i.Name, IsAcquired = true }).ToList()
            };

            return Task.FromResult<ExpenseDetailsModel?>(model);
        }

        public async Task DeleteExpenseAsync(Guid expenseId)
        {
            await Context.DeleteAsync(expenseId);
        }
    }
}
