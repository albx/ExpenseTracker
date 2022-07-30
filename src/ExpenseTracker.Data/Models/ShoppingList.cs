namespace ExpenseTracker.Data.Models;

public class ShoppingList
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public ICollection<ShoppingListItem> Items { get; set; } = new HashSet<ShoppingListItem>();

    public record ShoppingListItem(string Name, int? Quantity, bool IsAcquired);
}
