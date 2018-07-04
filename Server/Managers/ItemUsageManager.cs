using GrandTheftMultiplayer.Server.Elements;
using Roleplay.Base;
using Roleplay.Server.Controller;
using Roleplay.Server.Enums;
using Roleplay.Server.Models;
using System;
using System.Collections.Concurrent;

namespace Roleplay.Server.Managers
{
    internal class ItemUsageManager : RoleplayScript
    {
        public delegate void ItemUsageTrigger(Client client, InventoryItem item, int count, string itemTriggerName);

        public static ConcurrentDictionary<string, ItemUsageTrigger> _ItemUsageTriggers = new ConcurrentDictionary<string, ItemUsageTrigger>(StringComparer.InvariantCultureIgnoreCase);

        public static void RegisterItemUsageTrigger(string itemTriggerName, ItemUsageTrigger itemTrigger)
        {
            _ItemUsageTriggers[itemTriggerName] = itemTrigger;
        }

        public override ScriptPriority ScriptStartPosition => ScriptPriority.FIRST;

        public override void OnScriptStart()
        {
            ItemController.OnPlayerUseItem += ItemController_OnPlayerUseItem;
        }

        private void ItemController_OnPlayerUseItem(Client client, InventoryItem item, int itemCount)
        {
            string triggerName = ItemController.GetUsageTriggerFromItemId(item.ItemId);
            if (_ItemUsageTriggers.TryGetValue(triggerName, out var handler))
            {
                try
                {
                    handler.Invoke(client, item, itemCount, triggerName);
                    return;
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Problem in OnPlayerUseItem bei trigger: {0}", triggerName);
                }
            }
            else
            {
                logger.Trace("Unhandled OnPlayerUseItemEvent: {0}", triggerName);
            }
        }
    }
}