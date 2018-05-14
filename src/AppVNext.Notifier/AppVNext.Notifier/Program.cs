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
			if (arguments.AreValid())
			{
				SendNotification(arguments);
			}
			else
			{
				WriteLine($"{Globals.NewLine}The message argument is required.{Globals.NewLine}");
			}

#if DEBUG
			WriteLine("Hit Enter to exit.");
			ReadLine();
#endif
		}

		private static void SendNotification(NotificationArguments arguments)
		{
			if (arguments.ApplicationId == Globals.DefaultApplicationId)
			{
				ShortcutHelper.CreateShortcutIfNeeded(arguments.ApplicationId, arguments.ApplicationName);
			}
			Notifier.ShowToast(arguments);
		}

		private static NotificationArguments ProcessArguments(string[] args)
		{
			var arguments = new NotificationArguments();
			if (args.Length == 0)
			{
				DisplayHelp();
				return null;
			}
			else if (args.Length == 1)
			{
				var normalizedArgument = NormalizeArgument(args[0]);
				if (normalizedArgument == "?" || normalizedArgument == "h" || normalizedArgument == "help")
				{
					DisplayHelp();
				}
				else
				{
					arguments.Message = args[0];
				}
			}
			else
			{
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
								WriteLine(Globals.HelpForRegister);
								Exit(-1);
							}
							ShortcutHelper.CreateShortcutIfNeeded(arguments.ApplicationId, arguments.ApplicationName);
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
								WriteLine(Globals.HelpForTitle);
								Exit(-1);
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
								WriteLine(Globals.HelpForMessage);
								Exit(-1);
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
								WriteLine(Globals.HelpForPicture);
								Exit(-1);
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
								WriteLine(Globals.HelpForId);
								Exit(-1);
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
								WriteLine(Globals.HelpForSound);
								Exit(-1);
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
								WriteLine(Globals.HelpForAppId);
								Exit(-1);
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
								WriteLine(Globals.HelpForClose);
								Exit(-1);
							}
							break;

						default:
							WriteLine(string.Format(Globals.HelpForInvalidArgument, args[i]));
							Exit(-1);
							break;
					}
				}
			}

			return arguments;
		}

		/// <summary>
		/// Removes dash (-) and forward slash (/) from the beginning and converts the argument to lower case.
		/// </summary>
		/// <param name="argument"></param>
		/// <returns>Normilized arguments.</returns>
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
