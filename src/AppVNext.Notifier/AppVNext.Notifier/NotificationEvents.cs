using System;
using Windows.UI.Notifications;
using static System.Environment;
using static System.Console;
using System.Collections.Generic;
using System.Reflection;

namespace AppVNext.Notifier
{
	class NotificationEvents
	{
		internal void Activated(ToastNotification sender, object e)
		{
			var type = e.GetType();
			var properties = new List<PropertyInfo>(type.GetProperties());

			var results = string.Empty;

			foreach (var property in properties)
			{
				if (!string.IsNullOrEmpty(results))
				{
					results += $"{results}{Globals.NewLine}";
				}
				if (property.GetValue(e, null) is string value && !string.IsNullOrWhiteSpace(value))
				{
					results += $"{property.Name}: {value}";
				}
			}

			WriteLine($"The user clicked on the toast. {results}");
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
			WriteLine($"An error has occurred. {e.ErrorCode}");
			Exit(-1);
		}
	}
}
