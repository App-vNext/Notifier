using System;
using System.Collections.Generic;
using System.Linq;
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

		public static readonly string HelpForClose =
			$"Argument -close requires 1 value: <ID string>.{NewLine}" +
			$"Example: -close \"12345\"{NewLine}";

		public static readonly string HelpForNotificationsCheck =
			$"Argument -n requires 1 value: <appID string>.{NewLine}" +
			$"Example: -n \"com.appvnext.windows-notifier\"{NewLine}";

		public static readonly string HelpForDuration =
			$"Argument -d requires 1 value: <short|long>.{NewLine}" +
			$"Example: -d \"long\"{NewLine}";

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

		//Help text
		public static readonly string HelpText =
			$"Create a send notifications.{NewLine}{NewLine}" +
			$"Usage: notifier <command>{NewLine}{NewLine}" +
			$"Commands:{NewLine}{NewLine}" +
			(ApplicationType == ApplicationTypes.WindowsDesktop ? 
			$"[-r] <appId string><appName string>	Registers notifier into the Windows machine.{NewLine}" : string.Empty) +
			$"[-t] <title string>			Title is displayed on the first line of the notification.{NewLine}" +
			$"[-m] <message string>			Message is displayed wrapped below the title of the notification.{NewLine}" +
			$"[-p] <image URI>			URI for a picture file to be displayed with the notification. Local files only. {NewLine}" +
			$"[-w]					Wait for notification to expire or activate.{NewLine}" +
			$"[-id] <id string>			Sets the ID of the toast notification to be able to close it.{NewLine}" +
			$"[-s] [<sound URI>][<Windows sound>]	URI for a sound file or Windows sound to play when the notification displays.{NewLine}" +
			$"					For possible Windows sound values visit http://msdn.microsoft.com/en-us/library/windows/apps/hh761492.aspx.{NewLine}" +
			$"[-silent]				Does not play a sound when showing the notification.{NewLine}" +
			(ApplicationType == ApplicationTypes.WindowsDesktop ?
			$"[-d] <short|long>			Determines how long to display the notification for. Default is 'short'.{NewLine}" : string.Empty) +
			$"[-appID] <appID string>			Used to display the notification.{NewLine}" +
			$"[-n] <appID string>			Returns Notifications setting status for the application. Return values: Enabled, Disabled or Unknown.{NewLine}" +
			$"[-k]					Returns Notifications setting status for the system. Return values: Enabled or Disabled.{NewLine}" +
			$"[-close] <ID string>			Closes notification. In order to be able to close a notification,{NewLine}" +
			$"					the parameter -w must be used to create the notification.{NewLine}" +
			$"[-?]					Displays this help.{NewLine}" +
			$"[-help]					Displays this help.{NewLine}" +
			$"Exit Codes:				Failed -1, Success 0, Close 1, Dismiss 2, Timeout 3.{NewLine}{NewLine}" +
			$"Examples:{NewLine}{NewLine}" +
			$"notifier -t \"Hello World!\"{NewLine}" +
			$"notifier -t \"Notification Title\" -m \"Notification message.\"{NewLine}" +
			$"notifier help{NewLine}" +
			$"notifier ?{NewLine}" +
			(ApplicationType == ApplicationTypes.WindowsDesktop ?
			$"notifier register com.appvnext.windows-notifier appvnext-windows-notifier{NewLine}" : string.Empty) +
			$"{NewLine}";
	}
}
