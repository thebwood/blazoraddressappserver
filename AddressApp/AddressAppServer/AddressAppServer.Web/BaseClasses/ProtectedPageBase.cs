using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;
using AddressAppServer.Web.Security;
using AddressAppServer.Web.Services.Interfaces;
using AddressAppServer.ClassLibrary.Common;
using AddressAppServer.ClassLibrary.DTOs;

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
            if (!string.IsNullOrEmpty(refreshToken))
            {
                UserDTO? user = await StorageService.GetUserAsync();
                Result<RefreshUserTokenResponseDTO>? result = await AuthClient.RefreshTokenAsync(user, refreshToken);
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
            else
            {
                Logger.LogWarning("Refresh token is null or empty");
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