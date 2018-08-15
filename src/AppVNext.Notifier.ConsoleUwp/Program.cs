using AppVNext.Notifier.Common;
using DesktopNotifications;
using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using static System.Console;

namespace AppVNext.Notifier
{
	/// <summary>
	/// Notifier main program.
	/// </summary>
	class Program
	{
		/// <summary>
		/// Main method.
		/// </summary>
		/// <param name="args">Arguments for the notification.</param>
		static void Main(string[] args)
		{
			//Initialize application type. TODO: Replace this with dependency injection.
			Globals.ApplicationType = ApplicationTypes.UwpConsole;

			// Register AUMID, COM server, and activator
			DesktopNotificationManagerCompat.RegisterAumidAndComServer<NotifierActivator>("BraveAdsNotifier");
			DesktopNotificationManagerCompat.RegisterActivator<NotifierActivator>();

			// If launched from a toast notification
			if (args.Contains(DesktopNotificationManagerCompat.TOAST_ACTIVATED_LAUNCH_ARG))
			{
				//TODO: Handle if launched from a toast notification
			}
			else
			{
				var arguments = ArgumentManager.ProcessArguments(args);

				if (arguments == null)
				{
					WriteLine($"{Globals.HelpForNullMessage}{Globals.HelpForErrors}");
					ArgumentManager.DisplayHelp();
				}
				else
				{
					if (arguments.NotificationsCheck)
					{
						WriteLine(RegistryHelper.AreNotificationsEnabled(arguments.NotificationCheckAppId));
					}

					if (arguments.PushNotificationCheck)
					{
						WriteLine(RegistryHelper.ArePushNotificationsEnabled());
					}

					if (arguments.VersionInformation)
					{
						WriteLine(Globals.GetVersionInformation());
					}

					if (arguments.ClearNotifications)
					{
						DesktopNotificationManagerCompat.History.Clear();
					}

					if (string.IsNullOrEmpty(arguments.Errors) && !string.IsNullOrEmpty(arguments.Message))
					{
						SendNotification(arguments);
						while (arguments.Wait) { System.Threading.Thread.Sleep(500); }
					}
					else
					{
						WriteLine($"{(arguments.Errors ?? string.Empty)}");
					}
				}
			}
		}

		/// <summary>
		/// Send notification.
		/// </summary>
		/// <param name="arguments">Notification arguments object.</param>
		private static void SendNotification(NotificationArguments arguments)
		{
			//if (arguments.ApplicationId == Globals.DefaultApplicationId)
			//{
			//	ShortcutHelper.CreateShortcutIfNeeded(arguments.ApplicationId, arguments.ApplicationName);
			//}
			var toast = Notifier.ShowToast(arguments);
		}
	}
}
