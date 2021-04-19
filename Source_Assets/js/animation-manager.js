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
            TweenMax.to("#loading-overlay", 1, { opacity: 0, onComplete: () => { document.getElementById("loading-overlay").style.display = "none"; } });
        },
        fadeOutPage: () => 
        {
            TweenMax.to("#webpage", 0.25, { opacity: 0 });
        },
        fadeInPage: (bypass) => 
        {
            if (!bypass) 
            {
                TweenMax.to("#webpage", 0.25, { opacity: 1 });
            }
            else 
            {
                document.getElementById("webpage").style.opacity = 1;
            }
        },
        slideSettingsPanelInOut: (slideIn) => 
        {
            var width = document.getElementById("settings-panel").offsetWidth;
            if (slideIn) 
            {
                document.getElementById("settings-panel").style.right = -width;
                TweenMax.to("#settings-panel", 0.3, { right: 0 });
            }
            else 
            {
                TweenMax.to("#settings-panel", 0.3, { right: -width });
            }
        }
    }

})();