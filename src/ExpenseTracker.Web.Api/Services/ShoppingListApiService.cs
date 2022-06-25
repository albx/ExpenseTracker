using ExpenseTracker.Web.Shared.Models;

namespace ExpenseTracker.Web.Api.Services;

public class ShoppingListApiService
{
    public Task CreateShoppingListAsync(ShoppingListModel shoppingList, string userId)
    {
        //TODO Agganciamo la persistenza
        ShoppingListStore.Items.Add(shoppingList);
        return Task.CompletedTask;
    }
}

internal static class ShoppingListStore
{
    public static List<ShoppingListModel> Items { get; set; } = new();
}
