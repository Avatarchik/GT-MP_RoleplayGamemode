using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;
using Roleplay.Base;
using Roleplay.Server.Base;
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
    public class CharacterEditorController : RoleplayScript
    {
        public static event EventHandler<Client, Account> OnPlayerFinishedCharacterEditor;

        public CharacterEditorController()
        {
            CharacterController.OnPlayerSelectCharacter += CharacterController_OnPlayerSelectCharacter;
            ClientEventManager.RegisterClientEvent("CharacterEditorSetGender", ClientSideSetGender);
            ClientEventManager.RegisterClientEvent("CharacterEditorSaveCharacter", ClientSideSaveCharacter);
            ClientEventManager.RegisterClientEvent("CharacterEditorLeave", ClientSideLeaveEditor);
        }

        #region ClientSideEvents
        private void ClientSideSetGender(Client client, string eventName, object[] arguments)
        {
            if(arguments.Length < 1) return;

            int gender = Convert.ToInt32(arguments[0]);
            client.setSkin((gender == 0) ? PedHash.FreemodeMale01 : PedHash.FreemodeFemale01);
            client.Account().CurrentCharacter.CharacterStyle.Gender = gender;
            client.setData("ChangedGender", true);
            SetDefaultFeatures(client, gender, true);
        }

        private void ClientSideSaveCharacter(Client player, string eventName, object[] args)
        {
            if (args.Length < 8) return;

            player.setDefaultClothes();
            var character = player.Account().CurrentCharacter;
            var chstyle = character.CharacterStyle;

            // General Data
            if (character.FirstName != null && character.FirstName != "")
            {
                character.FirstName = args[8].ToString();
                character.LastName = args[9].ToString();
            }

            // gender
            chstyle.Gender = Convert.ToInt32(args[0]);

            // parents
            chstyle.Parents.Father = Convert.ToInt32(args[1]);
            chstyle.Parents.Mother = Convert.ToInt32(args[2]);
            chstyle.Parents.Similarity = (float)Convert.ToDouble(args[3]);
            chstyle.Parents.SkinSimilarity = (float)Convert.ToDouble(args[4]);

            // features
            float[] feature_data = JsonConvert.DeserializeObject<float[]>(args[5].ToString());
            chstyle.Features = feature_data;

            // appearance
            AppearanceItem[] appearance_data = JsonConvert.DeserializeObject<AppearanceItem[]>(args[6].ToString());
            chstyle.Appearance = appearance_data;

            // hair & colors
            int[] hair_and_color_data = JsonConvert.DeserializeObject<int[]>(args[7].ToString());
            for (int i = 0; i < hair_and_color_data.Length; i++)
            {
                switch (i)
                {
                    // Hair
                    case 0:
                        {
                            chstyle.Hair.Hair = hair_and_color_data[i];
                            break;
                        }

                    // Hair Color
                    case 1:
                        {
                            chstyle.Hair.Color = hair_and_color_data[i];
                            break;
                        }

                    // Hair Highlight Color
                    case 2:
                        {
                            chstyle.Hair.HighlightColor = hair_and_color_data[i];
                            break;
                        }

                    // Eyebrow Color
                    case 3:
                        {
                            chstyle.EyebrowColor = hair_and_color_data[i];
                            break;
                        }

                    // Beard Color
                    case 4:
                        {
                            chstyle.BeardColor = hair_and_color_data[i];
                            break;
                        }

                    // Eye Color
                    case 5:
                        {
                            chstyle.EyeColor = hair_and_color_data[i];
                            break;
                        }

                    // Blush Color
                    case 6:
                        {
                            chstyle.BlushColor = hair_and_color_data[i];
                            break;
                        }

                    // Lipstick Color
                    case 7:
                        {
                            chstyle.LipstickColor = hair_and_color_data[i];
                            break;
                        }

                    // Chest Hair Color
                    case 8:
                        {
                            chstyle.ChestHairColor = hair_and_color_data[i];
                            break;
                        }
                }
            }

            if (player.hasData("ChangedGender")) player.resetData("ChangedGender");
            ApplyCharacterStyle(player, player.Account().CurrentCharacter);
            CharacterController.SaveCharacter(player, character, true);
            LeavCharacterEditor(player);
        }

        private void ClientSideLeaveEditor(Client client, string eventName, object[] arguments)
        {
            client.kick("Unbekannter Fehler..");
            //ApplyCharacterStyle(client, client.Account().CurrentCharacter);
            //LeavCharacterEditor(client);
        }
        #endregion ClientSideEvents

        private void CharacterController_OnPlayerSelectCharacter(Client client, Account e)
        {
            if(e.CurrentCharacter.CharacterStyleString == null || e.CurrentCharacter.CharacterStyleString == "")
            {
                API.delay(100, true, () =>
                {
                    OpenCharacterEditorForPlayer(client, false);
                    client.FadeScreenIn(1000);
                });
                return;
            }
            ApplyCharacterStyle(client, e.CurrentCharacter);
            API.delay(1000, true, () =>
            {
                client.FadeScreenIn(1000);
                OnPlayerFinishedCharacterEditor?.Invoke(client, client.Account());
            });
        }

        public static void ApplyCharacterStyle(Client player, Character character)
        {
            if(character.CharacterStyle == null)
            {
                return;
            }

            player.setSkin((character.CharacterStyle.Gender == 0) ? PedHash.FreemodeMale01 : PedHash.FreemodeFemale01);
            player.setDefaultClothes();
            player.setClothes(2, character.CharacterStyle.Hair.Hair, 0);

            GameMode.sharedAPI.sendNativeToAllPlayers(
                Hash.SET_PED_HEAD_BLEND_DATA,
                player.handle,

                character.CharacterStyle.Parents.Mother,
                character.CharacterStyle.Parents.Father,
                0,

                character.CharacterStyle.Parents.Mother,
                character.CharacterStyle.Parents.Father,
                0,

                character.CharacterStyle.Parents.Similarity,
                character.CharacterStyle.Parents.SkinSimilarity,
                0,

                false
            );

            for (int i = 0; i < character.CharacterStyle.Features.Length; i++) GameMode.sharedAPI.sendNativeToAllPlayers(Hash._SET_PED_FACE_FEATURE, player.handle, i, character.CharacterStyle.Features[i]);
            for (int i = 0; i < character.CharacterStyle.Appearance.Length; i++) GameMode.sharedAPI.sendNativeToAllPlayers(Hash.SET_PED_HEAD_OVERLAY, player.handle, i, character.CharacterStyle.Appearance[i].Value, character.CharacterStyle.Appearance[i].Opacity);

            // apply colors
            GameMode.sharedAPI.sendNativeToAllPlayers(Hash._SET_PED_HAIR_COLOR, player.handle, character.CharacterStyle.Hair.Color, character.CharacterStyle.Hair.HighlightColor);
            GameMode.sharedAPI.sendNativeToAllPlayers(Hash._SET_PED_EYE_COLOR, player.handle, character.CharacterStyle.EyeColor);

            GameMode.sharedAPI.sendNativeToAllPlayers(Hash._SET_PED_HEAD_OVERLAY_COLOR, player.handle, 1, 1, character.CharacterStyle.BeardColor, 0);
            GameMode.sharedAPI.sendNativeToAllPlayers(Hash._SET_PED_HEAD_OVERLAY_COLOR, player.handle, 2, 1, character.CharacterStyle.EyebrowColor, 0);
            GameMode.sharedAPI.sendNativeToAllPlayers(Hash._SET_PED_HEAD_OVERLAY_COLOR, player.handle, 5, 2, character.CharacterStyle.BlushColor, 0);
            GameMode.sharedAPI.sendNativeToAllPlayers(Hash._SET_PED_HEAD_OVERLAY_COLOR, player.handle, 8, 2, character.CharacterStyle.LipstickColor, 0);
            GameMode.sharedAPI.sendNativeToAllPlayers(Hash._SET_PED_HEAD_OVERLAY_COLOR, player.handle, 10, 1, character.CharacterStyle.ChestHairColor, 0);

            player.setSyncedData("CustomCharacter", GameMode.sharedAPI.toJson(character.CharacterStyle));
        }

        public static void LeavCharacterEditor(Client client)
        {
            client.triggerEvent("DestroyCamera");
            client.Account().IsInEditor = false;
            client.DisplayRadar(true);
            client.Account().CurrentCharacter.ClothingString = null;
            client.Account().CurrentCharacter.ClothingStringToClothing();
            OnPlayerFinishedCharacterEditor?.Invoke(client, client.Account());
        }

        public static void OpenCharacterEditorForPlayer(Client client, bool editExistCharacter = true)
        {
            client.Account().IsInEditor = true;
            client.triggerEvent("CreatorPrepare");
            GameMode.sharedAPI.delay(400, true, () =>
            {
                AntiCheatController.TeleportPlayer(client, Constants.CharacterEditorCharPos, new Vector3(0f, 0f, Constants.CharacterEditorCharFacingAngle));
                client.DisplayRadar(false);
                DimensionManager.RequestPrivateDimension(client);
                if (editExistCharacter)
                {
                    SetCreatorClothes(client, client.Account().CurrentCharacter.CharacterStyle.Gender);
                    client.triggerEvent("UpdateCreator", GameMode.sharedAPI.toJson(client.Account().CurrentCharacter.CharacterStyle), client.Account().CurrentCharacter.FirstName,
                        client.Account().CurrentCharacter.LastName);
                }
                else
                {
                    SetDefaultFeatures(client, 0);
                }

                client.triggerEvent("CreatorCamera", Constants.CharacterEditorCameraPos, Constants.CharacterEditorCameraLookAtPos, Constants.CharacterEditorCharFacingAngle);
            });
        }

        public static void SetDefaultFeatures(Client player, int gender, bool reset = false)
        {
            if (reset)
            {
                var chstyle = player.Account().CurrentCharacter.CharacterStyle;
                chstyle = new CharacterStyle();
                chstyle.Gender = gender;

                chstyle.Parents.Father = 0;
                chstyle.Parents.Mother = 21;
                chstyle.Parents.Similarity = (gender == 0) ? 1.0f : 0.0f;
                chstyle.Parents.SkinSimilarity = (gender == 0) ? 1.0f : 0.0f;
            }

            // will apply the resetted data
            ApplyCharacterStyle(player, player.Account().CurrentCharacter);

            // clothes
            SetCreatorClothes(player, gender);
        }

        public static void SetCreatorClothes(Client player, int gender)
        {
            // clothes
            player.setDefaultClothes();
            for (int i = 0; i < 10; i++) player.clearAccessory(i);

            if (gender == 0) // Mann
            {
                player.setClothes(3, 15, 0); // Torso
                player.setClothes(4, 21, 0); // Hose
                player.setClothes(6, 34, 0); // Schuhe
                player.setClothes(8, 15, 0); // Unterhemd
                player.setClothes(11, 15, 0); // Oberteil
            }
            else // Frau
            {
                player.setClothes(3, 15, 0); // Torso
                player.setClothes(4, 10, 0); // Hose
                player.setClothes(6, 35, 0); // Schuhe
                player.setClothes(8, 15, 0); // Unterhemd
                player.setClothes(11, 15, 0); // Oberteil
            }

            player.setClothes(2, player.Account().CurrentCharacter.CharacterStyle.Hair.Hair, 0);
        }
    }
}
