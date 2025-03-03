using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;
using AddressAppServer.Web.Security;
using AddressAppServer.Web.Services.Interfaces;

namespace AddressAppServer.Web.BaseClasses
{
    public abstract class ProtectedPageBase : CommonBase
    {
        [Inject]
        protected JWTAuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        [Inject]
        protected IAuthClient AuthClient { get; set; } = default!;

        protected ClaimsPrincipal? User { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            User = authState.User;

            if (!User.Identity.IsAuthenticated)
            {
                var refreshToken = await AuthenticationStateProvider.GetRefreshTokenAsync();
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    var result = await AuthClient.RefreshTokenAsync(refreshToken);
                    if (result.Success)
                    {
                        await AuthenticationStateProvider.MarkUserAsAuthenticated(result.Value.User, result.Value.Token, result.Value.RefreshToken);
                    }
                    else
                    {
                        NavigationManager.NavigateTo("/login", forceLoad: true);
                    }
                }
                else
                {
                    NavigationManager.NavigateTo("/login", forceLoad: true);
                }
            }
            else if (!IsUserAuthorized())
            {
                NavigationManager.NavigateTo("/unauthorized", forceLoad: true);
            }
        }

        protected virtual bool IsUserAuthorized() => true;
    }
}