using AppVNext.Notifier.Common;
using Microsoft.Win32;

namespace AppVNext.Notifier
{
	/// <summary>
	/// Helper class to read Windows registry setting.
	/// </summary>
	static class RegistryHelper
	{
		/// <summary>
		/// Returns whether Notifications are enabled or not in Windows.
		/// </summary>
		/// <param name="appId">Application ID of the application to check the setting for.</param>
		/// <returns>Unknown = -1, Disabled = 0, Enabled = 1</returns>
		internal static EnableTypes AreNotificationsEnabled(string appId)
		{
			var notificationKey = string.Format(Globals.NotificationKey, appId);
			var setting = Registry.GetValue(notificationKey, "Enabled", 1);

			if (setting == null)
			{
				return EnableTypes.Unknown;
			}

			if (setting.GetType() != typeof(string))
			{
				int.TryParse(setting.ToString(), out int value);
				return value == 0 ? EnableTypes.Disabled : EnableTypes.Enabled;
			}

			return EnableTypes.Unknown;
		}

		/// <summary>
		/// Returns whether Push Notifications are enabled or not in Windows.
		/// </summary>
		/// <returns>Unknown = -1, Disabled = 0, Enabled = 1</returns>
		internal static EnableTypes ArePushNotificationsEnabled()
		{
			var notificationKey = string.Format(Globals.PushNotificationKey);
			var setting = Registry.GetValue(notificationKey, "ToastEnabled", null);

			if (setting == null)
			{
				return EnableTypes.Unknown;
			}

			if (setting.GetType() != typeof(string))
			{
				int.TryParse(setting.ToString(), out int value);
				return value == 0 ? EnableTypes.Disabled : EnableTypes.Enabled;
			}

			return EnableTypes.Unknown;
		}
	}
}