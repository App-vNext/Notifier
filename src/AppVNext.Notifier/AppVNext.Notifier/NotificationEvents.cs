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
			WriteLine("The user clicked on the toast.");
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
					WriteLine("The notification has been closed.");
					Exit(1);
					break;
				case ToastDismissalReason.UserCanceled:
					//				var d12 = DismissalActions.Hidden;
					WriteLine("The user dismissed this toast");
					Exit(2);
					break;
				case ToastDismissalReason.TimedOut:
					//					var d2 = DismissalActions.Timeout;
					WriteLine("The toast has timed out");
					Exit(3);
					break;
			}
		}

		internal void Failed(ToastNotification sender, ToastFailedEventArgs e)
		{
			WriteLine("An error has occurred.");
			Exit(-1);
		}
	}
}
