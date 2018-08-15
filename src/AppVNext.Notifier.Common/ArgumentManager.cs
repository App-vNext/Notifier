using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static System.Console;

namespace AppVNext.Notifier.Common
{
	public class ArgumentManager
	{
		/// <summary>
		/// Process arguments, normalize them and validate them.
		/// </summary>
		/// <param name="args">Notification arguments array.</param>
		/// <returns>Notification arguments object.</returns>
		public static NotificationArguments ProcessArguments(string[] args)
		{
			var arguments = new NotificationArguments();
			if (args.Length == 0)
			{
				return null;
			}
			else if (args.Length == 1)
			{
				var normalizedArgument = NormalizeArgument(args[0]);
				if (normalizedArgument == "?" || normalizedArgument == "h" || normalizedArgument == "help")
				{
					DisplayHelp();
					return null;
				}
			}
			var skipLoop = 0;
			for (int i = 0; i < args.Length; i++)
			{
				if (skipLoop > 0)
				{
					skipLoop--;
					continue;
				}
				switch (NormalizeArgument(args[i]))
				{
					//Help
					case "?":
					case "h":
					case "help":
						DisplayHelp();
						break;

					//Register application
					case "r":
					case "register":
						if (Globals.IsWindowsDesktopApp)
						{
							if (i + 2 < args.Length)
							{
								arguments.ApplicationId = args[i + 1];
								arguments.ApplicationName = args[i + 2];
								skipLoop = 2;
							}
							else
							{
								arguments.Errors += Globals.HelpForRegister;
							}
							arguments.Register = true;
						}
						else
						{
							arguments.Errors += string.Format(Globals.HelpForInvalidArgument, args[i]);
						}
						break;

					//Title
					case "t":
					case "title":
						if (i + 1 < args.Length)
						{
							arguments.Title = args[i + 1];
							skipLoop = 1;
						}
						else
						{
							arguments.Errors += Globals.HelpForTitle;
						}
						break;

					//Message
					case "m":
					case "message":
						if (i + 1 < args.Length)
						{
							arguments.Message = args[i + 1];
							skipLoop = 1;
						}
						else
						{
							arguments.Errors += Globals.HelpForMessage;
						}
						break;

					//Picture
					case "p":
					case "picture":
						if (i + 1 < args.Length)
						{
							arguments.PicturePath = Path.GetFullPath(args[i + 1]);
							skipLoop = 1;
						}
						else
						{
							arguments.Errors += Globals.HelpForPicture;
						}
						break;

					//Duration
					case "d":
					case "duration":
						if (Globals.IsWindowsDesktopApp)
						{
							if (i + 1 < args.Length)
							{
								arguments.Duration = args[i + 1];
								if (arguments.Duration.ToLower() != "long" && arguments.Duration.ToLower() != "short")
								{
									arguments.Errors += Globals.HelpForDuration;
								}
								skipLoop = 1;
							}
							else
							{
								arguments.Errors += Globals.HelpForDuration;
							}
						}
						else
						{
							arguments.Errors += string.Format(Globals.HelpForInvalidArgument, args[i]);
						}
						break;

					//Wait
					case "w":
					case "wait":
						arguments.Wait = true;
						break;

					//ID
					case "id":
						if (i + 1 < args.Length)
						{
							arguments.NotificationId = args[i + 1];
							skipLoop = 1;
						}
						else
						{
							arguments.Errors += Globals.HelpForId;
						}
						break;

					//Sound
					case "s":
					case "sound":
						if (i + 1 < args.Length)
						{
							if (args[i + 1].IndexOf("Notification.") > -1)
							{
								arguments.WindowsSound = args[i + 1];
								arguments.Loop = args[i + 1].IndexOf(".Looping.") > -1 ? "true" : "false";
							}
							else
							{
								arguments.SoundPath = Path.GetFullPath(args[i + 1]);
							}
							skipLoop = 1;
						}
						else
						{
							arguments.Errors += Globals.HelpForSound;
						}
						break;

					//Silent
					case "silent":
						arguments.Silent = true;
						break;

					//App ID
					case "appid":
						if (i + 1 < args.Length)
						{
							arguments.ApplicationId = args[i + 1];
							skipLoop = 1;
						}
						else
						{
							arguments.Errors += Globals.HelpForAppId;
						}
						break;

					//Logo
					case "l":
					case "wallpaper":
						if (i + 1 < args.Length)
						{
							arguments.Image = args[i + 1];
							skipLoop = 1;
						}
						else
						{
							arguments.Errors += Globals.HelpForWallpaper;
						}
						break;

					//Buttons
					case "b":
					case "buttons":
						if (Globals.IsUwpApp)
						{
							if (i + 1 < args.Length)
							{
								Button[] buttons = null;
								try
								{
									buttons = JsonConvert.DeserializeObject<Button[]>(args[i + 1]);
									arguments.Buttons = buttons;
								}
								catch (Exception ex)
								{
									arguments.Errors += $"Error: {ex.Message}{Globals.NewLine}{Globals.NewLine}{Globals.HelpForButtons}{Globals.NewLine}";
								}

								skipLoop = 1;
							}
							else
							{
								arguments.Errors += Globals.HelpForButtons;
							}
						}
						else
						{
							arguments.Errors += string.Format(Globals.HelpForInvalidArgument, args[i]);
						}
						break;

					//Inputs
					case "i":
					case "inputs":
						if (Globals.IsUwpApp)
						{
							if (i + 1 < args.Length)
							{
								Input[] inputs = null;
								try
								{
									inputs = JsonConvert.DeserializeObject<Input[]>(args[i + 1]);
									arguments.Inputs = inputs;
								}
								catch (Exception ex)
								{
									arguments.Errors += $"Error: {ex.Message}{Globals.NewLine}{Globals.NewLine}{Globals.HelpForInputs}{Globals.NewLine}";
								}

								skipLoop = 1;
							}
							else
							{
								arguments.Errors += Globals.HelpForButtons;
							}
						}
						else
						{
							arguments.Errors += string.Format(Globals.HelpForInvalidArgument, args[i]);
						}
						break;

					//Close
					case "close":
						if (i + 1 < args.Length)
						{
							arguments.NotificationId = args[i + 1];
							skipLoop = 1;
						}
						else
						{
							arguments.Errors += Globals.HelpForClose;
						}
						break;

					//Notification Status
					case "n":
						if (i + 1 < args.Length)
						{
							arguments.NotificationsCheck = true;
							arguments.NotificationCheckAppId = args[i + 1];
							skipLoop = 1;
						}
						else
						{
							arguments.Errors += Globals.HelpForNotificationsCheck;
						}
						break;

					//Push Notification Status
					case "k":
						arguments.PushNotificationCheck = true;
						break;

					//Attribution Text
					case "a":
						if (i + 1 < args.Length)
						{
							arguments.AttributionText = args[i + 1];
							skipLoop = 1;
						}
						else
						{
							arguments.Errors += Globals.HelpForAttributionText;
						}
						break;

					//Clear Notifications
					case "v":
					case "version":
						arguments.VersionInformation = true;
						break;

					//Version information
					case "c":
					case "clear":
						if (Globals.IsUwpApp)
						{
							arguments.ClearNotifications = true;
						}
						else
						{
							arguments.Errors += string.Format(Globals.HelpForInvalidArgument, args[i]);
						}
						break;

					default:
						arguments.Errors += string.Format(Globals.HelpForInvalidArgument, args[i]);
						break;
				}
			}
			return arguments;
		}

		/// <summary>
		/// Removes dash (-) and forward slash (/) from the beginning and converts the argument to lower case.
		/// </summary>
		/// <param name="argument">Argument to normalize.</param>
		/// <returns>Normalized arguments.</returns>
		public static string NormalizeArgument(string argument)
		{
			if (string.IsNullOrWhiteSpace(argument))
			{
				return null;
			}

			var normalizedArgument = argument;

			if (argument.Substring(0, 1) == "-" || argument.Substring(0, 1) == "/")
			{
				normalizedArgument = argument.Substring(1, argument.Length - 1);
			}

			return normalizedArgument.ToLower();
		}

		public static void DisplayHelp()
		{
			WriteLine(Globals.HelpText);
		}
	}
}
