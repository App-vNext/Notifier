using AppVNext.Notifier.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.QueryStringDotNET;
using Windows.Data.Xml.Dom;
using DesktopNotifications;
using System.IO;

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
		public static async Task<ToastNotification> ShowToast(NotificationArguments arguments)
		{
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
			
			//Set the logo override
			var imagePath = Globals.GetImageOrDefault(arguments.PicturePath);
			var isInternetImage = IsInternetImage(imagePath);
			var imageSource = isInternetImage ? await DownloadImageToDisk(imagePath) : imagePath;

			visual.BindingGeneric.AppLogoOverride = new ToastGenericAppLogo()
			{
				Source = imageSource,
				HintCrop = ToastGenericAppLogoCrop.Circle
			};

			//Set a background image
			if (!string.IsNullOrWhiteSpace(arguments.Image))
			{
				isInternetImage = IsInternetImage(arguments.Image);
				imageSource = isInternetImage ? await DownloadImageToDisk(arguments.Image) : arguments.Image;

				visual.BindingGeneric.Children.Add(new AdaptiveImage()
				{
					Source = imageSource
				});
			}

			// Construct the actions for the toast (inputs and buttons)
			var actions = new ToastActionsCustom();

			// Add any inputs
			if (arguments.Inputs != null)
			{
				foreach (var input in arguments.Inputs)
				{
					var textBox = new ToastTextBox(input.Id)
					{
						PlaceholderContent = input.PlaceHolderText
					};

					if (!string.IsNullOrWhiteSpace(input.Title))
					{
						textBox.Title = input.Title;
					}
					actions.Inputs.Add(textBox);
				}
			}

			// Add any buttons
			if (arguments.Buttons != null)
			{
				foreach (var button in arguments.Buttons)
				{
					actions.Buttons.Add(new ToastButton(button.Text, button.Arguments));

					//Background activation is not needed the COM activator decides whether
					//to process in background or launch foreground window
					//actions.Buttons.Add(new ToastButton(button.Text, button.Arguments)
					//{
					//	ActivationType = ToastActivationType.Background
					//});
				}
			}

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
				Actions = actions,
				Audio = audio
			};

			// Create notification
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(toastContent.GetContent());

			var toast = new ToastNotification(xmlDocument);

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
			DesktopNotificationManagerCompat.CreateToastNotifier().Show(toast);

			return toast;
		}

		private static bool IsInternetImage(string image)
		{
			return image.ToLower().StartsWith("http") || image.ToLower().StartsWith("https");
		}

		private static bool _hasPerformedCleanup;
		private static async Task<string> DownloadImageToDisk(string httpImage)
		{
			try
			{
				if (DesktopNotificationManagerCompat.CanUseHttpImages)
				{
					return httpImage;
				}

				var directory = Directory.CreateDirectory(Path.GetTempPath() + "AppVNextNotifierImages");

				if (!_hasPerformedCleanup)
				{
					_hasPerformedCleanup = true;

					foreach (var d in directory.EnumerateDirectories())
					{
						if (d.CreationTimeUtc.Date < DateTime.UtcNow.Date.AddDays(-3))
						{
							d.Delete(true);
						}
					}
				}

				var dayDirectory = directory.CreateSubdirectory(DateTime.UtcNow.Day.ToString());
				string imagePath = dayDirectory.FullName + "\\" + (uint)httpImage.GetHashCode();

				if (File.Exists(imagePath))
				{
					return imagePath;
				}

				System.Net.Http.HttpClient c = new System.Net.Http.HttpClient();
				using (var stream = await c.GetStreamAsync(httpImage))
				{
					using (var fileStream = File.OpenWrite(imagePath))
					{
						stream.CopyTo(fileStream);
					}
				}

				return imagePath;
			}
			catch { return ""; }
		}
	}
}