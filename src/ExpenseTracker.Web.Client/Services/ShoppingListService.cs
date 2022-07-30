using ExpenseTracker.Web.Shared.Models;
using System.Net.Http.Json;

namespace ExpenseTracker.Web.Client.Services;

public class ShoppingListService
{
    public HttpClient Client { get; }

    public ShoppingListService(HttpClient client)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<ShoppingListItemsModel> GetShoppingListItemsAsync()
    {
        var model = await Client.GetFromJsonAsync<ShoppingListItemsModel>("/api/ShoppingList");
        return model ?? new();
    }

    public async Task CreateShoppingListAsync(ShoppingListModel model)
    {
        var response = await Client.PostAsJsonAsync("/api/CreateShoppingList", model);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error creating shopping list");
        }
    }

    public async Task<ShoppingListModel> GetShoppingListDetailAsync(Guid shoppingListId)
    {
        var model = await Client.GetFromJsonAsync<ShoppingListModel>($"/api/ShoppingListDetail/{shoppingListId}");
        if (model is null)
        {
            throw new InvalidOperationException("model is null");
        }

        return model;
    }

    public async Task EditShoppingListAsync(Guid shoppingListId, ShoppingListModel model)
    {
        var response = await Client.PutAsJsonAsync(
            $"/api/UpdateShoppingList/{shoppingListId}",
            model);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error updating shopping list");
        }
    }
}
