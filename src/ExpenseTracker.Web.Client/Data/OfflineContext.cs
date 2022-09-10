using DnetIndexedDb;
using Microsoft.JSInterop;

namespace ExpenseTracker.Web.Client.Data;

public class OfflineContext : IndexedDbInterop
{
    public OfflineContext(IJSRuntime jsRuntime, IndexedDbOptions<OfflineContext> indexedDbDatabaseOptions)
        : base(jsRuntime, indexedDbDatabaseOptions)
    {
    }


}
