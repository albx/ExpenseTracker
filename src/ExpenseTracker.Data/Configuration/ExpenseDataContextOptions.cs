namespace ExpenseTracker.Data.Configuration;

public class ExpenseDataContextOptions
{
    public string ConnectionString { get; set; } = string.Empty;

    public string TableName { get; set; } = string.Empty;
}
