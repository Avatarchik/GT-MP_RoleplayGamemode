using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using Roleplay.Base;
using Roleplay.Server.Base;
using Roleplay.Server.Enums;
using Roleplay.Server.Extensions;
using Roleplay.Server.Managers;
using Roleplay.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Server.Controller
{
    public class SpawnController : RoleplayScript
    {
        public SpawnController()
        {
            CharacterEditorController.OnPlayerFinishedCharacterEditor += CharacterEditorController_OnPlayerFinishedCharacterEditor;
        }

        private void CharacterEditorController_OnPlayerFinishedCharacterEditor(Client client, Account e)
        {
            if (e.CurrentCharacter == null)
            {
                logger.Debug($"Player: {client.socialClubName} ({e.Id}) Character is Null");
            }

            CharacterController.ApplyCharacterClothing(client);

            // Spawn at last Position
            if(e.CurrentCharacter.Position == null || e.CurrentCharacter.Position == "" || e.CurrentCharacter.Position.FromJson<Vector3>().DistanceTo(new Vector3(0, 0, 0)) <= 2f)
            {
                AntiCheatController.TeleportPlayer(client, Constants.DefaultSpawnPosition, new Vector3(0, 0, Constants.DefaultSpawnRotation));
                client.position = Constants.DefaultSpawnPosition;
                client.rotation = new Vector3(0, 0, Constants.DefaultSpawnRotation);
                API.sendColoredNotificationToPlayer(client, "Deine Spawnposition war fehlerhaft, deshalb wurdest du an den Anfangspunkt zurückgebracht.",
                    (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_ORANGE);
                e.CurrentCharacter.Position = Constants.DefaultSpawnPosition.ToJson();
                e.CurrentCharacter.Rotation = Constants.DefaultSpawnRotation;
                return;
            }
            AntiCheatController.TeleportPlayer(client, e.CurrentCharacter.Position.FromJson<Vector3>(), new Vector3(0, 0, e.CurrentCharacter.Rotation));
            client.Account().IsSpawned = true;
            client.BlockInteractionKeys(false);
            DimensionManager.GoToNormalWorldDimension(client);
            InteractionController.UpdateInteractableObjectsForPlayer(client);
            if(client.Account().AdminLevel == AdminLevel.AdminLevel4)
            {
                client.AdminMode(true);
                API.sendColoredNotificationToPlayer(client, "AdminMode aktiviert", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_BLUE);
            }
        }
    }
}
