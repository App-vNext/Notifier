using System;
using Windows.UI.Notifications;

namespace AppVNext.Notifier
{
	class NotificationEvents
	{
		internal void Activated(ToastNotification sender, object e)
		{
			Console.WriteLine("The user activated the notification");
		}

		internal void Dismissed(ToastNotification sender, ToastDismissedEventArgs e)
		{
			String outputText = string.Empty;
			switch (e.Reason)
			{
				case ToastDismissalReason.ApplicationHidden:
					outputText = "The app hid the notification using ToastNotifier.Hide.";
					break;
				case ToastDismissalReason.UserCanceled:
					outputText = "The user dismissed the notification.";
					break;
				case ToastDismissalReason.TimedOut:
					outputText = "The notification has timed out.";
					break;
			}

			Console.WriteLine(outputText);
		}

		internal void Failed(ToastNotification sender, ToastFailedEventArgs e)
		{
			Console.WriteLine("The notification encountered an error.");
		}
	}
}
