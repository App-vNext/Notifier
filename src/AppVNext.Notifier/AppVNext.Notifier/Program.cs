using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;
using static System.Console;
using static System.Environment;

namespace AppVNext.Notifier
{
	class Program
	{
		static void Main(string[] args)
		{
			var arguments = ProcessArguments(args);

			if (arguments == null)
			{
				WriteLine($"{Globals.HelpForNullMessage}{Globals.HelpForErrors}");
				DisplayHelp();
			}
			else
			{
				if (arguments.Register)
				{
					if (ShortcutHelper.CreateShortcutIfNeeded(arguments.ApplicationId, arguments.ApplicationName))
					{
						WriteLine(string.Format(Globals.HelpForRegisterSuccess, arguments.ApplicationId, arguments.ApplicationName));
					}	
					else
					{
						WriteLine(string.Format(Globals.HelpForRegisterFail, arguments.ApplicationId, arguments.ApplicationName));
					}
				}

				if (arguments.NotificationsCheck)
				{
					WriteLine(RegistryHelper.AreNotificationsEnabled(arguments.NotificationCheckAppId));
				}

				if (arguments.PushNotificationCheck)
				{
					WriteLine(RegistryHelper.ArePushNotificationsEnabled());
				}

				if (string.IsNullOrEmpty(arguments.Errors) && !string.IsNullOrEmpty(arguments.Message))
				{
					SendNotification(arguments);
					while (arguments.Wait) { System.Threading.Thread.Sleep(500); }
				}
				else 
				{
					WriteLine($"{(arguments.Errors ?? string.Empty)}");
				}
			}
		}

		private static void SendNotification(NotificationArguments arguments)
		{
			if (arguments.ApplicationId == Globals.DefaultApplicationId)
			{
				ShortcutHelper.CreateShortcutIfNeeded(arguments.ApplicationId, arguments.ApplicationName);
			}
			var toast = Notifier.ShowToast(arguments);
		}

		private static NotificationArguments ProcessArguments(string[] args)
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
		/// <param name="argument"></param>
		/// <returns>Normalized arguments.</returns>
		private static string NormalizeArgument(string argument)
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

		private static void DisplayHelp()
		{
			WriteLine(Globals.HelpText);
		}
	}
}
