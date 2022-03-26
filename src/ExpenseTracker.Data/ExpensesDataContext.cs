using Azure.Data.Tables;
using ExpenseTracker.Data.Configuration;
using ExpenseTracker.Data.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ExpenseTracker.Data;

public class ExpensesDataContext
{
    private readonly TableClient _client;

    private readonly ExpenseDataContextOptions _options;

    private readonly ILogger<ExpensesDataContext> _logger;

    public ExpensesDataContext(IOptions<ExpenseDataContextOptions> options, ILogger<ExpensesDataContext> logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _client = new TableClient(_options.ConnectionString, _options.TableName);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _client.CreateIfNotExists();
    }

    public async Task SaveAsync(Expense expense)
    {
        try
        {
            var entity = new TableEntity(expense.ExpenseDate.ToString("o"), expense.Id.ToString())
            {
                [nameof(Expense.Title)] = expense.Title,
                [nameof(Expense.ExpenseDate)] = expense.ExpenseDate.ToUniversalTime(),
                [nameof(Expense.TotalAmount)] = expense.TotalAmount,
                [nameof(Expense.UserId)] = expense.UserId,
                [nameof(Expense.Items)] = JsonSerializer.Serialize(expense.Items),
            };

            await _client.UpsertEntityAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving Expense with Id {ExpenseId}: {ErrorMessage}", expense.Id, ex.Message);
            throw;
        }

    }

    public IEnumerable<Expense> GetAll()
    {
        var entities = _client.Query<TableEntity>();
        return entities.Select(e => new Expense
        {
            Id = Guid.Parse(e.RowKey),
            Title = e[nameof(Expense.Title)]?.ToString() ?? string.Empty,
            ExpenseDate = DateTime.Parse(e[nameof(Expense.ExpenseDate)]!.ToString()!),
            TotalAmount = decimal.Parse(e[nameof(Expense.TotalAmount)]!.ToString()!),
            UserId = e[nameof(Expense.UserId)]?.ToString() ?? string.Empty,
            Items = JsonSerializer.Deserialize<ICollection<Expense.ExpenseItem>>(e[nameof(Expense.Items)]?.ToString() ?? string.Empty) ?? new HashSet<Expense.ExpenseItem>()
        });
    }

    public Expense? GetById(Guid id)
    {
        var entity = _client.Query<TableEntity>(e => e.RowKey == id.ToString())?.SingleOrDefault();
        if (entity is null)
        {
            return null;
        }

        return new Expense
        {
            Id = id,
            Title = entity[nameof(Expense.Title)]?.ToString() ?? string.Empty,
            ExpenseDate = (DateTime)entity[nameof(Expense.ExpenseDate)],
            TotalAmount = (decimal)entity[nameof(Expense.TotalAmount)],
            UserId = entity[nameof(Expense.UserId)]?.ToString() ?? string.Empty,
            Items = JsonSerializer.Deserialize<ICollection<Expense.ExpenseItem>>(entity[nameof(Expense.Items)]?.ToString() ?? string.Empty) ?? new HashSet<Expense.ExpenseItem>()
        };
    }
}
