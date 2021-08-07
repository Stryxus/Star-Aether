using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

namespace UEESA.Client.Data.States
{
    internal class UIState
    {
        internal class PageState
        {
            internal bool HasSiteBeenRendered;
            internal bool IsPageTransitioning;

            internal event Action OnPagePreTransition;
            internal event Action OnPageTransitionStart;

            internal event Action OnPageTransitionBackgroundStage;

            internal event Action OnPageTransitionEnd;
            internal event Action OnPagePostTransition;

            internal event Action OnPageContextsSet;
            private List<PageContext> pageContexts;
            internal List<PageContext> PageContexts
            {
                get
                {
                    return pageContexts;
                }

                set
                {
                    pageContexts = value;
                    OnPageContextsSet?.Invoke();
                }
            }

            internal bool IsCurrentContextPageSet;
            private PageContext currentPageContext;
            internal PageContext CurrentPageContext
            {
                get 
                { 
                    return currentPageContext; 
                }

                set
                {
                    currentPageContext = value;
                    new Action(async () =>
                    {
                        IsCurrentContextPageSet = true;
                        OnPagePreTransition?.Invoke();
                        await Services.Get<JSInterface.Utilities>().SetTitle("UEESA - " + value.FormalPageName);
                        if (HasSiteBeenRendered)
                        {
                            await Services.Get<ComponentState>().UpdateSettingsPanelState(true);
                            IsPageTransitioning = true;
                        }
                        OnPageTransitionStart?.Invoke();
                        await Services.Get<JSInterface.AnimationManager>().FadeInOutBackground(false);
                        await Services.Get<JSInterface.AnimationManager>().FadePageInOut(false);
                        await Task.Delay(TimeSpan.FromSeconds(Services.Get<JSInterface.AnimationManager>().Time_PageFade));
                        Services.Get<NavigationManager>().NavigateTo(value.InformalPageName);
                        OnPageTransitionBackgroundStage?.Invoke();
                        await Services.Get<ComponentState>().UpdateNavBarTickerHealinesState();
                        await Services.Get<ComponentState>().UpdateNavBarTickerEconomeState();
                        OnPageTransitionEnd?.Invoke();
                        IsPageTransitioning = false;
                        await Services.Get<JSInterface.AnimationManager>().FadeInOutBackground(true);
                        await Services.Get<JSInterface.AnimationManager>().FadePageInOut(true);
                        OnPagePostTransition?.Invoke();
                        HasSiteBeenRendered = true;
                    }).Invoke();
                }
            }
        }

        internal class ComponentState
        {

            // Headlines Navigation Bar Ticker

            internal event Action OnNavBarTickerHeadlinesChanged;
            internal bool IsNavBarTickerHeadlinesVisible { private set; get; }

            internal async Task UpdateNavBarTickerHealinesState()
            {
                if (Services.Get<PageState>().CurrentPageContext.AllowsNavBarTickers && Services.Get<ClientState>().Settings.ShowHealinesTicker)
                {
                    IsNavBarTickerHeadlinesVisible = true;
                    OnNavBarTickerHeadlinesChanged?.Invoke();
                    await Services.Get<JSInterface.AnimationManager>().SlideInOutHeadlinesNavBarTicker(true);
                }
                else
                {
                    await Services.Get<JSInterface.AnimationManager>().SlideInOutHeadlinesNavBarTicker(false);
                    await Task.Delay(TimeSpan.FromSeconds(Services.Get<JSInterface.AnimationManager>().Time_NavigationBarTickerSlide));
                    IsNavBarTickerHeadlinesVisible = false;
                    OnNavBarTickerHeadlinesChanged?.Invoke();
                }
            }

            // Econome Navigation Bar Ticker

            internal event Action OnNavBarTickerEconomeChanged;
            internal bool IsNavBarTickerEconomeVisible { private set; get; }

            internal async Task UpdateNavBarTickerEconomeState()
            {
                if (Services.Get<PageState>().CurrentPageContext.AllowsNavBarTickers && Services.Get<ClientState>().Settings.ShowEconomeTicker)
                {
                    IsNavBarTickerEconomeVisible = true;
                    OnNavBarTickerEconomeChanged?.Invoke();
                    await Services.Get<JSInterface.AnimationManager>().SlideInOutEonomeNavBarTicker(true);
                }
                else
                {
                    await Services.Get<JSInterface.AnimationManager>().SlideInOutEonomeNavBarTicker(false);
                    await Task.Delay(TimeSpan.FromSeconds(Services.Get<JSInterface.AnimationManager>().Time_NavigationBarTickerSlide));
                    IsNavBarTickerEconomeVisible = false;
                    OnNavBarTickerEconomeChanged?.Invoke();
                }
            }

            // Settings Panel

            internal event Action OnSettingsPanelVisibilityChanged;
            internal bool IsSettingsPanelVisible { private set; get; }

            internal async Task UpdateSettingsPanelState(bool forceClose = false)
            {
                if (!Services.Get<PageState>().IsPageTransitioning)
                {
                    if (IsSettingsPanelVisible || forceClose)
                    {
                        await Services.Get<JSInterface.AnimationManager>().SlideInOutSettingsPanel(false);
                        await Task.Delay(TimeSpan.FromSeconds(Services.Get<JSInterface.AnimationManager>().Time_SettingsPanelSlide));
                        IsSettingsPanelVisible = false;
                        OnSettingsPanelVisibilityChanged?.Invoke();
                    }
                    else
                    {
                        IsSettingsPanelVisible = true;
                        OnSettingsPanelVisibilityChanged?.Invoke();
                        // For some reason a very slight delay is needed. Im not sure why, this issue needs to be solved and the delay removed.
                        await Task.Delay(TimeSpan.FromSeconds(0.005));
                        await Services.Get<JSInterface.AnimationManager>().SlideInOutSettingsPanel(true);
                    }
                }
            }

            // Tools Bar

            internal event Action OnToolsBarTypeChanged;
            private ToolsBarType toolsType;
            internal ToolsBarType ToolsType
            {
                get
                {
                    return toolsType;
                }
                set
                {
                    toolsType = value;
                    OnToolsBarTypeChanged.Invoke();
                }
            }

            internal enum ToolsBarType
            {
                Default,
                Roadmap
            }
        }
    }
}
