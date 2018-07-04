using GrandTheftMultiplayer.Server.Elements;
using Roleplay.Base;
using Roleplay.Server.Controller;
using Roleplay.Server.Enums;
using Roleplay.Server.Models;
using System;
using System.Collections.Concurrent;

namespace Roleplay.Server.Managers
{
    internal class ItemDisposeManager : RoleplayScript
    {
        public delegate void ItemDisposeTrigger(Client client, int itemId, int count, string itemTriggerName);

        public static ConcurrentDictionary<string, ItemDisposeTrigger> _ItemDisposeTriggers = new ConcurrentDictionary<string, ItemDisposeTrigger>(StringComparer.InvariantCultureIgnoreCase);

        public static void RegisterItemUsageTrigger(string itemTriggerName, ItemDisposeTrigger itemTrigger)
        {
            _ItemDisposeTriggers[itemTriggerName] = itemTrigger;
        }

        public override ScriptPriority ScriptStartPosition => ScriptPriority.FIRST;

        public override void OnScriptStart()
        {
            ItemController.OnPlayerDisposeItem += ItemController_OnPlayerDisposeItem;
        }

        private void ItemController_OnPlayerDisposeItem(Client client, InventoryItem item, int itemCount)
        {
            string triggerName = ItemController.GetDisposeTriggerFromItemId(item.ItemId);
            if (_ItemDisposeTriggers.TryGetValue(triggerName, out var handler))
            {
                try
                {
                    handler.Invoke(client, item.ItemId, itemCount, triggerName);
                    return;
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Problem in OnPlayerDisposeItem bei trigger: {0}", triggerName);
                }
            }
            else
            {
                logger.Trace("Unhandled OnPlayerDisposeItemEvent: {0}", triggerName);
            }
        }
    }
}