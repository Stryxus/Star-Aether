import * as variables from "./variables"
import * as animationManager from "./animation-manager"
import * as browserStorage from "./browser-storage"
import * as cacheStorage from "./cache-storage"
import * as runtime from "./runtime"
import * as utilities from "./utilities"

function requireAll(r) { r.keys().forEach(r); }
requireAll(require.context('../', true, /\.png$/));