using AddressAppServer.Web.Common;
using AddressAppServer.Web.Components;
using AddressAppServer.Web.Extensions;
using MudBlazor.Services;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.AddMudServices();

ApiSettings? apiSettings = builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>();

if (apiSettings == null)
{
    throw new ArgumentNullException(nameof(apiSettings), "API settings are not configured.");
}

string? baseAddress = apiSettings.BaseUrl;

if (string.IsNullOrEmpty(baseAddress))
{
    throw new ArgumentNullException(nameof(baseAddress), "Base address for API is not configured.");
}

builder.Services.AddPresentation(baseAddress);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
