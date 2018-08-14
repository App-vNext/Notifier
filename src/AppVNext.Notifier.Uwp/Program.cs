using System;
using System.Linq;
using static System.Console;
using AppVNext.Notifier.Common;

/// <summary>
/// Notifier main program.
/// </summary>
namespace AppVNext.Notifier.Uwp
{
	class Program
	{
		/// <summary>
		/// Main method.
		/// </summary>
		/// <param name="args">Arguments for the notification.</param>
		static void Main(string[] args)
		{
			//Initialize application type. TODO: Replace this with dependency injection.
			Globals.ApplicationType = ApplicationTypes.UwpNative;

			//RegisterBackgroundTask();

			var arguments = ArgumentManager.ProcessArguments(args);

			if (arguments == null)
			{
				WriteLine($"{Globals.HelpForNullMessage}{Globals.HelpForErrors}");
				ArgumentManager.DisplayHelp();
			}
			else
			{
				//////if (arguments.Register)
				//////{
				//////	if (ShortcutHelper.CreateShortcutIfNeeded(arguments.ApplicationId, arguments.ApplicationName))
				//////	{
				//////		WriteLine(string.Format(Globals.HelpForRegisterSuccess, arguments.ApplicationId, arguments.ApplicationName));
				//////	}
				//////	else
				//////	{
				//////		WriteLine(string.Format(Globals.HelpForRegisterFail, arguments.ApplicationId, arguments.ApplicationName));
				//////	}
				//////}

				//////if (arguments.NotificationsCheck)
				//////{
				//////	WriteLine(RegistryHelper.AreNotificationsEnabled(arguments.NotificationCheckAppId));
				//////}

				//////if (arguments.PushNotificationCheck)
				//////{
				//////	WriteLine(RegistryHelper.ArePushNotificationsEnabled());
				//////}

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

		//private static void RegisterBackgroundTask()
		//{
		//	const string taskName = "ToastBackgroundTask";

		//	// If background task is already registered, do nothing
		//	if (BackgroundTaskRegistration.AllTasks.Any(i => i.Value.Name.Equals(taskName)))
		//		return;

		//	// Otherwise create the background task
		//	var builder = new BackgroundTaskBuilder()
		//	{
		//		Name = taskName,
		//		TaskEntryPoint = typeof(BackgroundTaskComponent.ToastNotificationBackgroundTask).FullName
		//	};

		//	// And set the toast action trigger
		//	builder.SetTrigger(new ToastNotificationActionTrigger());

		//	// And register the task
		//	//builder.Register();
		//}
	}
}