using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using Newtonsoft.Json;
using Roleplay.Server.Base;
using System;
using System.Collections.Generic;

namespace Roleplay.Server.Models.MenuBuilder
{
    public class Menu
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Anchor { get; set; }
        public List<MenuItem> Items { get; set; }
        public string Identifier { get; set; }
        public bool OnIndexChangeEvent { get; set; }
        public bool OnMenuCloseEvent { get; set; }
        public int ExtraInt { get; set; }

        public Menu(string title, string subTitle = "", int x = 0, int y = 0, MenuAnchor anchor = MenuAnchor.MiddleRight, string identifier = "")
        {
            Title = title;
            SubTitle = subTitle;
            X = x;
            Y = y;
            Anchor = (int)anchor;
            Items = new List<MenuItem>();
            Identifier = identifier;
            OnIndexChangeEvent = false;
            OnMenuCloseEvent = false;
            ExtraInt = 0;
        }

        public Menu(string title, string subTitle, string identifier = "")
        {
            Title = title;
            SubTitle = subTitle;
            X = 0;
            Y = 0;
            Anchor = (int)MenuAnchor.MiddleRight;
            Items = new List<MenuItem>();
            Identifier = identifier;
            OnIndexChangeEvent = false;
            OnMenuCloseEvent = false;
            ExtraInt = 0;
        }

        public void Show(Client client)
        {
            Show(client, this);
        }

        public static void Show(Client client, Menu menu)
        {
            client.triggerEvent("CREATE_MENU", JsonConvert.SerializeObject(menu));
        }

        public static void Close(Client client)
        {
            client.triggerEvent("CLOSE_MENU");
        }
    }

    public class MenuItem
    {
        public string LeftLabel { get; set; }
        public string RightLabel { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public string EventTrigger { get; set; }
        public string EventString { get; set; }
        public int EventInt { get; set; }
        public bool OpenUserInput { get; set; }
        public string UserInputDefaultText { get; set; }

        public MenuItem(string leftLabel, string description = "", string rightLabel = "", bool enabled = true, string eventTrigger = "", 
            string eventString = "", int eventInt = 0, bool openUserInput = false, string userInputDefaultText = "")
        {
            LeftLabel = leftLabel;
            Description = description;
            RightLabel = rightLabel;
            Enabled = enabled;
            EventTrigger = eventTrigger;
            EventString = eventString;
            EventInt = eventInt;
            OpenUserInput = openUserInput;
            UserInputDefaultText = userInputDefaultText;
        }
    }

    public enum MenuAnchor
    {
        TopLeft = 0,
        TopMiddle = 1,
        TopRight = 2,
        MiddleLeft = 3,
        Middle = 4,
        MiddleRight = 6,
        BottomLeft = 7,
        BottomCenter = 8,
        BottomRight = 9
    }

    public class MenuEventData
    {
        public string MenuIdentifier { get; set; }
        public string EventTrigger { get; set; }
        public string EventString { get; set; }
        public int EventInt { get; set; }
        public string UserInput { get; set; }

        public MenuEventData()
        {
            UserInput = "";
        }
    }

    public class MenuHandlerDelegate
    {
        public string Name;
        public EntityType EntityType = EntityType.Player;
        public int position;
        public Action<Client, Menu> Handler;
    }
}