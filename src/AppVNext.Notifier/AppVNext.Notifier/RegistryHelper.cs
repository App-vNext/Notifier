using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppVNext.Notifier
{
	static class RegistryHelper
	{
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
