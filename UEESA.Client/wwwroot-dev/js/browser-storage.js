GLOBAL.JSLocalStorageInterface = null
GLOBAL.localStorageInterface = (ref) =>
{
    if (GLOBAL.JSLocalStorageInterface === null) GLOBAL.JSLocalStorageInterface = ref
}

(() =>
{
    window.localStorageInterface = {
        setData: (name, data) => window.localStorage.setItem(name, data),
        getData: (name) =>  (window.localStorage.getItem(name)),
        clear: () =>  window.localStorage.clear()
    }
})()