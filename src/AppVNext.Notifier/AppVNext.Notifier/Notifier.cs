using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using System.Linq;

namespace AppVNext.Notifier
{
	static class Notifier
	{

		public static void ShowToast(NotificationArguments arguments)
		{
			XmlDocument toastXml;
			if (string.IsNullOrWhiteSpace(arguments.PicturePath))
			{
				if (string.IsNullOrWhiteSpace(arguments.Title))
				{
					toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
					var stringElements = toastXml.GetElementsByTagName("text");
					stringElements[0].AppendChild(toastXml.CreateTextNode(arguments.Message));
				}
				else
				{
					toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
					var stringElements = toastXml.GetElementsByTagName("text");
					stringElements[0].AppendChild(toastXml.CreateTextNode(arguments.Title));
					stringElements[1].AppendChild(toastXml.CreateTextNode(arguments.Message));
				}
			}
			else
			{
				 toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText02);

				var imagePath = "file:///" + arguments.PicturePath;
				var imageElements = toastXml.GetElementsByTagName("image");
				imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;
			}

			if (arguments.Silent)
			{
				SetSilentAttribute(toastXml);
			}

			SetSoundAttribute(arguments, toastXml);

			var toast = new ToastNotification(toastXml);
			var events = new NotificationEvents();

			toast.Activated += events.Activated;
			toast.Dismissed += events.Dismissed;
			toast.Failed += events.Failed;

			ToastNotificationManager.CreateToastNotifier(arguments.ApplicationId).Show(toast);
		}

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
			if (String.IsNullOrWhiteSpace(arguments.WindowsSound))
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