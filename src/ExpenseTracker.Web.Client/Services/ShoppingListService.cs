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

    public async Task CreateShoppingListAsync(ShoppingListModel model)
    {
        var response = await Client.PostAsJsonAsync("/api/CreateShoppingList", model);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error creating shopping list");
        }
    }
}
