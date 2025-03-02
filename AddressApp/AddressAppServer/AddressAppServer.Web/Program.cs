using AddressAppServer.Web.Common;
using AddressAppServer.Web.Components;
using AddressAppServer.Web.Extensions;
using AddressAppServer.Web.Middlewares;
using AddressAppServer.Web.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MudBlazor.Services;
using Serilog;

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

// Add authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = "http://localhost:5025"; // Use HTTP for development
    options.Audience = "http://localhost:5025"; // Replace with your actual audience
    options.RequireHttpsMetadata = false; // Allow HTTP for development
    // Configure other options as needed
});

builder.Services.AddAuthorization();

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSerilog();
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

// Create a scope to resolve JWTAuthenticationStateProvider and initialize it
using (var scope = app.Services.CreateScope())
{
    var authProvider = scope.ServiceProvider.GetRequiredService<JWTAuthenticationStateProvider>();
    await authProvider.InitializeAsync();
}

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

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
