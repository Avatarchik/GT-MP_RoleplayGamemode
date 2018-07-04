using Roleplay.Base;
using System;
using System.Timers;

namespace Roleplay.Server.Controller
{
    internal class WorldEnvironmentController : RoleplayScript
    {
        private static Timer WorldTimeTimer = null;
        public static event NoArgumentsEventHandler OnUpdateWorldTime;

        public WorldEnvironmentController()
        {
            GameMode.OnWorldStartupFirst += GameMode_OnWorldStartupFirst;
            GameMode.OnWorldShutdown += GameMode_OnWorldShutdown;
        }

        private void GameMode_OnWorldStartupFirst()
        {
            if (WorldTimeTimer != null)
            {
                API.stopTimer(WorldTimeTimer);
                WorldTimeTimer = null;
            }
            WorldTimeTimer = API.startTimer(60000, false, () =>
            {
                API.setTime(DateTime.Now.Hour, DateTime.Now.Minute);
                OnUpdateWorldTime?.Invoke();
            });
        }

        private void GameMode_OnWorldShutdown()
        {
            if (WorldTimeTimer != null)
            {
                API.stopTimer(WorldTimeTimer);
                WorldTimeTimer = null;
            }
        }
    }
}