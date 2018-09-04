using AppVNext.Notifier.Common;
using DesktopNotifications;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

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

			//Set the attribution text
			if (!string.IsNullOrWhiteSpace(arguments.AttributionText))
			{
				visual.BindingGeneric.Attribution = new ToastGenericAttributionText()
				{
					Text = arguments.AttributionText
				};
			}

			//Set the logo override
			var imagePath = Globals.GetImageOrDefault(arguments.PicturePath);
			var isInternetImage = IsInternetImage(imagePath);
			var imageSource = isInternetImage ? DownloadImage(imagePath) : imagePath;

			visual.BindingGeneric.AppLogoOverride = new ToastGenericAppLogo()
			{
				Source = imageSource,
				HintCrop = ToastGenericAppLogoCrop.Circle
			};

			//Set a background image
			if (!string.IsNullOrWhiteSpace(arguments.Image))
			{
				isInternetImage = IsInternetImage(arguments.Image);
				imageSource = isInternetImage ? DownloadImage(arguments.Image) : arguments.Image;

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

		private static bool isCacheCleared;
		private static string DownloadImage(string imageUrl)
		{
			try
			{
				// Ignore if image URL is not served using SSL.
				var imageUri = new Uri(imageUrl);
				if (imageUri.Scheme.ToLower() != "https")
				{
					return string.Empty;
				}

				// Ignore if image is not hosted .
				if (imageUri.Host.IndexOf(Constants.ImageValidHost, StringComparison.CurrentCultureIgnoreCase) == -1)
				{
					return string.Empty;
				}

				// Return image URL if they are allowed.
				if (DesktopNotificationManagerCompat.CanUseHttpImages)
				{
					return imageUrl;
				}

				// Clear cache if it hasn't been cleared.
				var imagesDirectory = Directory.CreateDirectory(Path.GetTempPath() + Constants.ImagesFolder);

				if (!isCacheCleared)
				{
					isCacheCleared = true;

					foreach (var directory in imagesDirectory.EnumerateDirectories())
					{
						if (directory.CreationTimeUtc.Date < DateTime.UtcNow.Date.AddDays(-Constants.MaximumDays))
						{
							directory.Delete(true);
						}
					}
				}

				// Return image from the cache if it exist.
				var dayDirectory = imagesDirectory.CreateSubdirectory(DateTime.UtcNow.Day.ToString());
				var imagePath = dayDirectory.FullName + "\\" + (uint)imageUrl.GetHashCode();

				if (File.Exists(imagePath))
				{
					ValidateImage(ref imagePath);
					return imagePath;
				}

				// Download image to cache.
				var webClient = new WebClient();
				webClient.DownloadFile(imageUri, imagePath);

				ValidateImage(ref imagePath);
				return imagePath;
			}
			catch
			{
				// Ignore image if any error occurred.
				return string.Empty;
			}
		}

		private static void ValidateImage(ref string imagePath)
		{
			try
			{
				using (var image = Image.FromFile(imagePath))
				{ }
			}
			catch (OutOfMemoryException)
			{
				// If the file is not an image or GDI+ does not support the pixel format of the file
				// a OutOfMemoryException will be thrown and the file will be ignored.
				imagePath = string.Empty;
			}
		}
	}
}