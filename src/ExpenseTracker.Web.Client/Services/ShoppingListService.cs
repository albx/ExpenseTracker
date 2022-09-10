using ExpenseTracker.Web.Client.Data;
using ExpenseTracker.Web.Shared.Models;
using System.Net.Http.Json;

namespace ExpenseTracker.Web.Client.Services;

public class ShoppingListService
{
    public HttpClient Client { get; }
    public OfflineContext OfflineContext { get; }

    public ShoppingListService(HttpClient client, OfflineContext offlineContext)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
        OfflineContext = offlineContext ?? throw new ArgumentNullException(nameof(offlineContext));
    }

    public async Task<ShoppingListItemsModel> GetShoppingListItemsAsync()
    {
        var model = await Client.GetFromJsonAsync<ShoppingListItemsModel>("/api/ShoppingList") ?? new();
        await MergeWithOfflineAsync(model);

        return model;
    }

    private async Task MergeWithOfflineAsync(ShoppingListItemsModel model)
    {
        await OfflineContext.OpenIndexedDb();
        var offlineItems = await OfflineContext.GetAll<ShoppingListOfflineModel>();
        if (offlineItems.Any())
        {
            var mergedItems = model.Items.ToList();

            foreach (var shoppingList in offlineItems)
            {
                if (!mergedItems.Any(i => i.Id == shoppingList.Id))
                {
                    mergedItems.Add(new ShoppingListItemsModel.ItemDescriptor
                    {
                        Id = shoppingList.Id,
                        Title = shoppingList.Title,
                        NumberOfItems = shoppingList.Items.Count
                    });

                    //TODO l'elemento dovrebbe essere sincronizzato con una API
                }
            }

            model.Items = mergedItems;
        }
    }

    public async Task CreateShoppingListAsync(ShoppingListModel model)
    {
        model.Id = Guid.NewGuid();
        await CreateOfflineAsync(model);

        var response = await Client.PostAsJsonAsync("/api/CreateShoppingList", model);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error creating shopping list");
        }
    }

    private async Task CreateOfflineAsync(ShoppingListModel model)
    {
        await OfflineContext.OpenIndexedDb();
        var offlineModel = new ShoppingListOfflineModel
        {
            Id = model.Id,
            Title = model.Title,
            Items = model.Items.Select(i => new ShoppingListOfflineModel.ShoppingListItemModel
            {
                IsAcquired = i.IsAcquired,
                Name = i.Name,
                Quantity = i.Quantity
            }).ToList()
        };

        await OfflineContext.AddItems(new List<ShoppingListOfflineModel> { offlineModel });
    }

    public async Task<ShoppingListModel> GetShoppingListDetailAsync(Guid shoppingListId)
    {
        try
        {
            var model = await Client.GetFromJsonAsync<ShoppingListModel>($"/api/ShoppingListDetail/{shoppingListId}");
            if (model is null)
            {
                model = await LoadFromOfflineAsync(shoppingListId);
            }

            return model;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return await LoadFromOfflineAsync(shoppingListId);
        }
    }

    private async Task<ShoppingListModel> LoadFromOfflineAsync(Guid shoppingListId)
    {
        await OfflineContext.OpenIndexedDb();

        var offlineShoppingList = await OfflineContext.GetByKey<Guid, ShoppingListOfflineModel>(shoppingListId);
        if (offlineShoppingList is null)
        {
            throw new InvalidOperationException("model is null");
        }

        return new ShoppingListModel
        {
            Id = offlineShoppingList.Id,
            Title = offlineShoppingList.Title,
            Items = offlineShoppingList.Items.Select(i => new ShoppingListModel.ShoppingListItemModel
            {
                IsAcquired = i.IsAcquired,
                Name = i.Name,
                Quantity = i.Quantity
            }).ToList()
        };
    }

    public async Task EditShoppingListAsync(Guid shoppingListId, ShoppingListModel model)
    {
        await UpdateOfflineAsync(shoppingListId, model);

        var response = await Client.PutAsJsonAsync(
            $"/api/UpdateShoppingList/{shoppingListId}",
            model);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error updating shopping list");
        }
    }

    private async Task UpdateOfflineAsync(Guid shoppingListId, ShoppingListModel shoppingList)
    {
        await OfflineContext.OpenIndexedDb();

        var model = new ShoppingListOfflineModel
        {
            Id = shoppingListId,
            Title = shoppingList.Title,
            Items = shoppingList.Items.Select(i => new ShoppingListOfflineModel.ShoppingListItemModel
            {
                IsAcquired = i.IsAcquired,
                Name = i.Name,
                Quantity = i.Quantity
            }).ToList()
        };

        await OfflineContext.UpdateItems(new List<ShoppingListOfflineModel> { model });
    }
}
