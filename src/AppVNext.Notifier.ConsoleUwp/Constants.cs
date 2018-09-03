using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppVNext.Notifier
{
	/// <summary>
	/// Local public constants
	/// </summary>
	public class Constants
	{
		/// <summary>
		/// Used to temporarily save files downloaded files.
		/// </summary>
		internal const string ImagesFolder = "BraveAdsNotifierImages";
		internal const string ImageValidHost = "brave.com";
		internal const string DateKey = "HKEY_CURRENT_USER\\Software\\Brave Ads Notifier\\";
		internal const string DateValue = "date";
		internal static Int16 MaximumDays = 28;
	}
}
