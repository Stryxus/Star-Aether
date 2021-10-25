GLOBAL.JSAnimationInterface = null
GLOBAL.animationInterface = (ref) => { if (GLOBAL.JSAnimationInterface === null) GLOBAL.JSAnimationInterface = ref }
(() =>
{
    window.animationInterface = 
    {
        inOutPage: (timeSecs, show) => 
        {
            var el
            if ((el = document.getElementById("page-body")) !== null) 
            {
                if (!show) TweenMax.to(el, timeSecs, { opacity: -2, yPercent: -5, scaleX: "80%", scaleY: "80%" })
                else TweenMax.to(el, timeSecs, { opacity: 1, yPercent: 0, scaleX: "100%", scaleY: "100%" })
            }
        },
        inOutBackground: (timeSecs, show) => 
        {
            var el
            if ((el = document.getElementById("background")) !== null) 
            {
                if (!show) TweenMax.to(el, timeSecs, { opacity: -2, scaleX: "100%", scaleY: "100%" })
                else 
                {
                    // Setting the initial value in TweenMax is needed because the node is constantly changing between img and video.
                    TweenMax.to(el, 0, { opacity: -2, scaleX: "100%", scaleY: "100%" })
                    TweenMax.to(el, timeSecs, { opacity: 1, scaleX: "120%", scaleY: "120%" })
                }
            }
        },
        inNavbar: (timeSecs) => 
        {
            var el
            if ((el = document.getElementById("main-nav")) !== null) TweenMax.to(el, timeSecs, { opacity: 1 })
        },
        inOutHeadlinesBar: (timeSecs, show) => 
        {
            var el
            if ((el = document.getElementById("headlines-navbar-ticker")) !== null)
            {
                var width = el.offsetWidth
                if (show) TweenMax.to(el, timeSecs, { right: 0, ease: "power3.out" })
                else TweenMax.to(el, timeSecs, { right: -width, ease: "power3.out" })
            }
        },
        inOutEconomeBar: (timeSecs, show) => 
        {
            var el
            if ((el = document.getElementById("econome-navbar-ticker")) !== null) 
            {
                var width = el.offsetWidth
                if (show) TweenMax.to(el, timeSecs, { right: 0, ease: "power3.out" })
                else TweenMax.to(el, timeSecs, { right: -width, ease: "power3.out" })
            }
        },
        inOutProfilePanel : (timeSecs, show) => 
        {
            var el
            if ((el = document.getElementById("profile-panel")) !== null) 
            {
                var height = el.offsetHeight
                if (show) 
                {
                    TweenMax.to(el, timeSecs, { bottom: 114, ease: "power3.out" })
                    TweenMax.to(el, timeSecs, { opacity: 1, ease: "power3.out" })
                }
                else 
                {
                    TweenMax.to(el, timeSecs, { bottom: -height, ease: "power3.out" })
                    TweenMax.to(el, timeSecs, { opacity: 0, ease: "power3.out" })
                }
            }
        },
        inOutProfilePanelDropdown : (timeSecs, show, id) => 
        {
            var el
            if ((el = document.getElementById(id)) !== null) 
            {
                var chevron = el.getElementsByClassName('profile-panel-dropdown-button')[0].getElementsByTagName('i')[0];
                var content = el.getElementsByClassName('profile-panel-dropdown-content')[0];
                if (show) 
                {
                    content.style.display = 'block'
                    TweenMax.to(chevron, timeSecs, { rotate: 180, ease: "power3.out" })
                    TweenMax.to(content, timeSecs, { webkitClipPath: 'polygon(0px 0px, 100% 0px, 100% 100%, 0px 100%)', ease: "power3.out" })
                }
                else 
                {
                    TweenMax.to(chevron, timeSecs, { rotate: 0, ease: "power3.out" })
                    TweenMax.to(content, timeSecs, { webkitClipPath: 'polygon(0px 0px, 100% 0px, 100% 0px, 0px 0px)', ease: "power3.out", onComplete: () => content.style.display = 'none' })
                }
            }
        },
        scrollTo: (position) => 
        {
            TweenMax.to(document.getElementById("page-body").children[0].children[0], {duration: 1, ease: "power3.out", scrollTo: position})
        },
        scrollToElement: (elementID) => 
        {
            if (document.getElementById(elementID) !== null) TweenMax.to(document.getElementById("page-body").children[0].children[0], {duration: 1, ease: "power3.out", scrollTo: '#' + elementID})
        }
    }
})()