GLOBAL.JSRuntime = null
GLOBAL.runtime = (ref) => { if (GLOBAL.JSRuntime === null) GLOBAL.JSRuntime = ref }
(() =>
{
    var index = 0
    var resources = []

    window.runtime = 
    {
        load: (relativePath) => 
        {
            function createResource(type, location, url) 
            {
                var obj = document.createElement(type)
        
                if (type === 'script') 
                {
                    if (url) obj.src = url
                } 
                else 
                {
                    obj.href = url
                    obj.rel = 'stylesheet'
                }
                obj.location = location
                obj.onload = indexCount
                return obj
            }
        
            function sendScript(index) 
            {
                return resources[index].location.appendChild(resources[index])
            }
        
            function indexCount() 
            {
                if (index < resources.length) 
                {
                    sendScript(index)
                    index++
                }
            }
            
            resources.push(createResource('script', document.body, relativePath)) 
            indexCount()
        },
        isScriptLoaded: (relativePath) => 
        {
            for (var i = 0 ; i < resources.length - 1; i++) 
            {
                if (resources[i].src == relativePath) return true
            }
            return false
        }
    }
})()