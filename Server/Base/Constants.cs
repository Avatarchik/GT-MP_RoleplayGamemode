using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Server.Base
{
    public static class Constants
    {
        public readonly static Vector3 EmptyVector = new Vector3(0, 0, 0);
        public readonly static Vector3 LoginRegisterCamera = new Vector3(418.9948, 747.476, 192);
        public readonly static Vector3 LoginRegisterCameraLookAt = new Vector3(-428.2029, 1050.306, 320.3225);
        public readonly static Vector3 ConnectPosition = new Vector3(425.0599, 745.7097, 192.64);
        public readonly static Vector3 DefaultSpawnPosition = new Vector3(-256.5307, -295.8158, 21.62641);
        public readonly static double DefaultSpawnRotation = -160.6727;
        public readonly static double StartCash = 500;
        public readonly static string AccountLoginUrl = "https://gtaassets.tam.moe/roleplay/login.html";
        public readonly static string AccountRegisterUrl = "https://gtaassets.tam.moe/roleplay/register.html";
        public readonly static string BankAtmUrl = "https://gtaassets.tam.moe/roleplay/atm.html";
        public readonly static string GarageOverviewUrl = "https://gtaassets.tam.moe/roleplay/garage_main.html";
        public readonly static string GarageParkOutListUrl = "https://gtaassets.tam.moe/roleplay/garage_parkout_list.html";
        public readonly static string MenuUrl = "https://gtaassets.tam.moe/roleplay/menu.html";

        public readonly static Vector3 CharacterEditorCharPos = new Vector3(402.8664, -996.4108, -99.00027);
        public readonly static Vector3 CharacterEditorCameraPos = new Vector3(402.8664, -997.5515, -98.5);
        public readonly static Vector3 CharacterEditorCameraLookAtPos = new Vector3(402.8664, -996.4108, -98.5);
        public readonly static float CharacterEditorCharFacingAngle = -185.0f;

        public readonly static double UnemploymentBenefits = 100; // Arbeitslosengeld
        public readonly static int SalaryTimer = 15; // Alle wieviel Minuten gibt es gehalt
        public readonly static double BankAccountStartMoney = 1000;
    }
}
