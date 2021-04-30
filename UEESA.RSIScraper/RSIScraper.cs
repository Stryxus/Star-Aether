using System;
using System.Threading.Tasks;

using CefSharp;
using CefSharp.OffScreen;

using HtmlAgilityPack;

namespace UEESA.RSIScraper
{
    public abstract class RSIScraper
    {
        protected HtmlDocument HTML = new();
        protected ChromiumWebBrowser Browser;
        private bool multiRunFix = false;

        protected RSIScraper(Uri rsiLink, TimeSpan pageLoadWait)
        {
            CefSettings settings = new()
            {
                MultiThreadedMessageLoop = true,
                IgnoreCertificateErrors = true
            };
            settings.CefCommandLineArgs.Add("disable-gpu");
            settings.CefCommandLineArgs.Add("disable-gpu-compositing");
            if (!Cef.IsInitialized) Cef.Initialize(settings);
            Browser = new(rsiLink.ToString());
            Browser.LoadingStateChanged += async (object sender, LoadingStateChangedEventArgs e) =>
            {
                if (!e.IsLoading && !multiRunFix)
                {
                    multiRunFix = true;
                    await Task.Delay(pageLoadWait);
                    await ParseData();
                }
            };
        }

        protected abstract Task ParseData();
        protected abstract Task UploadToDatabase();
    }
}
