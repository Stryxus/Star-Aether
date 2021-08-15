GLOBAL.JSCacheStorageInterface = null;
GLOBAL.cacheStorageInterface = function (ref) 
{
    if (GLOBAL.JSCacheStorageInterface === null) GLOBAL.JSCacheStorageInterface = ref;
};

(function () {
    window.cacheStorageInterface = {
        clear: () => caches.keys().then(keys => { keys.forEach(key => caches.delete(key)); })
    }
})();