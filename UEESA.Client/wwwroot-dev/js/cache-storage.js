GLOBAL.JSCacheStorageInterface = null
GLOBAL.cacheStorageInterface = (ref) =>
{
    if (GLOBAL.JSCacheStorageInterface === null) GLOBAL.JSCacheStorageInterface = ref
}

(() =>
{
    window.cacheStorageInterface = {
        clear: () => caches.keys().then(keys => { keys.forEach(key => caches.delete(key)) })
    }
})()