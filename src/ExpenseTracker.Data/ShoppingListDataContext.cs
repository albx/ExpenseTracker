using Azure.Data.Tables;
using ExpenseTracker.Data.Configuration;
using ExpenseTracker.Data.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ExpenseTracker.Data;

public class ShoppingListDataContext
{
    private readonly TableClient _client;

    private readonly ShoppingListDataContextOptions _options;

    private readonly ILogger<ShoppingListDataContext> _logger;

    public ShoppingListDataContext(IOptions<ShoppingListDataContextOptions> options, ILogger<ShoppingListDataContext> logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _client = new TableClient(_options.ConnectionString, _options.TableName);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _client.CreateIfNotExists();
    }

    public async Task SaveAsync(ShoppingList shoppingList)
    {
        try
        {
            var entity = new TableEntity(shoppingList.UserId, shoppingList.Id.ToString())
            {
                [nameof(ShoppingList.Title)] = shoppingList.Title,
                [nameof(ShoppingList.UserId)] = shoppingList.UserId,
                [nameof(ShoppingList.Items)] = JsonSerializer.Serialize(shoppingList.Items),
            };

            await _client.UpsertEntityAsync(entity, TableUpdateMode.Replace);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving Expense with Id {ExpenseId}: {ErrorMessage}", shoppingList.Id, ex.Message);
            throw;
        }

    }

    public IEnumerable<ShoppingList> GetAll()
    {
        var entities = _client.Query<TableEntity>();
        return entities.Select(e => new ShoppingList
        {
            Id = Guid.Parse(e.RowKey),
            Title = e[nameof(Expense.Title)]?.ToString() ?? string.Empty,
            UserId = e[nameof(Expense.UserId)]?.ToString() ?? string.Empty,
            Items = JsonSerializer.Deserialize<ICollection<ShoppingList.ShoppingListItem>>(e[nameof(ShoppingList.Items)]?.ToString() ?? string.Empty) ?? new HashSet<ShoppingList.ShoppingListItem>()
        });
    }

    public ShoppingList? GetById(Guid id)
    {
        var entity = _client.Query<TableEntity>(e => e.RowKey == id.ToString())?.SingleOrDefault();
        if (entity is null)
        {
            return null;
        }

        return new ShoppingList
        {
            Id = id,
            Title = entity[nameof(Expense.Title)]?.ToString() ?? string.Empty,
            UserId = entity[nameof(Expense.UserId)]?.ToString() ?? string.Empty,
            Items = JsonSerializer.Deserialize<ICollection<ShoppingList.ShoppingListItem>>(entity[nameof(Expense.Items)]?.ToString() ?? string.Empty) ?? new HashSet<ShoppingList.ShoppingListItem>()
        };
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = _client.Query<TableEntity>(e => e.RowKey == id.ToString())?.SingleOrDefault();
        if (entity is not null)
        {
            var response = await _client.DeleteEntityAsync(entity.PartitionKey, entity.RowKey);
            if (response.IsError)
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
    }
}
