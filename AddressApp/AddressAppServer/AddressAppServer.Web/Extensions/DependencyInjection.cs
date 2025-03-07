using AddressAppServer.Web.Common;
using AddressAppServer.Web.Middlewares;
using AddressAppServer.Web.Security;
using AddressAppServer.Web.Services;
using AddressAppServer.Web.Services.Interfaces;
using AddressAppServer.Web.ViewModels.Addresses;
using AddressAppServer.Web.ViewModels.Admin;
using AddressAppServer.Web.ViewModels.Auth;
using AddressAppServer.Web.ViewModels.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using System.Text;

namespace AddressAppServer.Web.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            ApiSettings? apiSettings = configuration.GetSection("ApiSettings").Get<ApiSettings>();
            JwtSettings? jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

            if (apiSettings == null)
            {
                throw new ArgumentNullException(nameof(apiSettings), "API settings are not configured.");
            }

            if (jwtSettings == null)
            {
                throw new ArgumentNullException(nameof(jwtSettings), "JWT settings are not configured.");
            }
            services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiSettings.BaseUrl) });

            AsyncRetryPolicy<HttpResponseMessage>? retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError() // Handles 5xx and 408 errors
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt));

            // Add HttpClient with retry policy and exception handling
            services.AddHttpClient<IAuthClient, AuthClient>((serviceProvider, client) =>
            {
                ApiSettings? apiSettings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
                client.BaseAddress = new Uri(apiSettings.BaseUrl);

            })
                .AddPolicyHandler(retryPolicy); // Attach the retry policy


            services.AddHttpClient<IAddressClient, AddressClient>((serviceProvider, client) =>
            {
                ApiSettings? apiSettings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
                client.BaseAddress = new Uri(apiSettings.BaseUrl);

            })
                .AddPolicyHandler(retryPolicy); // Attach the retry policy

            services.AddHttpClient<IAdminClient, AdminClient>((serviceProvider, client) =>
            {
                ApiSettings? apiSettings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
                client.BaseAddress = new Uri(apiSettings.BaseUrl);

            })
                .AddPolicyHandler(retryPolicy); // Attach the retry policy

            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<AddressAuthenticationStateProvider>();
            services.AddScoped<AuthenticationStateProvider, AddressAuthenticationStateProvider>();
            services.AddTransient<ProtectedLocalStorage>();
            services.AddSingleton<UIStateViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<AddressesViewModel>();
            services.AddTransient<AddressDetailViewModel>();
            services.AddTransient<UsersViewModel>();
            services.AddTransient<LogoutViewModel>();
            services.AddSingleton<IAuthorizationMiddlewareResultHandler, BlazorAuthorizationMiddlewareResultHandler>();

            var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

            // Add authentication services
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.AccessDeniedPath = "/denied";
                options.LoginPath = "/login";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                options.Cookie.SameSite = SameSiteMode.Lax;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            services.AddAuthorization();

            services.AddCascadingAuthenticationState();

            return services;
        }
    }
}
