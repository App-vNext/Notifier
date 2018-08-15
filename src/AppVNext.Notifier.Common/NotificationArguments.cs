using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AppVNext.Notifier.Common
{
	/// <summary>
	/// Used for all possible arguments allowed.
	/// </summary>
	public class NotificationArguments
	{
		public string ApplicationId { get; set; } = Globals.DefaultApplicationId;
		public string ApplicationName { get; set; } = Globals.DefaultApplicationName;
		public string NotificationId { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }
		public string PicturePath { get; set; }
		public string WindowsSound { get; set; }
		public string Loop { get; set; } = "false";
		public string SoundPath { get; set; }
		public bool Silent { get; set; }
		public bool Wait { get; set; }
		public string Errors { get; set; }
		public bool Register { get; set; }
		public string Duration { get; set; }
		public bool NotificationsCheck { get; set; }
		public string NotificationCheckAppId { get; set; }
		public bool PushNotificationCheck { get; set; }
		public Button[] Buttons { get; set; }
		public Input[] Inputs { get; set; }
		public string Image { get; set; }
		public bool ClearNotifications { get; set; }
		public bool VersionInformation { get; set; }
		public string AttributionText { get; set; }

		/// <summary>
		/// Check if the arguments are valid or not.
		/// </summary>
		/// <returns>True if the arguments are valid, false otherwise.</returns>
		public bool AreValid()
		{
			var isValid = string.IsNullOrWhiteSpace(Errors) 
				&& (string.IsNullOrWhiteSpace(Duration) || !string.IsNullOrWhiteSpace(Duration));
			return isValid; 
		}
	}
}