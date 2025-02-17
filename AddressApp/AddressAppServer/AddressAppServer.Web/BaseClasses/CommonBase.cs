using Microsoft.AspNetCore.Components;

namespace AddressAppServer.Web.BaseClasses
{
    public class CommonBase : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }


    }
}
