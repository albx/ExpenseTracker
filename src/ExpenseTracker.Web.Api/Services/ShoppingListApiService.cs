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

        shoppingList.Title = model.Title;
        shoppingList.Items = model.Items
            .Select(i => new ShoppingList.ShoppingListItem(i.Name, i.Quantity, i.IsAcquired))
            .ToList();

        await Context.SaveAsync(shoppingList);
    }
}
