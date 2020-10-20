GLOBAL.JSWebGLInterface = null;
GLOBAL.webGLInterface = function (ref) 
{
    if (GLOBAL.JSWebGLInterface === null)
    {
        GLOBAL.JSWebGLInterface = ref;
    } 
};

(function () {

    window.webGLInterface = {

        loadStarGazer: (dataURL, frameworkURL, codeURL) => 
        {
            createUnityInstance(document.querySelector("#unity-canvas"), 
            {
                dataUrl: dataURL,
                frameworkUrl: frameworkURL,
                codeUrl: codeURL,
                streamingAssetsUrl: "StreamingAssets",
                companyName: "Stryxus",
                productName: "Star Gazer",
                productVersion: "0.1",
            });
        }
    }

})();