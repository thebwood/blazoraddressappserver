using AddressAppServer.Web.BaseClasses;

namespace AddressAppServer.Web.Components.Pages.Admin
{ 
    public partial class PermissionsPage : ProtectedPageBase
    {
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!firstRender)
                return;
        }
    }
}
