using System;
using Windows.UI.Notifications;
using static System.Environment;
using static System.Console;
using System.Collections.Generic;
using System.Reflection;
using AppVNext.Notifier.Common;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Microsoft.QueryStringDotNET;

namespace AppVNext.Notifier
{
	/// <summary>
	/// Used when the Notification has been activated, dismissed or failed.
	/// </summary>
	class NotificationEvents
	{
		internal void OnActivated(IActivatedEventArgs e)
		{
			// Get the root frame
			Frame rootFrame = Window.Current.Content as Frame;

			// TODO: Initialize root frame just like in OnLaunched

			// Handle toast activation
			if (e is ToastNotificationActivatedEventArgs)
			{
				var toastActivationArgs = e as ToastNotificationActivatedEventArgs;

				// Parse the query string (using QueryString.NET)
				QueryString args = QueryString.Parse(toastActivationArgs.Argument);

				// See what action is being requested 
				switch (args["action"])
				{
					// Open the image
					case "viewImage":

						// The URL retrieved from the toast args
						string imageUrl = args["imageUrl"];

						////////// If we're already viewing that image, do nothing
						//////if (rootFrame.Content is ImagePage && (rootFrame.Content as ImagePage).ImageUrl.Equals(imageUrl))
						//////	break;

						////////// Otherwise navigate to view it
						////////rootFrame.Navigate(typeof(ImagePage), imageUrl);
						break;


					// Open the conversation
					case "viewConversation":

						// The conversation ID retrieved from the toast args
						int conversationId = int.Parse(args["conversationId"]);

						////////// If we're already viewing that conversation, do nothing
						////////if (rootFrame.Content is ConversationPage && (rootFrame.Content as ConversationPage).ConversationId == conversationId)
						////////	break;

						////////// Otherwise navigate to view it
						////////rootFrame.Navigate(typeof(ConversationPage), conversationId);
						break;
				}

				//////////// If we're loading the app for the first time, place the main page on
				//////////// the back stack so that user can go back after they've been
				//////////// navigated to the specific page
				//////////if (rootFrame.BackStack.Count == 0)
				//////////	rootFrame.BackStack.Add(new PageStackEntry(typeof(MainPage), null, null));
			}

			// TODO: Handle other types of activation

			// Ensure the current window is active
			Window.Current.Activate();
		}


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
					WriteLine("The user dismissed this toast");
					Exit((int)DismissalActions.Dismissed);
					break;
				case ToastDismissalReason.TimedOut:
					WriteLine("The toast has timed out");
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
