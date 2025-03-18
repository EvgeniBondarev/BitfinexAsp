using BitfinexAsp.ApiClients;
using BitfinexAsp.ApiClients.Bitfinex.REST;
using BitfinexAsp.ApiClients.Connectors;
using BitfinexAsp.ApiClients.Connectors.Implementation;
using BitfinexAsp.Models;
using BitfinexAsp.Services.CryptoConverter;
using BitfinexAsp.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient();

builder.Services.AddSingleton<MainClient>();
builder.Services.AddSingleton<BitfinexClient>();
builder.Services.AddSingleton<ITestConnector, BitfinexConnector>();
builder.Services.AddSingleton<ICryptoConverterService, CryptoConverterService>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();