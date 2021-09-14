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
            public async Task SetVideoSpeed(string elementID, float speed) => await JSR.InvokeAsync<string>("utilities.setVideoSpeed", elementID, speed);
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
            public float Time_PageFade = 0.2F;
            public float Time_BackgroundFade = 0.2F;
            public float Time_NavigationBarSlideIn = 0.5F;
            public float Time_ToolsBarSlideIn = 0.5F;
            public float Time_NavigationBarTickerSlide = 0.5F;
            public float Time_SettingsPanelSlide = 0.25F;

            public async Task FadePageInOut(bool fadeIn) => await JSR.InvokeVoidAsync("animationInterface.fadeInOutPage", Time_PageFade, fadeIn);
            public async Task FadeInOutBackground(bool fadeIn) => await JSR.InvokeVoidAsync("animationInterface.fadeInOutBackground", Time_BackgroundFade, fadeIn);
            public async Task SlideInNavigationBar() => await JSR.InvokeVoidAsync("animationInterface.slideInNavigationBar", Time_NavigationBarSlideIn);
            public async Task SlideInToolsBar() => await JSR.InvokeVoidAsync("animationInterface.slideInToolsBar", Time_ToolsBarSlideIn);
            public async Task SlideInOutHeadlinesNavBarTicker(bool slideOut) => await JSR.InvokeVoidAsync("animationInterface.slideInOutHeadlinesNavBarTicker", Time_NavigationBarTickerSlide, slideOut);
            public async Task SlideInOutEonomeNavBarTicker(bool slideOut) => await JSR.InvokeVoidAsync("animationInterface.slideInOutEonomeNavBarTicker", Time_NavigationBarTickerSlide, slideOut);
            public async Task SlideInOutSettingsPanel(bool slideOut) => await JSR.InvokeVoidAsync("animationInterface.slideInOutSettingsPanel", Time_SettingsPanelSlide, slideOut);

            // Universal
            public async Task ScrollTo(int position) => await JSR.InvokeVoidAsync("animationInterface.scrollTo", position);
            public async Task ScrollToElement(string elementID) => await JSR.InvokeVoidAsync("animationInterface.scrollToElement", elementID);
        }
    }

    public class BrowserDimensions
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
