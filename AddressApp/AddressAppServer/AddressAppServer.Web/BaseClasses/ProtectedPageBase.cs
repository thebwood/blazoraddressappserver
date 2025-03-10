using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;
using AddressAppServer.Web.Security;
using AddressAppServer.Web.Services.Interfaces;
using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.ClassLibrary.DTOs;
using AddressAppServer.Web.Services;
using System.IdentityModel.Tokens.Jwt;

namespace AddressAppServer.Web.BaseClasses
{
    public abstract class ProtectedPageBase : CommonBase
    {
        [Inject]
        protected AddressAuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        [Inject]
        protected IAuthClient AuthClient { get; set; } = default!;
        [Inject]
        protected IStorageService StorageService { get; set; } = default!;
        [Inject]
        protected ILogger<ProtectedPageBase> Logger { get; set; } = default!;

        protected ClaimsPrincipal? User { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            Logger.LogInformation("OnInitializedAsync called");

            if (AuthenticationStateProvider == null || AuthClient == null)
            {
                Logger.LogError("AuthenticationStateProvider or AuthClient is null");
                HandleAuthenticationFailure();
                return;
            }

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (authState == null)
            {
                Logger.LogError("AuthenticationState is null");
                HandleAuthenticationFailure();
                return;
            }
            User = authState.User;

            var refreshToken = await StorageService.GetRefreshTokenAsync();
            var token = await StorageService.GetAccessTokenAsync();
            var userDto = await StorageService.GetUserAsync();
            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(refreshToken) && userDto != null)
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken != null && jwtToken.ValidTo < DateTime.UtcNow.AddMinutes(5))
                {
                    var result = await AuthClient.RefreshTokenAsync(userDto, refreshToken);
                    if (result.Success)
                    {
                        await AuthenticationStateProvider.MarkUserAsAuthenticated(result.Value.User, result.Value.Token, result.Value.RefreshToken);
                    }
                    else
                    {
                        Logger.LogWarning("Failed to refresh token");
                        HandleAuthenticationFailure();
                    }
                }

                if(jwtToken.ValidTo > DateTime.UtcNow)
                {
                    Logger.LogWarning("User has been logged out");
                    HandleAuthenticationFailure();
                }
            }
            else
            {
                Logger.LogWarning("Token, refresh token, or user DTO is null or empty");
                HandleAuthenticationFailure();
            }
        }

        private void HandleAuthenticationFailure()
        {
            Logger.LogInformation("Handling authentication failure");
            // Log the user out and redirect to the login page
            AuthenticationStateProvider?.MarkUserAsLoggedOut();
            NavigationManager.NavigateTo("/login", forceLoad: true);
        }

        protected virtual bool IsUserAuthorized() => true;
    }
}