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
                                await Services.Get<JSInterface.AnimationManager>().InOutNavbar();
                            }).Invoke();
                        }
                        OnPagePreTransition?.Invoke();
                        await Services.Get<JSInterface.Utilities>().SetTitle("UEESA - " + value.FormalPageName);
                        if (HasSiteBeenRendered) IsPageTransitioning = true;
                        OnPageTransitionStart?.Invoke();
                        List<Task> startStageTasks = new();
                        startStageTasks.Add(Task.Delay(TimeSpan.FromSeconds(Services.Get<JSInterface.AnimationManager>().Time_Fastest)));
                        startStageTasks.Add(Services.Get<JSInterface.AnimationManager>().InOutBackground(false));
                        startStageTasks.Add(Services.Get<JSInterface.AnimationManager>().InOutPage(false));
                        await Task.WhenAll(startStageTasks);
                        Services.Get<NavigationManager>().NavigateTo(value.InformalPageName);
                        OnPageTransitionBackgroundStage?.Invoke();
                        List<Task> pageComponents = new();
                        OnPageTransitionEnd?.Invoke();
                        await Task.WhenAll(pageComponents);
                        IsPageTransitioning = false;
                        List<Task> endStageTasks = new();
                        endStageTasks.Add(Services.Get<JSInterface.AnimationManager>().InOutBackground(true));
                        endStageTasks.Add(Services.Get<JSInterface.AnimationManager>().InOutPage(true));
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
