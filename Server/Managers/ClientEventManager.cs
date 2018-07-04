using GrandTheftMultiplayer.Server.Elements;
using Roleplay.Base;
using Roleplay.Server.Enums;
using System;
using System.Collections.Concurrent;
using static GrandTheftMultiplayer.Server.API.API;

namespace Roleplay.Server.Managers
{
    internal class ClientEventManager : RoleplayScript
    {
        private static ConcurrentDictionary<string, ServerEventTrigger> _ClientEvents = new ConcurrentDictionary<string, ServerEventTrigger>(StringComparer.InvariantCultureIgnoreCase);

        public static void RegisterClientEvent(string eventName, ServerEventTrigger serverFunction)
        {
            _ClientEvents[eventName] = serverFunction;
        }

        public override ScriptPriority ScriptStartPosition => ScriptPriority.FIRST;

        public override void OnScriptStart()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if (_ClientEvents.TryGetValue(eventName, out var handler))
            {
                try
                {
                    handler.Invoke(player, eventName, arguments);
                    return;
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Problem in onClientEventTrigger {0}", eventName);
                }
            }
            else
            {
                logger.Trace("Unhandled ClientEvent: {0}", eventName);
            }
        }
    }
}