namespace ExpenseTracker.Data.Configuration;

public class ShoppingListDataContextOptions
{
    public string ConnectionString { get; set; } = string.Empty;

    public string TableName { get; set; } = string.Empty;

    public void Configure(string connectionString, string tableName)
    {
        ConnectionString = connectionString;
        TableName = tableName;
    }
}
