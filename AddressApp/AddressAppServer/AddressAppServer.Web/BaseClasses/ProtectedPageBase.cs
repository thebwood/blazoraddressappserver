using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;

namespace AddressAppServer.Web.BaseClasses
{
    public abstract class ProtectedPageBase : CommonBase
    {
        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        protected ClaimsPrincipal? User { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            User = authState.User;

            if (!User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo("/login", forceLoad: true);
            }
            else if (!IsUserAuthorized())
            {
                NavigationManager.NavigateTo("/unauthorized", forceLoad: true);
            }
        }

        protected virtual bool IsUserAuthorized() => true;
    }
}