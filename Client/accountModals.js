var accountModalCefWindow = null;
var accountModalCamera = null;
API.onResourceStart.connect(function () {
    API.setCanOpenChat(false);
});
API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName == "showAccountModal") {
        if (accountModalCefWindow != null) {
            API.destroyCefBrowser(accountModalCefWindow);
            accountModalCefWindow = null;
        }
        if (accountModalCamera != null) {
            API.deleteCamera(accountModalCamera);
            accountModalCamera = null;
        }
        accountModalCamera = API.createCamera(args[1], new Vector3(0, 0, 86.82519));
        API.pointCameraAtPosition(accountModalCamera, args[2]);
        API.setActiveCamera(accountModalCamera);
        var res = API.getScreenResolutionMaintainRatio();
        accountModalCefWindow = API.createCefBrowser(res.Width, res.Height, false);
        API.setCefBrowserHeadless(accountModalCefWindow, true);
        API.waitUntilCefBrowserInit(accountModalCefWindow);
        API.loadPageCefBrowser(accountModalCefWindow, args[0]);
        API.waitUntilCefBrowserLoaded(accountModalCefWindow);
        API.setCefBrowserHeadless(accountModalCefWindow, false);
        API.showCursor(true);
    }
    if (eventName == "hideAccountModal") {
        if (accountModalCefWindow != null) {
            API.destroyCefBrowser(accountModalCefWindow);
            accountModalCefWindow = null;
        }
        if (accountModalCamera != null) {
            API.deleteCamera(accountModalCamera);
            accountModalCamera = null;
        }
        API.setActiveCamera(null);
        API.showCursor(false);
        API.setCanOpenChat(true);
    }
    if (eventName == "showBrowser") {
        if (accountModalCefWindow != null) {
            API.destroyCefBrowser(accountModalCefWindow);
            accountModalCefWindow = null;
        }
        var res = API.getScreenResolutionMaintainRatio();
        accountModalCefWindow = API.createCefBrowser(res.Width, res.Height, false);
        API.setCefBrowserHeadless(accountModalCefWindow, true);
        API.waitUntilCefBrowserInit(accountModalCefWindow);
        API.loadPageCefBrowser(accountModalCefWindow, args[0]);
        API.waitUntilCefBrowserLoaded(accountModalCefWindow);
        if (args.Length > 1) {
            if (args.Length > 2) {
                accountModalCefWindow.call(args[1], args[2]);
            }
            else {
                accountModalCefWindow.call(args[1]);
            }
        }
        API.setCefBrowserHeadless(accountModalCefWindow, false);
        API.showCursor(true);
    }
    if (eventName == "updateBrowser") {
        if (accountModalCefWindow == null)
            return;
        if (args.Length > 0) {
            if (args.Length > 1) {
                accountModalCefWindow.call(args[0], args[1]);
            }
            else {
                accountModalCefWindow.call(args[0]);
            }
        }
    }
});
function SendAccountModelDataToServer(arg1, arg2, arg3) {
    API.triggerServerEvent(arg1, arg2, arg3);
}
function TriggerServerEvent(eventName) {
    API.triggerServerEvent(eventName);
}
function TriggerServerEventWithArg(eventName, arg) {
    API.triggerServerEvent(eventName, arg);
}
