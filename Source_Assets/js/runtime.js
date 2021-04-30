GLOBAL.JSRuntime = null;
GLOBAL.runtime = function (ref) 
{
    if (GLOBAL.JSRuntime === null)
    {
        GLOBAL.JSRuntime = ref;
    } 
};

(function () {

    var index = 0;
    var resources = [];

    var webglalerted = false;

    window.runtime = {

        load: (relativePath) => 
        {
            function createResource(type, location, url) {
                var obj = document.createElement(type);
        
                if (type === 'script') {
                    if (url) obj.src = url;
                } else {
                    obj.href = url;
                    obj.rel = 'stylesheet';
                }
                obj.location = location;
                obj.onload = indexCount;
                return obj;
            }
        
            function sendScript(index) {
                return resources[index].location.appendChild(resources[index]);
            }
        
            function indexCount() {
                if (index < resources.length) {
                    sendScript(index);
                    index++;
                }
            }

            resources.push(createResource('script', document.body, relativePath)); 
            indexCount();
        },
        isScriptLoaded: (relativePath) => 
        {
            for (var i = 0; i < resources.length - 1; i++) 
            {
                if (resources[i].src == relativePath)
                {
                    return true;
                }
            }
            return false;
        },
        loadWebGL2: () => 
        {
            var canv = document.getElementById("stargazer-canvas");
            var gl = canv.getContext("webgl2");
            if (gl) {
                if (!webglalerted) {
                    webglalerted = true;
                    alert("This is a WebGL2 testing placeholder. If you see a black screen, its_working.gif!");
                }

                gl.viewportWidth = canv.width;
                gl.viewportHeight = canv.height;
                window.addEventListener("resize", () => {
                    resize(gl, true);
                });
                gl.clearColor(0.0, 0.0, 0.0, 1.0);
                gl.clear(gl.COLOR_BUFFER_BIT);
            }
        }
    }

})();