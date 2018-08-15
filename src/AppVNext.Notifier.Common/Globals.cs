using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Environment;

namespace AppVNext.Notifier.Common
{
	/// <summary>
	/// Global variables and constants
	/// </summary>
	public class Globals
	{
		/// <summary>
		/// Used to differentiate between the Windows Desktop and UWP application types as not 
		/// all features apply to both.
		/// </summary>
		public static ApplicationTypes ApplicationType { get; set; }

		public static string GetVersionInformation()
		{
			var assembly = Assembly.GetEntryAssembly();
			var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
			return $"{versionInfo.FileDescription}{NewLine}" +
					$"{versionInfo.Comments}{NewLine}" +
					$"{versionInfo.InternalName}. Version {versionInfo.FileVersion} ({ApplicationType.ToString()}){NewLine}" +
					$"{versionInfo.CompanyName}{NewLine}" +
					$"{versionInfo.LegalCopyright}";
		}

		public static string GetImageOrDefault(string picturePath)
		{
			return string.IsNullOrWhiteSpace(picturePath) ? GetDefaultIcon() : "file:///" + picturePath;
		}

		public static bool IsWindowsDesktopApp { get { return ApplicationType == ApplicationTypes.WindowsDesktop; } }
		public static bool IsUwpApp { get { return ApplicationType == ApplicationTypes.UwpNative || ApplicationType == ApplicationTypes.UwpConsole; } }

		public static string GetDefaultIcon()
		{
			string icon = null;
			switch (ApplicationType)
			{
				case ApplicationTypes.WindowsDesktop:
				case ApplicationTypes.UwpConsole:
					icon = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Icon.ico");					
					break;
				case ApplicationTypes.UwpNative:
					icon = "ms-appx:///Assets/Icon.ico";
					break;
			}

			return icon;
		}

		//General
		public static readonly string NewLine = Environment.NewLine;
		public const string DefaultApplicationId = "com.appvnext.windows-notifier";
		public const string DefaultApplicationName = "appvnext-windows-notifier";
		public const string NotificationKey = "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Notifications\\Settings\\{0}";
		public const string PushNotificationKey = "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\PushNotifications";

		//Arguments help text
		public static readonly string HelpForRegister =
			$"Argument -r requires 2 values: <appId string> and <appName string>.{NewLine}" +
			$"Example: -r \"com.appvnext.windows-notifier\" \"appvnext-windows-notifier\"{NewLine}";

		public static readonly string HelpForTitle =
			$"Argument -t requires 1 value: <title string>.{NewLine}" +
			$"Example: -t \"Notification Title\"{NewLine}";

		public static readonly string HelpForMessage =
			$"Argument -m requires 1 value: <message string>.{NewLine}" +
			$"Example: -m \"Notification message.\"{NewLine}";

		public static readonly string HelpForPicture =
			$"Argument -p requires 1 value: <image URI>.{NewLine}" +
			$"Example: -p \"C:\\Pictures\\Picture.png\"{NewLine}";

		public static readonly string HelpForId =
			$"Argument -id requires 1 value: <id string>.{NewLine}" +
			$"Example: -id \"12345\"{NewLine}";

		public static readonly string HelpForSound =
			$"Argument -s requires 1 value: <title string>.{NewLine}" +
			$"Example: -s \"C:\\Sounds\\Sound.wav\"{NewLine}";

		public static readonly string HelpForAppId =
			$"Argument -appID requires 1 value: <appID string>.{NewLine}" +
			$"Example: -appID \"com.appvnext.windows-notifier\"{NewLine}";

		public static readonly string HelpForButtons =
			$"Argument -b requires 1 value: <json string>.{NewLine}" +
			$"Example: -b \"[{{'id':'button1', 'text':'Button 1'}}, {{'id':'button2','text':'Button 2'}}]\"{NewLine}";

		public static readonly string HelpForInputs =
			$"Argument -i requires 1 value: <json string>.{NewLine}" +
			$"Example: -i \"[{{'id':'input1', 'title':'Input 1', 'placeholdertext':'Enter value'}}, {{'id':'input2', 'title':'Input 2', 'placeholdertext':'Enter value'}}]\"{NewLine}";

		public static readonly string HelpForClose =
			$"Argument -close requires 1 value: <ID string>.{NewLine}" +
			$"Example: -close \"12345\"{NewLine}";

		public static readonly string HelpForNotificationsCheck =
			$"Argument -n requires 1 value: <appID string>.{NewLine}" +
			$"Example: -n \"com.appvnext.windows-notifier\"{NewLine}";

		public static readonly string HelpForDuration =
			$"Argument -d requires 1 value: <short|long>.{NewLine}" +
			$"Example: -d \"long\"{NewLine}";

		public static readonly string HelpForWallpaper =
			$"Argument -l requires 1 value: <image URI>.{NewLine}" +
			$"Example: -l \"C:\\Pictures\\Picture.png\"{NewLine}";

		public static readonly string HelpForInvalidArgument =
			$"Argument '{{0}}' is invalid.{NewLine}";

		public static readonly string HelpForNullMessage =
			$"At least argument -m <message string> is required.{NewLine}" +
			$"Example: -m \"Notification message.\"{NewLine}";

		public static readonly string HelpForErrors = 
			$"{NewLine}Please type 'notifier help' to show the valid arguments.{NewLine}";

		public static readonly string HelpForRegisterSuccess =
			$"The application shortcut for '{{0}}' '{{1}}' was successfully created.{NewLine}";

		public static readonly string HelpForRegisterFail =
			$"The application shortcut for '{{0}}' '{{1}}' already existed.{NewLine}";

		public static readonly string HelpForAttributionText =
			$"Argument -a requires 1 value: <text string>.{NewLine}" +
			$"Example: -a \"Via SMS\"{NewLine}";

		//Help text
		public static string GetHelpText()
		{
			return
			$"Create a send notifications.{NewLine}{NewLine}" +
			$"Usage: notifier <command>{NewLine}{NewLine}" +
			$"Commands:{NewLine}{NewLine}" +
			(IsWindowsDesktopApp ?
			$"[-r] <appId string><appName string>	Registers notifier into the Windows machine.{NewLine}" : string.Empty) +
			$"[-t] <title string>			Title is displayed on the first line of the notification.{NewLine}" +
			$"[-m] <message string>			Message is displayed wrapped below the title of the notification.{NewLine}" +
			$"[-p] <image URI>			URI for a picture file to be displayed with the notification. {NewLine}" +
			$"[-w]					Wait for notification to expire or activate.{NewLine}" +
			$"[-id] <id string>			Sets the ID of the toast notification to be able to close it.{NewLine}" +
			$"[-s] [<sound URI>][<Windows sound>]	URI for a sound file or Windows sound to play when the notification displays.{NewLine}" +
			$"					For possible Windows sound values visit http://msdn.microsoft.com/en-us/library/windows/apps/hh761492.aspx.{NewLine}" +
			$"[-silent]				Does not play a sound when showing the notification.{NewLine}" +
			(IsWindowsDesktopApp ?
			$"[-d] <short|long>			Determines how long to display the notification for. Default is 'short'.{NewLine}" : string.Empty) +
			$"[-appID] <appID string>			Used to display the notification.{NewLine}" +
			(IsUwpApp ?
			$"[-l] <image URI>			URI for a picture file to be displayed with the notification.{NewLine}" : string.Empty) +
			$"[-n] <appID string>			Returns Notifications setting status for the application. Return values: Enabled, Disabled or Unknown.{NewLine}" +
			$"[-k]					Returns Notifications setting status for the system. Return values: Enabled or Disabled.{NewLine}" +
			(IsUwpApp ?
			$"[-b] <ID string, Text string>		Display buttons.{NewLine}" : string.Empty) +
			(IsUwpApp ?
			$"[-i] <ID string, Text string,{NewLine}      Place Holder Text string>		Display inputs.{NewLine}" : string.Empty) +
			$"[-close] <ID string>			Closes notification. In order to be able to close a notification,{NewLine}" +
			$"					the parameter -w must be used to create the notification.{NewLine}" +
			(IsUwpApp ?
			$"[-a] <text string>			Attribution text is displayed at the bottom of the notification.{NewLine}" : string.Empty) +
			$"[-v]					Displays version information.{NewLine}" +
			$"[-?]					Displays this help.{NewLine}" +
			$"[-help]					Displays this help.{NewLine}{NewLine}" +
			$"Exit Codes:				Failed -1, Success 0, Close 1, Dismiss 2, Timeout 3.{NewLine}{NewLine}" +
			$"Examples:{NewLine}{NewLine}" +
			$"notifier -t \"Hello World!\"{NewLine}" +
			$"notifier -t \"Notification Title\" -m \"Notification message.\"{NewLine}" +
			$"notifier help{NewLine}" +
			$"notifier ?{NewLine}" +
			(IsUwpApp ?
			$"notifier register com.appvnext.windows-notifier appvnext-windows-notifier{NewLine}" : string.Empty) +
			$"{NewLine}";
		}
	}
}