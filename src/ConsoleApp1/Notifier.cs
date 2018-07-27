using AppVNext.Notifier.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.QueryStringDotNET;
using System.Xml;

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
						},

						new AdaptiveImage()
						{
							Source = arguments.PicturePath
						}
					},

					AppLogoOverride = new ToastGenericAppLogo()
					{
						Source = arguments.PicturePath,
						HintCrop = ToastGenericAppLogoCrop.Circle
					}
				}
			};


			// In a real app, these would be initialized with actual data
			int conversationId = 384928;

			// Construct the actions for the toast (inputs and buttons)
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
						{ "conversationId", conversationId.ToString() }

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
						{ "conversationId", conversationId.ToString() }

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
				Actions = actions,

				// Arguments when the user taps body of toast
				Launch = new QueryString()
				{
					{ "action", "viewConversation" },
					{ "conversationId", conversationId.ToString() }

				}.ToString()
						};

			// And create the toast notification
			var f = new Windows.Data.Xml.Dom.XmlDocument();
			f.LoadXml(toastContent.GetContent());

			var toast = new ToastNotification(f);

			var events = new NotificationEvents();

			toast.Activated += events.Activated;
			toast.Dismissed += events.Dismissed;
			toast.Failed += events.Failed;


			ToastNotificationManager.CreateToastNotifier().Show(toast);

			return toast;
		}
	}
}
