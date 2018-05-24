using System;
using Windows.UI.Notifications;
using static System.Environment;
using static System.Console;

namespace AppVNext.Notifier
{
	class NotificationEvents
	{
		internal void Activated(ToastNotification sender, object e)
		{
			WriteLine("The user activated the notification");
			Exit(0);
		}

		internal void Dismissed(ToastNotification sender, ToastDismissedEventArgs e)
		{
			switch (e.Reason)
			{
				//Failed	-1
				//Success	0
				//Hidden	1
				//Dismissed	2
				//Timeout	3

				case ToastDismissalReason.ApplicationHidden:
					//					var d = DismissalActions.Hidden;
					WriteLine("The app hid the notification using ToastNotifier.Hide.");
					Exit(1);
					break;
				case ToastDismissalReason.UserCanceled:
					//				var d12 = DismissalActions.Hidden;
					WriteLine("The user dismissed the notification.");
					Exit(2);
					break;
				case ToastDismissalReason.TimedOut:
					//					var d2 = DismissalActions.Timeout;
					WriteLine("The notification has timed out.");
					Exit(3);
					break;
			}
		}

		internal void Failed(ToastNotification sender, ToastFailedEventArgs e)
		{
			WriteLine("The notification encountered an error.");
			Exit(-1);
		}
	}
}
