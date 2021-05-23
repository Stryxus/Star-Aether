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

        fadeOutPage: (timeSecs) => 
        {
            TweenMax.to("#webpage", timeSecs, { opacity: 0, ease: "power3.out" });
        },
        fadeInPage: (timeSecs, bypass) => 
        {
            if (!bypass) 
            {
                TweenMax.to("#webpage", timeSecs, { opacity: 1, ease: "power3.out" });
            }
            else 
            {
                document.getElementById("webpage").style.opacity = 1;
            }
        },
        fadeInBackground: (timeSecs) => 
        {
            TweenMax.to("#background", timeSecs, { opacity: 1, ease: "power3.out" });
        },
        slideInNavigationBar: (timeSecs) => 
        {
            var height = document.getElementById("main-nav").offsetHeight;
            document.getElementById("main-nav").style.bottom = -height;
            TweenMax.to("#main-nav", timeSecs, { bottom: 0, ease: "power3.out" });
        },
        slideInToolsBar: (timeSecs) => 
        {
            var width = document.getElementById("tools-sidebar").offsetWidth;
            document.getElementById("tools-sidebar").style.left = -width;
            TweenMax.to("#tools-sidebar", timeSecs, { left: 0, ease: "power3.out" });
        },
        slideInOutHeadlinesNavBarTicker: (timeSecs, slideIn) => 
        {
            var width = document.getElementById("headlines-navbar-ticker").offsetWidth;
            if (!slideIn) 
            {
                document.getElementById("headlines-navbar-ticker").style.right = -width;
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
                document.getElementById("econome-navbar-ticker").style.right = -width;
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
                document.getElementById("settings-panel").style.right = -width;
                TweenMax.to("#settings-panel", timeSecs, { right: 0, ease: "power3.out" });
            }
            else 
            {
                TweenMax.to("#settings-panel", timeSecs, { right: -width, ease: "power3.out" });
            }
        }
    }

})();