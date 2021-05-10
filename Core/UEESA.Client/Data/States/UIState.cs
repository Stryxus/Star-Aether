using System;

namespace UEESA.Client.Data.States
{
    internal class UIState
    {
        internal event Action OnPageChanged;
        private string currentPage = "/";
        internal string CurrentPage 
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
