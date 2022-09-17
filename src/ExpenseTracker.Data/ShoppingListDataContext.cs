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
                [nameof(ShoppingList.LastModifiedDate)] = shoppingList.LastModifiedDate,
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

    public async Task AddRangeAsync(IEnumerable<ShoppingList> shoppingListItems)
    {
        var entities = shoppingListItems.Select(s => new TableEntity(s.UserId, s.Id.ToString())
        {
            [nameof(ShoppingList.Title)] = s.Title,
            [nameof(ShoppingList.UserId)] = s.UserId,
            [nameof(ShoppingList.LastModifiedDate)] = s.LastModifiedDate,
            [nameof(ShoppingList.Items)] = JsonSerializer.Serialize(s.Items),
        });

        foreach (var entity in entities)
        {
            await _client.AddEntityAsync(entity);
        }
    }

    public async Task UpdateRangeAsync(IEnumerable<ShoppingList> shoppingListItems)
    {
        var entities = shoppingListItems.Select(s => new TableEntity(s.UserId, s.Id.ToString())
        {
            [nameof(ShoppingList.Title)] = s.Title,
            [nameof(ShoppingList.UserId)] = s.UserId,
            [nameof(ShoppingList.LastModifiedDate)] = s.LastModifiedDate,
            [nameof(ShoppingList.Items)] = JsonSerializer.Serialize(s.Items),
        });

        foreach (var entity in entities)
        {
            await _client.UpsertEntityAsync(entity);
        }
    }

    public IEnumerable<ShoppingList> GetAll()
    {
        var entities = _client.Query<TableEntity>();
        return entities.Select(e => new ShoppingList
        {
            Id = Guid.Parse(e.RowKey),
            Title = e[nameof(ShoppingList.Title)]?.ToString() ?? string.Empty,
            LastModifiedDate = ((DateTimeOffset)e[nameof(ShoppingList.LastModifiedDate)]).UtcDateTime,
            UserId = e[nameof(ShoppingList.UserId)]?.ToString() ?? string.Empty,
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
            Title = entity[nameof(ShoppingList.Title)]?.ToString() ?? string.Empty,
            LastModifiedDate = DateTime.Parse(entity[nameof(ShoppingList.LastModifiedDate)].ToString()!),
            UserId = entity[nameof(ShoppingList.UserId)]?.ToString() ?? string.Empty,
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
