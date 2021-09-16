using System;
using System.Threading.Tasks;
using System.Collections.Generic;

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
                        if (HasSiteBeenRendered) IsPageTransitioning = true;
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

        }
    }
}
