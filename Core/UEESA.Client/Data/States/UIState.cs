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
                ToggleForceNavTickersInvisible(false);
            }
        }
        internal bool FirstRender = true;

        internal event Action OnIsNavTickersForcedInvisibleChanged;
        internal bool IsNavTickersForcedInvisible { get; private set; }
        internal void ToggleForceNavTickersInvisible(bool? forceInvisible = null)
        {
            bool boolCache = IsNavTickersForcedInvisible;
            IsNavTickersForcedInvisible = forceInvisible != null ? (bool)forceInvisible : !IsNavTickersForcedInvisible;
            if (boolCache != IsNavTickersForcedInvisible) OnIsNavTickersForcedInvisibleChanged.Invoke();
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
