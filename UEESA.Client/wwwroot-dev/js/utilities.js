GLOBAL.JSUtilities = null
GLOBAL.utilities = (ref) =>
{
    if (GLOBAL.JSUtilities === null) GLOBAL.JSUtilities = ref
}

(() =>
{
    window.utilities = {
        setTitle: (title) => document.title = title,
        setVideoSpeed: (elementID, speed) => document.getElementById(elementID).playbackRate = parseFloat(speed)
    }
})()