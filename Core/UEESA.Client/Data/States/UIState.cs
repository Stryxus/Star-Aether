﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UEESA.Client.Data.States
{
    internal class UIState
    {
        private bool IsPageContextsSet = false;
        internal event Action OnPageContextsSet;
        private List<PageContext> pageContexts = new();
        internal List<PageContext> PageContexts
        {
            get
            {
                return pageContexts;
            }

            private set
            {
                pageContexts = value;
                OnPageContextsSet.Invoke();
            }
        }
        internal void SetPageContexts(List<PageContext> contexts)
        {
            if (!IsPageContextsSet)
            {
                IsPageContextsSet = true;
                PageContexts = contexts;
            }
            else Logger.LogError("Page Contexts have already been set!");
        }

        internal event Action OnPageChanged;
        private PageContext currentPage;
        internal PageContext CurrentPage 
        {
            get
            {
                return currentPage;
            }

            set
            {
                currentPage = value;
                OnPageChanged?.Invoke();
                new Action(async () => await Services.Get<JSInterface.Utilities>().SetTitle("UEESA - " + value.FormalPageName)).Invoke();
                IsHeadlinesNavBarTickerVisible = !value.ForceNavBarTickersInvisible;
                IsEconomeNavBarTickerVisible = !value.ForceNavBarTickersInvisible;
            }
        }
        internal bool FirstRender = true;

        internal event Action OnIsHeadlinesNavBarTickerVisibleChange;
        internal bool HasHeadlinesNavBarTickerInitialRendererd;
        private bool isHeadlinesNavBarTickerVisible;
        internal bool IsHeadlinesNavBarTickerVisible
        {
            get
            {
                return isHeadlinesNavBarTickerVisible;
            }
            set
            {
                if (isHeadlinesNavBarTickerVisible != value && Services.Get<ClientState>().Settings != null)
                {
                    isHeadlinesNavBarTickerVisible = value;
                    new Action(async () =>
                    {
                        if (!CurrentPage.ForceNavBarTickersInvisible && Services.Get<ClientState>().Settings.ShowHealinesTicker)
                        {
                            if (!HasHeadlinesNavBarTickerInitialRendererd)
                            {
                                HasHeadlinesNavBarTickerInitialRendererd = true;
                                await Task.Delay(TimeSpan.FromSeconds(Services.Get<JSInterface.AnimationManager>().Time_NavigationBarSlideIn));
                            }
                            OnIsHeadlinesNavBarTickerVisibleChange?.Invoke();
                            await Services.Get<JSInterface.AnimationManager>().SlideInOutHeadlinesNavBarTicker(true);
                        }
                        else
                        {
                            await Task.Delay(TimeSpan.FromSeconds(Services.Get<JSInterface.AnimationManager>().Time_PageFade));
                            await Services.Get<JSInterface.AnimationManager>().SlideInOutHeadlinesNavBarTicker(false);
                            await Task.Delay(TimeSpan.FromSeconds(Services.Get<JSInterface.AnimationManager>().Time_NavigationBarTickerSlide));
                            OnIsHeadlinesNavBarTickerVisibleChange?.Invoke();
                        }
                    }).Invoke();
                }
            }
        }

        internal event Action OnIsEconomeNavBarTickerVisibleChange;
        internal bool HasEconomeNavBarTickerInitialRendererd;
        private bool isEconomeNavBarTickerVisible;
        internal bool IsEconomeNavBarTickerVisible
        {
            get
            {
                return isEconomeNavBarTickerVisible;
            }
            set
            {
                if (isEconomeNavBarTickerVisible != value && Services.Get<ClientState>().Settings != null)
                {
                    isEconomeNavBarTickerVisible = value;
                    new Action(async () =>
                    {
                        if (!CurrentPage.ForceNavBarTickersInvisible && Services.Get<ClientState>().Settings.ShowEconomeTicker)
                        {
                            if (!HasEconomeNavBarTickerInitialRendererd)
                            {
                                HasEconomeNavBarTickerInitialRendererd = true;
                                await Task.Delay(TimeSpan.FromSeconds(Services.Get<JSInterface.AnimationManager>().Time_NavigationBarSlideIn));
                            }
                            OnIsEconomeNavBarTickerVisibleChange?.Invoke();
                            await Services.Get<JSInterface.AnimationManager>().SlideInOutEonomeNavBarTicker(true);
                        }
                        else
                        {
                            await Task.Delay(TimeSpan.FromSeconds(Services.Get<JSInterface.AnimationManager>().Time_PageFade));
                            await Services.Get<JSInterface.AnimationManager>().SlideInOutEonomeNavBarTicker(false);
                            await Task.Delay(TimeSpan.FromSeconds(Services.Get<JSInterface.AnimationManager>().Time_NavigationBarTickerSlide));
                            OnIsEconomeNavBarTickerVisibleChange?.Invoke();
                        }
                    }).Invoke();
                }
            }
        }

        internal event Action OnIsSettingsPanelVisibleChange;
        private bool isSettingPanelVisible;
        internal bool IsSettingsPanelVisible 
        {
            get
            {
                return isSettingPanelVisible;
            }
            private set
            {
                isSettingPanelVisible = value;
                if (!value) new Action(async () =>
                {
                    await Services.Get<JSInterface.AnimationManager>().SlideInOutSettingsPanel(false);
                    await Task.Delay(TimeSpan.FromSeconds(Services.Get<JSInterface.AnimationManager>().Time_SettingsPanelSlide));
                    OnIsSettingsPanelVisibleChange?.Invoke();
                }).Invoke();
                else OnIsSettingsPanelVisibleChange?.Invoke();
            }
        }
        internal void ToggleSettingsPanelVisibility() => IsSettingsPanelVisible = !IsSettingsPanelVisible;

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
