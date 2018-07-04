var data = null;
var selectEvent = null;
var indexChangeEvent = null;
API.onServerEventTrigger.connect(function (name, args) {
    API.closeAllMenus();
    if (name == "CREATE_MENU") {
        if (selectEvent != null) {
            selectEvent.disconnect();
            selectEvent = null;
        }
        if (indexChangeEvent != null) {
            indexChangeEvent.disconnect();
            indexChangeEvent = null;
        }
        data = JSON.parse(args[0]);
        var menu = API.createMenu(data["Title"], data["SubTitle"], data["X"], data["Y"], data["Anchor"]);
        for (var i = 0; i < data["Items"].length; i++) {
            var item = data["Items"][i];
            var mitem = API.createMenuItem(item["LeftLabel"], item["Description"]);
            if (item["RightLabel"] != "") {
                mitem.SetRightLabel(item["RightLabel"]);
            }
            mitem.Enabled = item["Enabled"];
            menu.AddItem(mitem);
        }
        menu.Visible = true;

        selectEvent = menu.OnItemSelect.connect(function (sender, selectedItem, index) {
            if (data == null)
                return;
            var userString = "";
            if (data["Items"][index]["OpenUserInput"]) {
                userString = API.getUserInput(data["Items"][index]["UserInputDefaultText"], 100);
            }
            API.triggerServerEvent("MENU_SELECT_EVENT", data["Identifier"], data["Items"][index]["EventTrigger"], data["Items"][index]["EventString"], data["Items"][index]["EventInt"], userString)
        });
        if (data["OnIndexChangeEvent"]) {
            indexChangeEvent = menu.OnIndexChange.connect(function (sender, newIndex) {
                if (data = null)
                    return;
                API.triggerServerEvent("MENU_INDEX_CHANGE_EVENT", data["Identifier"], data["Items"][newIndex]["EventTrigger"], data["Items"][newIndex]["EventString"], data["Items"][newIndex]["EventInt"])
            });
        }
        if (data["OnMenuCloseEvent"]) {
            menu.OnMenuClose.connect(function (sender) {
                if (data = null)
                    return;
                API.triggerServerEvent("MENU_CLOSE_EVENT", data["Identifier"]);
            });
        }
        return;
    }
    if (name == "CLOSE_MENU") {
        if (selectEvent != null) {
            selectEvent.disconnect();
            selectEvent = null;
        }
        if (indexChangeEvent != null) {
            indexChangeEvent.disconnect();
            indexChangeEvent = null;
        }
        API.closeAllMenus();
    }

});