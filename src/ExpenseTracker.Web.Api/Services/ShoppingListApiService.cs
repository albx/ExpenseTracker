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

    public ShoppingListItemsModel GetShoppingListItems()
    {
        //TODO recuperare i dati dalla persitenza
        var model = new ShoppingListItemsModel
        {
            Items = ShoppingListStore.Items.Select(s => new ShoppingListItemsModel.ItemDescriptor
            {
                Id = Guid.NewGuid(),
                NumberOfItems = s.Items.Count,
                Title = s.Title
            })
        };
        return model;
    }
}

internal static class ShoppingListStore
{
    public static List<ShoppingListModel> Items { get; set; } = new();
}
