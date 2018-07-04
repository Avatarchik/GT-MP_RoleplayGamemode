/// <reference path="../types-gt-mp/index.d.ts" />
// Allow repeated events when holding down a key?
var noRepeatCheck = false;
var targetModels = [];
var lastKey = null
var isAdmin = false;
var keyBlock = true;
API.onServerEventTrigger.connect(function (eventName, args) {
    var player = API.getLocalPlayer();
    if (eventName === "GTMPVOICE") {
        API.voiceEnable(args[0], args[1], args[2], args[3], args[4], args[5]); 
        return;
    }
    if (eventName === "LIPSYNC") {
        API.playPlayerFacialAnimation(args[0], args[1], args[2]);
        return;
    }
    if (eventName == "CLIENT_WRAPPER") {
        switch (args[0]) {
            case "ShowLoadingPrompt":
                API.showLoadingPrompt(args[1], args[2]);
                break;
            case "HideLoadingPrompt":
                API.hideLoadingPrompt();
                break;
            case "FadeScreenOut":
                API.fadeScreenOut(args[1]);
                break;
            case "FadeScreenIn":
                API.fadeScreenIn(args[1]);
                break;
            case "ShowMissionPassedMessage":
                API.showMissionPassedMessage(args[1], args[2]);
                break;
            case "ShowMpMessageLarge":
                API.showMpMessageLarge(args[1], args[2], args[3]);
                break;
            case "ShowWeaponPurchasedMessage":
                API.showWeaponPurchasedMessage(args[1], args[2], args[3], args[4]);
                break;
            case "ShowRankupMessage":
                API.showRankupMessage(args[1], args[2], args[3], args[4]);
                break;
            case "ShowOldMessage":
                API.showOldMessage(args[1], args[2]);
                break;
            case "ShowColoredShard":
                API.showColoredShard(args[1], args[2], args[3], args[4], args[5]);
                break;
            case "DisplaySubtitle":
                API.displaySubtitle(args[1], args[2]);
                break;
            case "PlayScreenEffect":
                API.playScreenEffect(args[1], args[2], args[3]);
                break;
            case "StopScreenEffect":
                API.stopScreenEffect(args[1]);
                break;
            case "StopAllScreenEffects":
                API.stopAllScreenEffects();
                break;
            case "ShowSimpleMessage":
                API.showSimpleMessage(args[0], args[1], args[2]);
                break;
            case "ShowMidsizedMessage":
                API.showMidsizedMessage(args[0], args[1], args[2], args[3], args[4], args[5]);
                break;
            case "SetThermalVisionEnabled":
                API.setThermalVisionEnabled(args[0]);
                break;
            case "SetNightVisionEnabled":
                API.setNightVisionEnabled(args[0]);
                break;
            case "DisplayHelpText":
                API.displayHelpText(args[0], args[1], args[2], args[3]);
                break;
            case "ShowPlaneMessage":
                API.showPlaneMessage(args[0], args[1], args[2], args[3]);
                break;
        }
        return;
    }
    if (eventName == "REGISTER_INTERACTION_MODEL") {
        targetModels = JSON.parse(args[0]);
    }
    if (eventName == "ADMIN_MODE") {
        isAdmin = args[0];
    }
    if (eventName == "KEYBLOCK") {
        keyBlock = args[0];
    }
});

API.onKeyDown.connect(function (sender, e) {
    if (!noRepeatCheck && (lastKey == e.KeyCode))
        return;
    if (keyBlock) return;
    lastKey = e.KeyCode;

    var localplr = API.getLocalPlayer();

    switch (e.KeyCode) {
        case Keys.PageDown: API.triggerServerEvent("RADIO_TOGGLE_SPEAK"); break;
        case Keys.PageUp: API.triggerServerEvent("RADIO_MUTE");break;
        case Keys.End: API.triggerServerEvent("RADIO_NEXT"); break;
        case Keys.OemBackslash: API.triggerServerEvent("RADIO_PTT", true); break;
        case Keys.E:
            if (API.isPlayerInAnyVehicle(localplr)) {
                //API.triggerServerEvent("INTERACTION_IN_VEHICLE");
            } else {
                var startPoint: Vector3 = API.getEntityPosition(localplr);
                var aimPoint = API.getEntityFrontPosition(localplr);
                var endPoint = Vector3Lerp(startPoint, aimPoint, 4);
                var rayCast = API.createRaycast(startPoint, endPoint, -1, localplr);
                if (rayCast.didHitEntity) {
                    var model = API.getEntityModel(rayCast.hitEntity);
                    if (isAdmin && e.Alt) {
                        API.triggerServerEvent("INTERACTION_WITH_UNKNOWN_OBJECT", rayCast.hitEntity, API.getEntityModel(rayCast.hitEntity), API.getEntityPosition(rayCast.hitEntity), API.getEntityRotation(rayCast.hitEntity), API.getEntityType(rayCast.hitEntity));
                        return;
                    }
                    if (API.getEntityType(rayCast.hitEntity) == 1) {
                        API.triggerServerEvent("INTERACTION_WITH_VEHICLE", rayCast.hitEntity, API.getEntityModel(rayCast.hitEntity), API.getEntityPosition(rayCast.hitEntity), API.getEntityRotation(rayCast.hitEntity), API.getEntityType(rayCast.hitEntity));
                        return;
                    }
                    if (targetModels.indexOf(model) !== -1) {
                        API.triggerServerEvent("INTERACTION_WITH_OBJECT", rayCast.hitEntity, API.getEntityModel(rayCast.hitEntity), API.getEntityPosition(rayCast.hitEntity), API.getEntityRotation(rayCast.hitEntity), API.getEntityType(rayCast.hitEntity));
                        return;
                    }
                }
                API.triggerServerEvent("INTERACTION_ONFOOT");
            }
            break;
        case Keys.I:
            if (API.isPlayerInAnyVehicle(localplr)) {
                API.triggerServerEvent("INTERACTION_IN_VEHICLE");
            } else {
                API.triggerServerEvent("INTERACTION_ONFOOT_INVENTORY");
            }
            break;
    }
});


API.onKeyUp.connect(function (sender, e) {
    lastKey = null;
    if (keyBlock) return;
    switch (e.KeyCode) {
        case Keys.OemBackslash: 
            API.triggerServerEvent("RADIO_PTT", false); break;

    }
});

function Vector3Lerp(start, end, fraction) {
    return new Vector3(
        (start.X + (end.X - start.X) * fraction),
        (start.Y + (end.Y - start.Y) * fraction),
        (start.Z + (end.Z - start.Z) * fraction)
    );
}