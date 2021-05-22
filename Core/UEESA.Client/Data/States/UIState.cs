using System;
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
            }
        }
        internal bool FirstRender = true;

        internal event Action OnNavBarTickersVisibilityChange;
        internal bool IsNavBarTickersVisible { get; private set; }
        internal void SetNavBarTickerVisibility(bool isVisible)
        {
            bool boolCache = IsNavBarTickersVisible;
            IsNavBarTickersVisible = isVisible;
            if (boolCache != IsNavBarTickersVisible) OnNavBarTickersVisibilityChange.Invoke();
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
                    await Services.Get<JSInterface.AnimationManager>().SlideSettingsPanelInOut(false);
                    await Task.Delay(TimeSpan.FromSeconds(0.3));
                    OnIsSettingsPanelVisibleChange.Invoke();
                }).Invoke();
                else OnIsSettingsPanelVisibleChange.Invoke();
            }
        }
        internal void ToggleSettingsPanelVisibility() => IsSettingsPanelVisible = !IsSettingsPanelVisible;

        internal event Action OnUtilitiesTypeChanged;
        private UtilitiesBarType utilitiesType;
        internal UtilitiesBarType UtilitiesType
        {
            get
            {
                return utilitiesType;
            } 
            set 
            {
                utilitiesType = value;
                OnUtilitiesTypeChanged();
            }
        }

        internal enum UtilitiesBarType
        {
            SocialMedia,
            Roadmap
        }
    }
}
