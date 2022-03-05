namespace ExpenseTracker.Web.Shared.Models;

public class ExpensesListViewModel
{
    public IEnumerable<ExpenseListItemViewModel> Items { get; set; } = Array.Empty<ExpenseListItemViewModel>();

    public record ExpenseListItemViewModel
    {
        public Guid Id { get; set; }

        public DateTime ExpenseDate { get; set; }

        public string Title { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }
    }
}
