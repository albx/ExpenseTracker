using ExpenseTracker.Data;
using ExpenseTracker.Data.Models;
using ExpenseTracker.Web.Shared.Models;

namespace ExpenseTracker.Web.Api.Services;

public class ShoppingListApiService
{
    public ShoppingListDataContext Context { get; }

    public ShoppingListApiService(ShoppingListDataContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task CreateShoppingListAsync(ShoppingListModel shoppingList, string userId)
    {
        var model = new ShoppingList
        {
            Id = shoppingList.Id,
            Title = shoppingList.Title,
            LastModifiedDate = shoppingList.LastModifiedDate,
            UserId = userId,
            Items = shoppingList.Items.Select(i => new ShoppingList.ShoppingListItem(i.Name, i.Quantity, i.IsAcquired)).ToList()
        };

        await Context.SaveAsync(model);
    }

    public ShoppingListItemsModel GetShoppingListItems()
    {
        var items = Context.GetAll();

        var model = new ShoppingListItemsModel
        {
            Items = items.Select(i => new ShoppingListItemsModel.ItemDescriptor
            {
                Id = i.Id,
                Title = i.Title,
                NumberOfItems = i.Items.Count
            })
        };

        return model;
    }

    public ShoppingListModel? GetShoppingListDetail(Guid shoppingListId)
    {
        var shoppingList = Context.GetById(shoppingListId);
        if (shoppingList is null)
        {
            return null;
        }

        return new ShoppingListModel
        {
            Id = shoppingListId,
            Title = shoppingList.Title,
            LastModifiedDate = shoppingList.LastModifiedDate,
            Items = shoppingList?.Items.Select(i => new ShoppingListModel.ShoppingListItemModel
            {
                Name = i.Name,
                Quantity = i.Quantity,
                IsAcquired = i.IsAcquired,
            }).ToList() ?? new List<ShoppingListModel.ShoppingListItemModel>()
        };
    }

    public async Task UpdateShoppingListAsync(Guid shoppingListId, ShoppingListModel model)
    {
        var shoppingList = Context.GetById(shoppingListId);
        if (shoppingList is null)
        {
            throw new ArgumentOutOfRangeException(nameof(shoppingListId));
        }

        shoppingList.LastModifiedDate = model.LastModifiedDate;
        shoppingList.Title = model.Title;
        shoppingList.Items = model.Items
            .Select(i => new ShoppingList.ShoppingListItem(i.Name, i.Quantity, i.IsAcquired))
            .ToList();

        await Context.SaveAsync(shoppingList);
    }

    public async Task DeleteShoppingListAsync(Guid shoppingListId)
    {
        await Context.DeleteAsync(shoppingListId);
    }

    public async Task MergeShoppingListItemsAsync(IEnumerable<ShoppingListModel> shoppingListItems, string userId)
    {
        var items = Context.GetAll();
        var itemsMap = items.ToDictionary(i => i.Id);

        var itemsToAdd = new List<ShoppingList>();
        var itemsToUpdate = new List<ShoppingList>();

        foreach (var shoppingList in shoppingListItems)
        {
            if (!itemsMap.ContainsKey(shoppingList.Id))
            {
                itemsToAdd.Add(new ShoppingList
                {
                    Id = shoppingList.Id,
                    LastModifiedDate = shoppingList.LastModifiedDate,
                    Title = shoppingList.Title,
                    UserId = userId,
                    Items = shoppingList.Items.Select(i => new ShoppingList.ShoppingListItem(i.Name, i.Quantity, i.IsAcquired)).ToList()
                });
            }
            else if (itemsMap[shoppingList.Id].LastModifiedDate < shoppingList.LastModifiedDate)
            {
                var shoppingListToUpdate = itemsMap[shoppingList.Id];
                shoppingListToUpdate.LastModifiedDate = shoppingList.LastModifiedDate;
                shoppingListToUpdate.Title = shoppingList.Title;
                shoppingListToUpdate.Items = shoppingList.Items
                    .Select(i => new ShoppingList.ShoppingListItem(i.Name, i.Quantity, i.IsAcquired)).ToList();

                itemsToUpdate.Add(shoppingListToUpdate);
            }
        }

        if (itemsToAdd.Any())
        {
            await Context.AddRangeAsync(itemsToAdd);
        }
        if (itemsToUpdate.Any())
        {
            await Context.UpdateRangeAsync(itemsToUpdate);
        }
    }
}
