﻿using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AddressAppServer.Web.BaseClasses
{
    public class CommonBase : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

    }
}
