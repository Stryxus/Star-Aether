GLOBAL.JSUtilities = null;
GLOBAL.utilities = function (ref) 
{
    if (GLOBAL.JSUtilities === null) GLOBAL.JSUtilities = ref;
};

(function () {
    window.utilities = {
        setTitle: (title) => document.title = title
    }
})();