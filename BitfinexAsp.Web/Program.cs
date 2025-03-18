using BitfinexAsp.ApiClients;
using BitfinexAsp.ApiClients.Bitfinex.REST;
using BitfinexAsp.ApiClients.Connectors;
using BitfinexAsp.ApiClients.Connectors.Implementation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient();

builder.Services.AddSingleton<MainClient>();
builder.Services.AddSingleton<BitfinexClient>();
builder.Services.AddSingleton<ITestConnector, BitfinexConnector>();

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