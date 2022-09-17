using AzureStaticWebApps.Blazor.Authentication;
using DnetIndexedDb;
using DnetIndexedDb.Fluent;
using DnetIndexedDb.Models;
using ExpenseTracker.Web.Client;
using ExpenseTracker.Web.Client.Data;
using ExpenseTracker.Web.Client.Models;
using ExpenseTracker.Web.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddStaticWebAppsAuthentication();
builder.Services.AddLocalization();

builder.Services.AddSingleton(ConnectionState.Default);

builder.Services.AddHttpClient<ExpensesService>(client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

builder.Services.AddHttpClient<ShoppingListService>(client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

builder.Services.AddIndexedDbDatabase<OfflineContext>(options =>
{
    var indexedDbDatabaseModel = new IndexedDbDatabaseModel()
        .WithName(nameof(OfflineContext))
        .WithVersion(1);

    indexedDbDatabaseModel.AddStore(nameof(ShoppingListOfflineModel))
        .WithKey(nameof(ShoppingListOfflineModel.Id))
        .AddIndex(nameof(ShoppingListOfflineModel.Title));

    options.UseDatabase(indexedDbDatabaseModel);
});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
