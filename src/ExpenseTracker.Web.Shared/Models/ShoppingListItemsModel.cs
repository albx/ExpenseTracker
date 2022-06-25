namespace ExpenseTracker.Web.Shared.Models;

public class ShoppingListItemsModel
{
    public IEnumerable<ItemDescriptor> Items { get; set; } = Array.Empty<ItemDescriptor>();

    public class ItemDescriptor
    {
        public Guid Id { get; init; }

        public string Title { get; init; } = string.Empty;

        public int NumberOfItems { get; init; }
    }
}
