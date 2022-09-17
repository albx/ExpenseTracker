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
        var model = new ShoppingListItemsModel();

        try
        {
            model = await Client.GetFromJsonAsync<ShoppingListItemsModel>("/api/ShoppingList") ?? new();
        }
        catch (HttpRequestException)
        {
            //TODO loggare
        }

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
                }
            }

            var mergeModel = offlineItems.Select(i => new ShoppingListModel
            {
                Id = i.Id,
                Title = i.Title,
                LastModifiedDate = i.LastModifiedDate,
                Items = i.Items.Select(x => new ShoppingListModel.ShoppingListItemModel
                {
                    Name = x.Name,
                    IsAcquired = x.IsAcquired,
                    Quantity = x.Quantity
                }).ToList()
            });

            await Client.PostAsJsonAsync("/api/MergeShoppingList", mergeModel);

            model.Items = mergedItems;
        }
    }

    public async Task CreateShoppingListAsync(ShoppingListModel model)
    {
        model.Id = Guid.NewGuid();
        model.LastModifiedDate = DateTime.UtcNow;

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
            LastModifiedDate = model.LastModifiedDate,
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
            LastModifiedDate = offlineShoppingList.LastModifiedDate,
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
        model.LastModifiedDate = DateTime.UtcNow;
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
            LastModifiedDate = shoppingList.LastModifiedDate,
            Items = shoppingList.Items.Select(i => new ShoppingListOfflineModel.ShoppingListItemModel
            {
                IsAcquired = i.IsAcquired,
                Name = i.Name,
                Quantity = i.Quantity
            }).ToList()
        };

        await OfflineContext.UpdateItems(new List<ShoppingListOfflineModel> { model });
    }

    public async Task DeleteShoppingListAsync(ShoppingListItemsModel.ItemDescriptor shoppingList)
    {
        await RemoveOfflineAsync(shoppingList);

        var response = await Client.DeleteAsync($"/api/DeleteShoppingList/{shoppingList.Id}");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error deleting shopping list {shoppingList.Title}");
        }
    }

    private async Task RemoveOfflineAsync(ShoppingListItemsModel.ItemDescriptor shoppingList)
    {
        await OfflineContext.OpenIndexedDb();
        await OfflineContext.DeleteByKey(nameof(ShoppingListOfflineModel), shoppingList.Id);
    }
}
