﻿using AddressAppServer.Web.Common;
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
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Serilog;
using System.Security.Claims;
using System.Text;

namespace AddressAppServer.Web.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration, ConfigureHostBuilder host)
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
                client.BaseAddress = new Uri(apiSettings.BaseUrl);

            })
                .AddPolicyHandler(retryPolicy); // Attach the retry policy


            services.AddHttpClient<IAddressClient, AddressClient>((serviceProvider, client) =>
            {
                client.BaseAddress = new Uri(apiSettings.BaseUrl);

            })
                .AddPolicyHandler(retryPolicy); // Attach the retry policy

            services.AddHttpClient<IAdminClient, AdminClient>((serviceProvider, client) =>
            {
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
            .AddJwtBearer(options =>
            {
                options.UseSecurityTokenValidators = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    RoleClaimType = ClaimTypes.Role // Ensure roles are validated
                };
            });

            services.AddAuthorization();


            host.UseSerilog((context, services, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
            });

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog();
            });


            services.AddCascadingAuthenticationState();

            return services;
        }
    }
}
