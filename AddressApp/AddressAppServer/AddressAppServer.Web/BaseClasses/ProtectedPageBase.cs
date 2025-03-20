using AddressAppServer.Web.Security;
using AddressAppServer.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            Logger.LogInformation("OnAfterRenderAsync called");

            if (AuthenticationStateProvider == null || AuthClient == null)
            {
                Logger.LogError("AuthenticationStateProvider or AuthClient is null");
                HandleAuthenticationFailure();
                return;
            }

            try
            {
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

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshToken) || userDto == null)
                {
                    Logger.LogWarning("Token, refresh token, or user DTO is null or empty");
                    HandleAuthenticationFailure();
                    return;
                }

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                {
                    Logger.LogWarning("Invalid JWT token");
                    HandleAuthenticationFailure();
                    return;
                }

                // TODO: Value is getting set to null.  Need to inveestigate why this is happening.
                //if (jwtToken.ValidTo <= DateTime.UtcNow)
                //{
                //    Logger.LogWarning("User has been logged out");
                //    HandleAuthenticationFailure();
                //}

                if (jwtToken.ValidTo < DateTime.UtcNow.AddMinutes(5))
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

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occurred during authentication");
                HandleAuthenticationFailure();
            }
        }

        private void HandleAuthenticationFailure()
        {
            Logger.LogInformation("Handling authentication failure");
            AuthenticationStateProvider?.MarkUserAsLoggedOut();
            NavigationManager.NavigateTo("/login", forceLoad: true);
        }

        protected virtual bool IsUserAuthorized() => true;
    }
}