using AppVNext.Notifier.Common;
using System.Collections.Generic;
using System.Reflection;
using Windows.UI.Notifications;
using static System.Console;
using static System.Environment;

namespace AppVNext.Notifier
{
	/// <summary>
	/// Used when the Notification has been activated, dismissed or failed.
	/// </summary>
	class NotificationEvents
	{
		/// <summary>
		/// Triggered when the notification has been activated or clicked on.
		/// </summary>
		/// <param name="sender">ToastNotification sender object.</param>
		/// <param name="e">Used to get the properties when the notifications has been activated or clicked on.</param>
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

		/// <summary>
		/// Triggered when the notification has been dismissed.
		/// </summary>
		/// <param name="sender">ToastNotification sender object.</param>
		/// <param name="e">Toast dismissed event arguments.</param>
		internal void Dismissed(ToastNotification sender, ToastDismissedEventArgs e)
		{
			switch (e.Reason)
			{
				case ToastDismissalReason.ApplicationHidden:
					WriteLine("The notification has been closed.");
					Exit((int)DismissalActions.Hidden);
					break;
				case ToastDismissalReason.UserCanceled:
					WriteLine("The user dismissed this toast.");
					Exit((int)DismissalActions.Dismissed);
					break;
				case ToastDismissalReason.TimedOut:
					WriteLine("The toast has timed out.");
					Exit((int)DismissalActions.Timeout);
					break;
			}
		}

		/// <summary>
		/// Triggered when the notification failed.
		/// </summary>
		/// <param name="sender">ToastNotification sender object.</param>
		/// <param name="e">Toast failed event arguments.</param>
		internal void Failed(ToastNotification sender, ToastFailedEventArgs e)
		{
			WriteLine($"An error has occurred. {e.ErrorCode}");
			Exit(-1);
		}
	}
}
