GLOBAL.JSAnimationInterface = null
GLOBAL.animationInterface = (ref) =>
{
    if (GLOBAL.JSAnimationInterface === null) GLOBAL.JSAnimationInterface = ref
}

(() =>
{

    window.animationInterface = {
        fadeInOutPage: (timeSecs, fadeIn) => 
        {
            if (document.getElementById("page-body") !== null) 
            {
                if (!fadeIn) TweenMax.to("#page-body", timeSecs, { opacity: 0, yPercent: -5, ease: "power3.out" })
                else 
                {
                    TweenMax.to("#page-body", 0, { yPercent: 5 })
                    TweenMax.to("#page-body", timeSecs, { opacity: 1, yPercent: 0, ease: "power3.out" })
                }
            }
        },
        fadeInOutBackground: (timeSecs, fadeIn) => 
        {
            if (document.getElementById("background") !== null) 
            {
                if (!fadeIn) TweenMax.to("#background", timeSecs, { opacity: 0, ease: "power3.out" })
                else TweenMax.to("#background", timeSecs, { opacity: 1, ease: "power3.out" })
            }
        },
        slideInNavigationBar: (timeSecs) => 
        {
            if (document.getElementById("main-nav") !== null)
            {
                var height = document.getElementById("main-nav").offsetHeight
                TweenMax.to("#main-nav", 0, { bottom: -height })
                TweenMax.to("#main-nav", timeSecs, { bottom: 0, ease: "power3.out" })
            }
        },
        slideInToolsBar: (timeSecs) => 
        {
            if (document.getElementById("tools-sidebar") !== null)
            {
                var width = document.getElementById("tools-sidebar").offsetWidth
                TweenMax.to("#tools-sidebar", 0, { left: -width })
                TweenMax.to("#tools-sidebar", timeSecs, { left: 0, ease: "power3.out" })
            }
        },
        slideInOutHeadlinesNavBarTicker: (timeSecs, slideOut) => 
        {
            if (document.getElementById("headlines-navbar-ticker") !== null)
            {
                var width = document.getElementById("headlines-navbar-ticker").offsetWidth
                if (slideOut) 
                {
                    TweenMax.to("#headlines-navbar-ticker", 0, { right: -width })
                    TweenMax.to("#headlines-navbar-ticker", timeSecs, { right: 0, ease: "power3.out" })
                }
                else TweenMax.to("#headlines-navbar-ticker", timeSecs, { right: -width, ease: "power3.out" })
            }
        },
        slideInOutEonomeNavBarTicker: (timeSecs, slideOut) => 
        {
            if (document.getElementById("econome-navbar-ticker") !== null) 
            {
                var width = document.getElementById("econome-navbar-ticker").offsetWidth
                if (slideOut) 
                {
                    TweenMax.to("#econome-navbar-ticker", 0, { right: -width })
                    TweenMax.to("#econome-navbar-ticker", timeSecs, { right: 0, ease: "power3.out" })
                }
                else TweenMax.to("#econome-navbar-ticker", timeSecs, { right: -width, ease: "power3.out" })
            }
        },
        slideInOutProfilePanel : (timeSecs, slideOut) => 
        {
            if (document.getElementById("profile-panel") !== null) 
            {
                var height = document.getElementById("profile-panel").offsetHeight
                if (slideOut) 
                {
                    TweenMax.to("#profile-panel", timeSecs, { bottom: 114, ease: "power3.out" })
                    TweenMax.to("#profile-panel", timeSecs, { opacity: 1, ease: "power3.out" })
                }
                else 
                {
                    TweenMax.to("#profile-panel", timeSecs, { bottom: -height, ease: "power3.out" })
                    TweenMax.to("#profile-panel", timeSecs, { opacity: 0, ease: "power3.out" })
                }
            }
        },
        slideInOutProfileDropdownPanel : (timeSecs, slideOut, id) => 
        {
            if (document.getElementById(id) !== null) 
            {
                var parent = document.getElementById(id)
                var chevron = parent.getElementsByClassName('profile-panel-dropdown-button')[0].getElementsByTagName('i')[0];
                var content = parent.getElementsByClassName('profile-panel-dropdown-content')[0];
                if (slideOut) 
                {
                    content.style.display = 'block'
                    TweenMax.to(chevron, timeSecs, { rotate: 180, ease: "power3.out" })
                    TweenMax.to(content, timeSecs, { webkitClipPath: 'polygon(0px 0px, 100% 0px, 100% 100%, 0px 100%)', ease: "power3.out" })
                }
                else 
                {
                    TweenMax.to(chevron, timeSecs, { rotate: 0, ease: "power3.out" })
                    TweenMax.to(content, timeSecs, { webkitClipPath: 'polygon(0px 0px, 100% 0px, 100% 0px, 0px 0px)', ease: "power3.out", onComplete: () =>
                    {
                        content.style.display = 'none'
                    }})
                }
            }
        },
        scrollTo: (position) => 
        {
            TweenMax.to(document.getElementById("page-body").children[0].children[0], {duration: 1, ease: "power3.out", scrollTo: position})
        },
        scrollToElement: (elementID) => 
        {
            if (document.getElementById(elementID) !== null) 
            {
                TweenMax.to(document.getElementById("page-body").children[0].children[0], {duration: 1, ease: "power3.out", scrollTo: '#' + elementID})
            }
        }
    }
})()