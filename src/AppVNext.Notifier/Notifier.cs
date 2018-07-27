using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using System.Linq;
using System.Diagnostics;
using AppVNext.Notifier.Common;

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

			XmlDocument toastXml;

			//Set the image
			//if (string.IsNullOrWhiteSpace(arguments.PicturePath))
			//{
			//	if (string.IsNullOrWhiteSpace(arguments.Title))
			//	{
			//		toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
			//		var stringElements = toastXml.GetElementsByTagName("text");
			//		stringElements[0].AppendChild(toastXml.CreateTextNode(arguments.Message));
			//	}
			//	else
			//	{
			//		toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
			//		var stringElements = toastXml.GetElementsByTagName("text");
			//		stringElements[0].AppendChild(toastXml.CreateTextNode(arguments.Title));
			//		stringElements[1].AppendChild(toastXml.CreateTextNode(arguments.Message));
			//	}
			//}
			//else
			//{
				if (string.IsNullOrWhiteSpace(arguments.Title))
				{
					toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText01);
					var stringElements = toastXml.GetElementsByTagName("text");
					stringElements[0].AppendChild(toastXml.CreateTextNode(arguments.Message));
				}
				else
				{
					toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText02);
					var stringElements = toastXml.GetElementsByTagName("text");
					stringElements[0].AppendChild(toastXml.CreateTextNode(arguments.Title));
					stringElements[1].AppendChild(toastXml.CreateTextNode(arguments.Message));
				}
			//}

			var imagePath = "file:///" + (string.IsNullOrWhiteSpace(arguments.PicturePath)
			? Globals.DefaultIcon
			: arguments.PicturePath);

			var imageElements = toastXml.GetElementsByTagName("image");
			imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;

			if (arguments.Silent)
			{
				SetSilentAttribute(toastXml);
			}

			if (!string.IsNullOrWhiteSpace(arguments.WindowsSound) || !string.IsNullOrWhiteSpace(arguments.SoundPath))
			{
				SetSoundAttribute(arguments, toastXml);
			}

			SetBrandingAttribute(toastXml);

			var toast = new ToastNotification(toastXml);
			var events = new NotificationEvents();

			toast.Activated += events.Activated;
			toast.Dismissed += events.Dismissed;
			toast.Failed += events.Failed;

			//SetCommandsAttribute is commented out as is not used at this moment.
			//SetCommandsAttribute(toastXml);

			SetVisualAttribute(toastXml);

			if (!string.IsNullOrWhiteSpace(arguments.Duration))
			{
				SetDurationAttribute(toastXml, arguments.Duration);
			}

			Debug.WriteLine(toastXml.GetXml());

			ToastNotificationManager.CreateToastNotifier(arguments.ApplicationId).Show(toast);			

			return toast;
		}

		/// <summary>
		/// Set sound attribute.
		/// </summary>
		/// <param name="arguments">Notification arguments object.</param>
		/// <param name="toastXml">XML object used to add the sound attribute to.</param>
		private static void SetSoundAttribute(NotificationArguments arguments, XmlDocument toastXml)
		{
			var audio = toastXml.GetElementsByTagName("audio").FirstOrDefault();

			if (audio == null)
			{
				audio = toastXml.CreateElement("audio");
				var toastNode = ((XmlElement)toastXml.SelectSingleNode("/toast"));

				if (toastNode != null)
				{
					toastNode.AppendChild(audio);
				}
			}

			string sound;
			if (string.IsNullOrWhiteSpace(arguments.WindowsSound))
			{
				sound = "file:///" + arguments.SoundPath;
			}
			else
			{
				sound = $"ms-winsoundevent:{arguments.WindowsSound}";
			}

			var attribute = toastXml.CreateAttribute("src");
			attribute.Value = sound;
			audio.Attributes.SetNamedItem(attribute);

			attribute = toastXml.CreateAttribute("loop");
			attribute.Value = arguments.Loop;
		}

		/// <summary>
		/// Set the visual attribute.
		/// </summary>
		/// <param name="toastXml">XML object used to add the visual attribute to.</param>
		private static void SetVisualAttribute(XmlDocument toastXml)
		{
			var visual = toastXml.GetElementsByTagName("visual").FirstOrDefault();

			var attribute = toastXml.CreateAttribute("branding");
			attribute.Value = "none";
			visual.Attributes.SetNamedItem(attribute);
			Debug.WriteLine(toastXml.GetXml());
		}

		/// <summary>
		/// Set the branding attribute.
		/// </summary>
		/// <param name="toastXml">XML object used to add the branding attribute to.</param>
		private static void SetBrandingAttribute(XmlDocument toastXml)
		{
			var visual = toastXml.GetElementsByTagName("binding").FirstOrDefault();

			var attribute = toastXml.CreateAttribute("branding");
			attribute.Value = "logo";
			visual.Attributes.SetNamedItem(attribute);
			Debug.WriteLine(toastXml.GetXml());
		}

		/// <summary>
		/// Set the duration attribute.
		/// </summary>
		/// <param name="toastXml">XML object used to add the duration attribute to.</param>
		/// <param name="duration">Duration (Long or Short.)</param>
		private static void SetDurationAttribute(XmlDocument toastXml, string duration)
		{
			var toast = toastXml.GetElementsByTagName("toast").FirstOrDefault();

			var attribute = toastXml.CreateAttribute("duration");
			attribute.Value = duration;
			toast.Attributes.SetNamedItem(attribute);
		}

		/// <summary>
		/// Set the commands attribute.
		/// </summary>
		/// <param name="toastXml">XML object used to add the commands attribute to.</param>
		private static void SetCommandsAttribute(XmlDocument toastXml)
		{
			var commands = toastXml.GetElementsByTagName("commands").FirstOrDefault();
			XmlElement commandsNode = null;

			if (commands == null)
			{
				commands = toastXml.CreateElement("commands");
				var toastNode = ((XmlElement)toastXml.SelectSingleNode("/toast"));

				if (toastNode != null)
				{
					var attribute = toastXml.CreateAttribute("branding");
					attribute.Value = "none";
					toastNode.Attributes.SetNamedItem(attribute);
					toastNode.AppendChild(commands);

					attribute = toastXml.CreateAttribute("scenario");
					attribute.Value = "incomingCall";
					commands.Attributes.SetNamedItem(attribute);

					var command = toastXml.CreateElement("command");
					commandsNode = ((XmlElement)toastXml.SelectSingleNode("/toast/commands"));

					if (commandsNode != null)
					{
						commandsNode.AppendChild(command);
					}

					attribute = toastXml.CreateAttribute("id");
					attribute.Value = "voice";
					command.Attributes.SetNamedItem(attribute);

					attribute = toastXml.CreateAttribute("arguments");
					attribute.Value = "close-notification";
					command.Attributes.SetNamedItem(attribute);

					command = toastXml.CreateElement("command");
					commandsNode = ((XmlElement)toastXml.SelectSingleNode("/toast/commands"));

					if (commandsNode != null)
					{
						commandsNode.AppendChild(command);
					}

					attribute = toastXml.CreateAttribute("id");
					attribute.Value = "video";
					command.Attributes.SetNamedItem(attribute);

					attribute = toastXml.CreateAttribute("arguments");
					attribute.Value = "snooze-notification";
					command.Attributes.SetNamedItem(attribute);

					command = toastXml.CreateElement("command");
					commandsNode = ((XmlElement)toastXml.SelectSingleNode("/toast/commands"));

					if (commandsNode != null)
					{
						commandsNode.AppendChild(command);
					}

					attribute = toastXml.CreateAttribute("id");
					attribute.Value = "decline";
					command.Attributes.SetNamedItem(attribute);

					attribute = toastXml.CreateAttribute("arguments");
					attribute.Value = "decline-notification";
					command.Attributes.SetNamedItem(attribute);
				}
			}
		}

		/// <summary>
		/// Set the silent attribute.
		/// </summary>
		/// <param name="toastXml">XML object used to add the silent attribute to.</param>
		private static void SetSilentAttribute(XmlDocument toastXml)
		{
			var audio = toastXml.GetElementsByTagName("audio").FirstOrDefault();

			if (audio == null)
			{
				audio = toastXml.CreateElement("audio");
				var toastNode = ((XmlElement)toastXml.SelectSingleNode("/toast"));

				if (toastNode != null)
				{
					toastNode.AppendChild(audio);
				}
			}

			var attribute = toastXml.CreateAttribute("silent");
			attribute.Value = "true";
			audio.Attributes.SetNamedItem(attribute);
		}
	}
}