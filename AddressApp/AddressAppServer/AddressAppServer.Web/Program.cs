using Microsoft.AspNetCore.Authentication.JwtBearer;
using AddressAppServer.Web.Common;
using AddressAppServer.Web.Components;
using AddressAppServer.Web.Extensions;
using AddressAppServer.Web.Middlewares;
using MudBlazor.Services;
using Serilog;
using Microsoft.AspNetCore.Authorization;

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
string? authority = apiSettings.Authority;
string? audience = apiSettings.Audience;

if (string.IsNullOrEmpty(baseAddress))
{
    throw new ArgumentNullException(nameof(baseAddress), "Base address for API is not configured.");
}

if (string.IsNullOrEmpty(authority))
{
    throw new ArgumentNullException(nameof(authority), "Authority for API is not configured.");
}

if (string.IsNullOrEmpty(audience))
{
    throw new ArgumentNullException(nameof(audience), "Audience for API is not configured.");
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
    options.UseSecurityTokenValidators = true;
    options.Authority = authority;
    options.Audience = audience;
    options.RequireHttpsMetadata = false; // Allow HTTP for development
    // Configure other options as needed
});

builder.Services.AddAuthorization();

// Register BlazorAuthorizationMiddlewareResultHandler as scoped
builder.Services.AddScoped<IAuthorizationMiddlewareResultHandler, BlazorAuthorizationMiddlewareResultHandler>();

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSerilog();
});

// Configure CircuitOptions for detailed errors
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

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
