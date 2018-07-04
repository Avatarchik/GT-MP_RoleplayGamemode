using GrandTheftMultiplayer.Server.Elements;
using Roleplay.Base;
using Roleplay.Server.Managers;
using Roleplay.Server.Models.MenuBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Server.Controller
{
    public class MenuController : RoleplayScript
    {
        public static event EventHandler<Client, MenuEventData> OnPlayerMenuSelectEvent;       
        public static event EventHandler<Client, MenuEventData> OnPlayerMenuIndexChangeEvent;       
        public static event EventHandler<Client, string> OnPlayerMenuCloseEvent;
        public MenuController()
        {
            ClientEventManager.RegisterClientEvent("MENU_SELECT_EVENT", MenuSelectEvent);
            ClientEventManager.RegisterClientEvent("MENU_INDEX_CHANGE_EVENT", MenuIndexChangeEvent);
            ClientEventManager.RegisterClientEvent("MENU_CLOSE_EVENT", MenuCloseEvent);
        }

        private void MenuCloseEvent(Client sender, string eventName, object[] arguments)
        {
            OnPlayerMenuCloseEvent?.Invoke(sender, arguments[0].ToString());
        }

        private void MenuIndexChangeEvent(Client sender, string eventName, object[] arguments)
        {
            var data = new MenuEventData
            {
                MenuIdentifier = arguments[0].ToString(),
                EventTrigger = arguments[1].ToString(),
                EventString = arguments[2].ToString(),
                EventInt = Convert.ToInt32(arguments[3])
            };
            OnPlayerMenuIndexChangeEvent?.Invoke(sender, data);
        }

        private void MenuSelectEvent(Client sender, string eventName, object[] arguments)
        {
            var data = new MenuEventData
            {
                MenuIdentifier = arguments[0].ToString(),
                EventTrigger = arguments[1].ToString(),
                EventString = arguments[2].ToString(),
                EventInt = Convert.ToInt32(arguments[3]),
                UserInput = arguments[4].ToString()
            };
            OnPlayerMenuSelectEvent?.Invoke(sender, data);
        }

        public static void CloseAllMenus(Client client)
        {
            client.triggerEvent("CLOSE_MENU");
        }
    }
}
