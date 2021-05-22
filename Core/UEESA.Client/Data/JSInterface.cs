using System;
using System.Threading.Tasks;

using Microsoft.JSInterop;

namespace UEESA.Client.Data
{
    public class JSInterface
    {
        public static IJSRuntime JSR { get; private set; }

        public JSInterface(IJSRuntime runtime) => JSR = runtime;

        public async Task InitializeInterface<T>(T tiedObject, string functionName) where T : class => await JSR.InvokeVoidAsync("GLOBAL." + functionName, DotNetObjectReference.Create(tiedObject));

        public class Utilities
        {
            public async Task SetTitle(string title) => await JSR.InvokeAsync<string>("utilities.setTitle", title);
        }

        public class Runtime
        {
            public async Task Load(string relativePath) => await JSR.InvokeAsync<string>("runtime.load", relativePath);
            public async Task<bool> IsScriptLoaded(string relativePath) => await JSR.InvokeAsync<bool>("runtime.isScriptLoaded", relativePath);
            public async Task LoadWebGL2() => await JSR.InvokeVoidAsync("runtime.loadWebGL2");
        }

        public class Cache
        {
            public async Task Clear() => await JSR.InvokeVoidAsync("cacheStorageInterface.clear");
        }

        public class LocalData
        {
            public async Task SetData(string name, string data) => await JSR.InvokeVoidAsync("localStorageInterface.setData", name, data);
            public async Task<string> GetData(string name) => await JSR.InvokeAsync<string>("localStorageInterface.getData", name);
            public async Task Clear() => await JSR.InvokeVoidAsync("localStorageInterface.clear");
        }

        public class AnimationManager
        {
            public async Task ShowRouter() => await JSR.InvokeVoidAsync("animationInterface.showRouter");
            public async Task FadePageOut() => await JSR.InvokeVoidAsync("animationInterface.fadeOutPage");
            public async Task FadePageIn(bool bypass) => await JSR.InvokeVoidAsync("animationInterface.fadeInPage", bypass);
            public async Task SlideSettingsPanelInOut(bool slideIn) => await JSR.InvokeVoidAsync("animationInterface.slideSettingsPanelInOut", slideIn);
        }
    }
}
