using AppVNext.Notifier.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.QueryStringDotNET;

namespace AppVNext.Notifier
{
	/// <summary>
	/// Notifier class.
	/// </summary>
	static class Notifier
	{
		/// <summary>
		/// Show toast notification.
		/// </summary>
		/// <param name="arguments">Notification arguments object.</param>
		/// <returns>Toast notification object.</returns>
		public static ToastNotification ShowToast(NotificationArguments arguments)
		{
			var notifier = ToastNotificationManager.CreateToastNotifier(arguments.ApplicationId);
			var scheduled = notifier.GetScheduledToastNotifications();

			for (var i = 0; i < scheduled.Count; i++)
			{
				// The itemId value is the unique ScheduledTileNotification.Id assigned to the notification when it was created.
				if (scheduled[i].Id == "")
				{
					notifier.RemoveFromSchedule(scheduled[i]);
				}
			}

			//Set the toast visual
			var visual = new ToastVisual()
			{
				BindingGeneric = new ToastBindingGeneric()
				{
					Children =
					{
						new AdaptiveText()
						{
							Text = arguments.Title
						},
						new AdaptiveText()
						{
							Text = arguments.Message
						}
					}
				}
			};

			//Set the image
			var imagePath = "file:///" + (string.IsNullOrWhiteSpace(arguments.PicturePath) 
				? Globals.DefaultIcon
				: arguments.PicturePath);

			visual.BindingGeneric.AppLogoOverride = new ToastGenericAppLogo()
			{
				Source = imagePath,
				HintCrop = ToastGenericAppLogoCrop.Circle
			};

			//Set the audio
			ToastAudio audio = null;
			if (!string.IsNullOrWhiteSpace(arguments.WindowsSound) || !string.IsNullOrWhiteSpace(arguments.SoundPath))
			{
				string sound;
				if (string.IsNullOrWhiteSpace(arguments.WindowsSound))
				{
					sound = "file:///" + arguments.SoundPath;
				}
				else
				{
					sound = $"ms-winsoundevent:{arguments.WindowsSound}";
				}

				audio = new ToastAudio()
				{
					Src = new Uri(sound),
					Loop = bool.Parse(arguments.Loop),
					Silent = arguments.Silent
				};
			}

			// Construct the toast content
			var toastContent = new ToastContent()
			{
				Visual = visual,
				Audio = audio
			};

			// Create notification
			var toast = new ToastNotification(toastContent.GetXml());

			// Set the expiration time
			if (!string.IsNullOrWhiteSpace(arguments.Duration))
			{
				switch (arguments.Duration)
				{
					case "short":
						toast.ExpirationTime = DateTime.Now.AddSeconds(5);
						break;
					case "long":
						toast.ExpirationTime = DateTime.Now.AddSeconds(25);
						break;
				}
			}

			//Add event handlers
			var events = new NotificationEvents();
			toast.Activated += events.Activated;
			toast.Dismissed += events.Dismissed;
			toast.Failed += events.Failed;

			//Show notification
			ToastNotificationManager.CreateToastNotifier().Show(toast);

			return toast;
		}
	}
}
