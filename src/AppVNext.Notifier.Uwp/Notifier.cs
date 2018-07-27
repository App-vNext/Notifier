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

			var image = "https://picsum.photos/360/202?image=883";
			var logo = "ms-appdata:///local/Icon.ico";

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

			if (!string.IsNullOrWhiteSpace(arguments.PicturePath))
			{
				var imagePath = "file:///" + arguments.PicturePath;
				//visual.BindingGeneric.Children.Add(new AdaptiveImage()
				//{
				//	Source = imagePath
				//});

				visual.BindingGeneric.AppLogoOverride = new ToastGenericAppLogo()
				{
					Source = imagePath,
					HintCrop = ToastGenericAppLogoCrop.Circle
				};
			}

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

			ToastActionsCustom actions = new ToastActionsCustom()
			{
				Inputs =
				{
					new ToastTextBox("tbReply")
					{
						PlaceholderContent = "Type a response"
					}
				},

				Buttons =
				{
					new ToastButton("Reply", new QueryString()
					{
						{ "action", "reply" },
						{ "conversationId", arguments.NotificationId }

					}.ToString())
					{
						ActivationType = ToastActivationType.Background,
						ImageUri = "Assets/Reply.png",
 
						// Reference the text box's ID in order to
						// place this button next to the text box
						TextBoxId = "tbReply"
					},

					new ToastButton("Like", new QueryString()
					{
						{ "action", "like" },
						{ "conversationId", arguments.NotificationId }

					}.ToString())
					{
						ActivationType = ToastActivationType.Background
					},

					new ToastButton("View", new QueryString()
					{
						{ "action", "viewImage" },
						{ "imageUrl", image }

					}.ToString())
				}
			};

			// Now we can construct the final toast content
			ToastContent toastContent = new ToastContent()
			{
				Visual = visual,
				Audio = audio,
				//Actions = actions,

				// Arguments when the user taps body of toast
				Launch = new QueryString()
				{
					{ "action", "viewConversation" },
					{ "conversationId", arguments.NotificationId }

				}.ToString()
			};

			// Create the toast notification
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

			var events = new NotificationEvents();

			toast.Activated += events.Activated;
			toast.Dismissed += events.Dismissed;
			toast.Failed += events.Failed;


			ToastNotificationManager.CreateToastNotifier().Show(toast);

			return toast;
		}
	}
}
