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

        fadeOutPage: () => 
        {
            TweenMax.to("#webpage", 0.3, { opacity: 0, ease: "power3.out" });
        },
        fadeInPage: (bypass) => 
        {
            if (!bypass) 
            {
                TweenMax.to("#webpage", 0.3, { opacity: 1, ease: "power3.out" });
            }
            else 
            {
                document.getElementById("webpage").style.opacity = 1;
            }
        },
        fadeInBackground: () => 
        {
            TweenMax.to("#background", 1, { opacity: 1, ease: "power3.out" });
        },
        slideInNavigationBar: () => 
        {
            var height = document.getElementById("main-nav").offsetHeight;
            document.getElementById("main-nav").style.bottom = -height;
            TweenMax.to("#main-nav", 1, { bottom: 0, ease: "power3.out" });
        },
        slideInToolsBar: () => 
        {
            var width = document.getElementById("tools-sidebar").offsetWidth;
            document.getElementById("tools-sidebar").style.left = -width;
            TweenMax.to("#tools-sidebar", 1, { left: 0, ease: "power3.out" });
        },
        slideInOutHeadlinesNavBarTicker: (slideIn) => 
        {
            var width = document.getElementById("headlines-navbar-ticker").offsetWidth;
            if (!slideIn) 
            {
                document.getElementById("headlines-navbar-ticker").style.right = -width;
                TweenMax.to("#headlines-navbar-ticker", 1.5, { right: 0, ease: "power3.out" });
            }
            else 
            {
                TweenMax.to("#headlines-navbar-ticker", 1.5, { right: -width, ease: "power3.out" });
            }
        },
        slideInOutEonomeNavBarTicker: (slideIn) => 
        {
            var width = document.getElementById("econome-navbar-ticker").offsetWidth;
            if (!slideIn) 
            {
                document.getElementById("econome-navbar-ticker").style.right = -width;
                TweenMax.to("#econome-navbar-ticker", 1.5, { right: 0, ease: "power3.out" });
            }
            else 
            {
                TweenMax.to("#econome-navbar-ticker", 1.5, { right: -width, ease: "power3.out" });
            }
        },
        slideInOutSettingsPanel: (slideIn) => 
        {
            var width = document.getElementById("settings-panel").offsetWidth;
            if (!slideIn) 
            {
                document.getElementById("settings-panel").style.right = -width;
                TweenMax.to("#settings-panel", 0.5, { right: 0, ease: "power3.out" });
            }
            else 
            {
                TweenMax.to("#settings-panel", 0.5, { right: -width, ease: "power3.out" });
            }
        }
    }

})();