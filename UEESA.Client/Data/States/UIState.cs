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

            internal List<PageContext> PageContexts;

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
                    Task.Run(async () =>
                    {
                        IsCurrentContextPageSet = true;
                        if (!HasSiteBeenRendered)
                        {
                            new Action(async () =>
                            {
                                await Task.Delay(TimeSpan.FromSeconds(Services.Get<JSInterface.AnimationManager>().Time_NavigationBarSlideIn));
                                await Services.Get<JSInterface.AnimationManager>().SlideInNavigationBar();
                                await Task.Delay(TimeSpan.FromSeconds(Services.Get<JSInterface.AnimationManager>().Time_NavigationBarSlideIn));
                                await Services.Get<JSInterface.AnimationManager>().SlideInToolsBar();
                                await Task.Delay(TimeSpan.FromSeconds(Services.Get<JSInterface.AnimationManager>().Time_ToolsBarSlideIn));
                            }).Invoke();
                        }
                        OnPagePreTransition?.Invoke();
                        await Services.Get<JSInterface.Utilities>().SetTitle("UEESA - " + value.FormalPageName);
                        if (HasSiteBeenRendered)
                        {
                            await Services.Get<ComponentState>().UpdateSettingsPanelState(true);
                            IsPageTransitioning = true;
                        }
                        OnPageTransitionStart?.Invoke();
                        List<Task> startStageTasks = new();
                        startStageTasks.Add(Services.Get<JSInterface.AnimationManager>().FadeInOutBackground(false));
                        startStageTasks.Add(Services.Get<JSInterface.AnimationManager>().FadePageInOut(false));
                        startStageTasks.Add(Task.Delay(TimeSpan.FromSeconds(Services.Get<JSInterface.AnimationManager>().Time_PageFade)));
                        await Task.WhenAll(startStageTasks);
                        Services.Get<NavigationManager>().NavigateTo(value.InformalPageName);
                        OnPageTransitionBackgroundStage?.Invoke();
                        if (!HasSiteBeenRendered) await Task.Delay(TimeSpan.FromSeconds(Services.Get<JSInterface.AnimationManager>().Time_NavigationBarTickerSlide));
                        List<Task> pageComponents = new();
                        OnPageTransitionEnd?.Invoke();
                        await Task.WhenAll(pageComponents);
                        IsPageTransitioning = false;
                        List<Task> endStageTasks = new();
                        endStageTasks.Add(Services.Get<JSInterface.AnimationManager>().FadeInOutBackground(true));
                        endStageTasks.Add(Services.Get<JSInterface.AnimationManager>().FadePageInOut(true));
                        await Task.WhenAll(endStageTasks);
                        OnPagePostTransition?.Invoke();
                        HasSiteBeenRendered = true;
                    });
                }
            }
        }

        internal class ComponentState
        {
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
        }
    }
}
