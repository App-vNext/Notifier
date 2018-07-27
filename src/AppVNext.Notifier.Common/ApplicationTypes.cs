namespace AppVNext.Notifier.Common
{
	/// <summary>
	/// Used to differentiate between Windows Desktop and UWP applications.
	/// </summary>
	public enum ApplicationTypes
	{
		/// <summary>
		/// Not known
		/// </summary>
		Unknown = -1,
		/// <summary>
		/// Windows Desktop Application ()
		/// </summary>
		WindowsDesktop = 0,
		/// <summary>
		/// UWP Application
		/// </summary>
		UniversalWindowsPlatform = 1
	}
}