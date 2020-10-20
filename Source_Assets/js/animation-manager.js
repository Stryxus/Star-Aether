GLOBAL.JSAnimationInterface = null;
GLOBAL.animationInterface = function (ref) 
{
    if (GLOBAL.JSAnimationInterface === null)
    {
        GLOBAL.JSAnimationInterface = ref;
    } 
};

(function () {

    window.animationInterface = {

        showRouter: () => 
        {
            document.getElementById("router").style.display = "block";
            document.getElementById("router").style.opacity = 1;
            TweenMax.to("#loading-overlay", 2, { opacity: 0, onComplete: () => { document.getElementById("loading-overlay").style.display = "none"; } });
        },
        fadeOutPage: () => 
        {
            TweenMax.to("#webpage", 0.4, { opacity: 0 });
        },
        fadeInPage: (bypass) => 
        {
            if (!bypass) 
            {
                TweenMax.to("#webpage", 0.4, { opacity: 1 });
            }
            else 
            {
                document.getElementById("webpage").style.opacity = 1;
            }
        },
        slideSettingsPanelInOut: (slideIn) => 
        {
            if (slideIn) 
            {
                document.getElementById("settings-panel").style.display = "block";
                TweenMax.to("#settings-panel", 0.4, { opacity: 1 });
            }
            else 
            {
                TweenMax.to("#settings-panel", 0.4, { opacity: 0, onComplete: () => 
                {
                    document.getElementById("settings-panel").style.display = "none";
                } });
            }
        }
    }

})();