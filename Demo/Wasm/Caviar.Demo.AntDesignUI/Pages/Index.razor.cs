using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Caviar.Models.SystemData;
using Caviar.AntDesignPages.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Caviar.AntDesignPages.Shared;
using AntDesign;

namespace Caviar.Demo.AntDesignUI.Pages
{
    partial class Index
    {

        [CascadingParameter]
        public EventCallback LayoutStyleCallBack { get; set; }

        [Inject]
        HttpHelper Http { get; set; }

        [Inject]
        UserConfigHelper UserConfig { get; set; }

        [Inject]
        IJSRuntime JsRuntime { get; set; }

        CavDataTemplate CavData { get; set; }

        RenderFragment test;

        protected override Task OnInitializedAsync()
        {

            return base.OnInitializedAsync();
        }

        public async Task Test()
        {

            StateHasChanged();
        }


    }
}