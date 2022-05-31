namespace ExpenseTracker.Data.Models;

public class Expense
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateTime ExpenseDate { get; set; } = DateTime.Today;

    public decimal TotalAmount { get; set; } = 0;

    public string UserId { get; set; } = string.Empty;

    public ICollection<ExpenseItem> Items { get; set; } = new HashSet<ExpenseItem>();

    public record ExpenseItem(string Name);
}
