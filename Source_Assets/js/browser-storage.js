GLOBAL.JSLocalStorageInterface = null;
GLOBAL.localStorageInterface = function (ref) 
{
    if (GLOBAL.JSLocalStorageInterface === null)
    {
        GLOBAL.JSLocalStorageInterface = ref;
    } 
};

(function () {

    window.localStorageInterface = {

        setData: (name, data) => 
        {
            window.localStorage.setItem(name, data);
        },
        getData: (name) => 
        {
            return (window.localStorage.getItem(name));
        },
        clear: () => 
        {
            window.localStorage.clear();
        }
    }

})();