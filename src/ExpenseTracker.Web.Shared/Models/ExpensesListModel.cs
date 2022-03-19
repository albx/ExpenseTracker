namespace ExpenseTracker.Web.Shared.Models;

public class ExpensesListModel
{
    public IEnumerable<ExpenseListItemModel> Items { get; set; } = Array.Empty<ExpenseListItemModel>();

    public record ExpenseListItemModel
    {
        public Guid Id { get; set; }

        public DateTime ExpenseDate { get; set; }

        public string Title { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }
    }
}
