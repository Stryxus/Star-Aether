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
            public float Time_PageFade = 0.3F;
            public float Time_BackgroundFade = 1F;
            public float Time_NavigationBarSlideIn = 1F;
            public float Time_ToolsBarSlideIn = 1F;
            public float Time_NavigationBarTickerSlide = 1.5F;
            public float Time_SettingsPanelSlide = 0.5F;

            public async Task FadePageOut() => await JSR.InvokeVoidAsync("animationInterface.fadeOutPage", Time_PageFade);
            public async Task FadePageIn(bool bypass) => await JSR.InvokeVoidAsync("animationInterface.fadeInPage", Time_PageFade, bypass);
            public async Task FadeInBackground() => await JSR.InvokeVoidAsync("animationInterface.fadeInBackground", Time_BackgroundFade);
            public async Task SlideInNavigationBar() => await JSR.InvokeVoidAsync("animationInterface.slideInNavigationBar", Time_NavigationBarSlideIn);
            public async Task SlideInToolsBar() => await JSR.InvokeVoidAsync("animationInterface.slideInToolsBar", Time_ToolsBarSlideIn);
            public async Task SlideInOutHeadlinesNavBarTicker(bool slideIn) => await JSR.InvokeVoidAsync("animationInterface.slideInOutHeadlinesNavBarTicker", Time_NavigationBarTickerSlide, slideIn);
            public async Task SlideInOutEonomeNavBarTicker(bool slideIn) => await JSR.InvokeVoidAsync("animationInterface.slideInOutEonomeNavBarTicker", Time_NavigationBarTickerSlide, slideIn);
            public async Task SlideInOutSettingsPanel(bool slideIn) => await JSR.InvokeVoidAsync("animationInterface.slideInOutSettingsPanel", Time_SettingsPanelSlide, slideIn);
        }
    }
}
