using AddressAppServer.Web.Common;
using AddressAppServer.Web.Middlewares;
using AddressAppServer.Web.Security;
using AddressAppServer.Web.Services;
using AddressAppServer.Web.Services.Interfaces;
using AddressAppServer.Web.ViewModels.Addresses;
using AddressAppServer.Web.ViewModels.Admin;
using AddressAppServer.Web.ViewModels.Auth;
using AddressAppServer.Web.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace AddressAppServer.Web.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, string baseAddress)
        {
            services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });

            AsyncRetryPolicy<HttpResponseMessage>? retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError() // Handles 5xx and 408 errors
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt));

            // Add HttpClient with retry policy and exception handling
            services.AddHttpClient<IAuthClient, AuthClient>((serviceProvider, client) => {
                ApiSettings? apiSettings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
                client.BaseAddress = new Uri(baseAddress);

            })
                .AddPolicyHandler(retryPolicy); // Attach the retry policy


            services.AddHttpClient<IAddressClient, AddressClient>((serviceProvider, client) => {
                ApiSettings? apiSettings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
                client.BaseAddress = new Uri(baseAddress);

            })
                .AddPolicyHandler(retryPolicy); // Attach the retry policy

            services.AddHttpClient<IAdminClient, AdminClient>((serviceProvider, client) => {
                ApiSettings? apiSettings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
                client.BaseAddress = new Uri(baseAddress);

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

            services.AddCascadingAuthenticationState();

            return services;
        }
    }
}
