using System;
using System.Drawing;

namespace Roleplay.Server.Discord
{
	internal class DiscordService
	{
		public static readonly string WebHookString = "https://discordapp.com/api/webhooks/400423361613660162/rsfUwx9rTSLuM18CBt18bkARg31AGnAXGcrUNGJwmcgzfMTS--nXWDPkJN2VAIho6lu9";

		public static async void SendInfoMessage(string message)
		{
			Webhook webhook = new Webhook(WebHookString);
			Embed embed = new Embed();
			embed.Title = "Server Information";
			embed.Description = message;
			embed.Color = Color.LightBlue.ToRgb();
			webhook.Embeds.Add(embed);
			try
			{
				await webhook.Send();
			}
			catch (Exception) { }
		}

		public static async void SendWarningMessage(string message)
		{
			Webhook webhook = new Webhook(WebHookString);
			Embed embed = new Embed();
			embed.Title = "Server Warnung";
			embed.Description = message;
			embed.Color = Color.Orange.ToRgb();
			webhook.Embeds.Add(embed);
			try
			{
				await webhook.Send();
			}
			catch (Exception) { }
		}

		public static async void SendErrorMessage(string message)
		{
			Webhook webhook = new Webhook(WebHookString);
			Embed embed = new Embed();
			embed.Title = "Server Fehler";
			embed.Description = message;
			embed.Color = Color.Red.ToRgb();
			webhook.Embeds.Add(embed);
			try
			{
				await webhook.Send();
			}
			catch (Exception) { }
		}
	}
}