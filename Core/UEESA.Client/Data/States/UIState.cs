using System;
using System.Collections.Generic;

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

        internal bool IsSettingsPanelVisible { get; private set; }
        internal async void ToggleSettingsPanelVisibility()
        {
            IsSettingsPanelVisible = !IsSettingsPanelVisible;
            if (IsSettingsPanelVisible) await Services.Get<JSInterface.AnimationManager>().SlideSettingsPanelInOut(true);
            else await Services.Get<JSInterface.AnimationManager>().SlideSettingsPanelInOut(false);
        }

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
