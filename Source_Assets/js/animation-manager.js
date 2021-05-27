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

        fadeInOutPage: (timeSecs, fadeIn) => 
        {
            if (!fadeIn) 
            {
                TweenMax.to("#page-body", timeSecs, { opacity: 0, yPercent: -5, ease: "power3.out" });
            }
            else 
            {
                TweenMax.to("#page-body", 0, { yPercent: 5 });
                TweenMax.to("#page-body", timeSecs, { opacity: 1, yPercent: 0, ease: "power3.out" });
            }
        },
        fadeInOutBackground: (timeSecs, fadeIn) => 
        {
            if (!fadeIn) 
            {
                TweenMax.to("#background", timeSecs, { opacity: 0, ease: "power3.out" });
            }
            else 
            {
                TweenMax.to("#background", timeSecs, { opacity: 1, ease: "power3.out" });
            }
        },
        slideInNavigationBar: (timeSecs) => 
        {
            var height = document.getElementById("main-nav").offsetHeight;
            TweenMax.to("#main-nav", 0, { bottom: -height });
            TweenMax.to("#main-nav", timeSecs, { bottom: 0, ease: "power3.out" });
        },
        slideInToolsBar: (timeSecs) => 
        {
            var width = document.getElementById("tools-sidebar").offsetWidth;
            TweenMax.to("#tools-sidebar", 0, { left: -width });
            TweenMax.to("#tools-sidebar", timeSecs, { left: 0, ease: "power3.out" });
        },
        slideInOutHeadlinesNavBarTicker: (timeSecs, slideIn) => 
        {
            var width = document.getElementById("headlines-navbar-ticker").offsetWidth;
            if (!slideIn) 
            {
                TweenMax.to("#headlines-navbar-ticker", 0, { right: -width });
                TweenMax.to("#headlines-navbar-ticker", timeSecs, { right: 0, ease: "power3.out" });
            }
            else 
            {
                TweenMax.to("#headlines-navbar-ticker", timeSecs, { right: -width, ease: "power3.out" });
            }
        },
        slideInOutEonomeNavBarTicker: (timeSecs, slideIn) => 
        {
            var width = document.getElementById("econome-navbar-ticker").offsetWidth;
            if (!slideIn) 
            {
                TweenMax.to("#econome-navbar-ticker", 0, { right: -width });
                TweenMax.to("#econome-navbar-ticker", timeSecs, { right: 0, ease: "power3.out" });
            }
            else 
            {
                TweenMax.to("#econome-navbar-ticker", timeSecs, { right: -width, ease: "power3.out" });
            }
        },
        slideInOutSettingsPanel: (timeSecs, slideIn) => 
        {
            var width = document.getElementById("settings-panel").offsetWidth;
            if (!slideIn) 
            {
                TweenMax.to("#settings-panel", 0, { right: -width });
                TweenMax.to("#settings-panel", timeSecs, { right: 0, ease: "power3.out" });
            }
            else 
            {
                TweenMax.to("#settings-panel", timeSecs, { right: -width, ease: "power3.out" });
            }
        }
    }

})();