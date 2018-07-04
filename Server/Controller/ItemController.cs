using GrandTheftMultiplayer.Server.Elements;
using Roleplay.Base;
using Roleplay.Server.Extensions;
using Roleplay.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Server.Controller
{
    public class ItemController : RoleplayScript
    {
        public delegate void ItemActionEventHandler<TSender, TEventArgs, TEventArgs2>(TSender client, TEventArgs item, TEventArgs2 itemCount);
        public static event ItemActionEventHandler<Client, InventoryItem, int> OnPlayerUseItem;
        public static event ItemActionEventHandler<Client, InventoryItem, int> OnPlayerDisposeItem;

        public ItemController()
        {

        }

        public static void UseItem(Client client, InventoryItem item, int useCount = 1)
        {
            if (item == null)
                return;
            OnPlayerUseItem?.Invoke(client, item, useCount);
        }

        public static void DisposeItem(Client client, InventoryItem item, int useCount = 1)
        {
            if (item == null)
                return;
            OnPlayerDisposeItem?.Invoke(client, item, useCount);
        }

        #region Helper Functions
        public static bool DoesPlayerOwnItem(Client client, int itemId)
        {
            return client.Character().Inventory.Where(x => x.ItemId == itemId) != null;
        }

        public static string GetUsageTriggerFromItemId(int itemId)
        {
            ItemInfo item = GameMode.DbContext.ItemInformations.FirstOrDefault(x => x.Id == itemId);
            if (item == null)
                return "";
            return item.UsageTrigger;
        }

        public static string GetDisposeTriggerFromItemId(int itemId)
        {
            ItemInfo item = GameMode.DbContext.ItemInformations.FirstOrDefault(x => x.Id == itemId);
            if (item == null)
                return "";
            return item.DisposeTrigger;
        }
        #endregion Helper Functions
    }
}
