namespace ExpenseTracker.Web.Client.Models;

public class ConnectionState
{
    public bool IsOnline { get; set; }

    internal static ConnectionState Default { get; } = new ConnectionState { IsOnline = true };
}
