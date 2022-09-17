const connectionState = (function () {
    let dotNet;

    const updateOnlineStatus = function () {
        dotNet.invokeMethodAsync("OnConnectionStatusChanged", navigator.onLine);
    };

    const initialize = function (dotNetObject) {
        dotNet = dotNetObject;

        window.addEventListener("online", updateOnlineStatus);
        window.addEventListener("offline", updateOnlineStatus);

        updateOnlineStatus(dotNetObject);
    };

    const dispose = function () {
        if (updateOnlineStatus != null) {
            window.removeEventListener("online", updateOnlineStatus);
            window.removeEventListener("offline", updateOnlineStatus);
        }
    };

    return { initialize, dispose };
}());

window.connectionState = connectionState; 