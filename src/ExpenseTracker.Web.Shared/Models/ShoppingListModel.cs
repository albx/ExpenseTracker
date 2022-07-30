using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Web.Shared.Models;

public class ShoppingListModel
{
    public Guid Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public List<ShoppingListItemModel> Items { get; set; } = new();

    public class ShoppingListItemModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public int? Quantity { get; set; }

        public bool IsAcquired { get; set; }
    }
}
