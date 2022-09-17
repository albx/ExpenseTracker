using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Web.Client.Data;

public class ShoppingListOfflineModel
{
    public Guid Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public DateTime LastModifiedDate { get; set; }

    public List<ShoppingListItemModel> Items { get; set; } = new();

    public class ShoppingListItemModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public int? Quantity { get; set; }

        public bool IsAcquired { get; set; }
    }
}
