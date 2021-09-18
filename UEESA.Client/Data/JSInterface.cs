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
            public float Time_Fastest = 0.25F;
            public float Time_Fast = 0.5F;
            public float Time_Normal = 0.75F;
            public float Time_Slow = 1F;
            public float Time_Slowest = 1.25F;

            public async Task InOutPage(bool show) => await JSR.InvokeVoidAsync("animationInterface.inOutPage", Time_Fastest, show);
            public async Task InOutBackground(bool show) => await JSR.InvokeVoidAsync("animationInterface.inOutBackground", Time_Slow, show);
            public async Task InOutNavbar() => await JSR.InvokeVoidAsync("animationInterface.inNavbar", Time_Normal);
            public async Task InOutHeadlinesBar(bool show) => await JSR.InvokeVoidAsync("animationInterface.inOutHeadlinesBar", Time_Slow, show);
            public async Task InOutEconomeBar(bool show) => await JSR.InvokeVoidAsync("animationInterface.inOutEconomeBar", Time_Slow, show);

            // Profile Panel

            public async Task InOutProfilePanel(bool show) => await JSR.InvokeVoidAsync("animationInterface.inOutProfilePanel", Time_Fast, show);
            public async Task InOutProfilePanelDropdown(bool show, int id) => await JSR.InvokeVoidAsync("animationInterface.inOutProfilePanelDropdown", Time_Fast, show, id);

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
