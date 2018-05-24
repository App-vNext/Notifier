﻿using System;

namespace AppVNext.Notifier
{
	internal class NotificationArguments
	{
		public string ApplicationId { get; internal set; } = Globals.DefaultApplicationId;
		public string ApplicationName { get; internal set; } = Globals.DefaultApplicationName;
		public string NotificationId { get; internal set; }
		public string Title { get; internal set; }
		public string Message { get; internal set; }
		public string PicturePath { get; internal set; }
		public string WindowsSound { get; internal set; }
		public string Loop { get; internal set; } = "false";
		public string SoundPath { get; internal set; }
		public bool Silent { get; internal set; }
		public bool Wait { get; internal set; }
		public string Errors { get; internal set; }
		public bool Register { get; internal set; }
		public string Duration { get; internal set; }

		internal bool AreValid()
		{
			var isValid = string.IsNullOrWhiteSpace(Errors) 
				&& (string.IsNullOrWhiteSpace(Duration) || !string.IsNullOrWhiteSpace(Duration));
			return isValid; 
		}
	}
}