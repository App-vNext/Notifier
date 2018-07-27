namespace AppVNext.Notifier.Common
{
	/// <summary>
	/// Used when determining whether Push Notifications are allowed or not on Windows.
	/// </summary>
	public enum EnableTypes
	{
		/// <summary>
		/// Not known.
		/// </summary>
		Unknown = -1,
		/// <summary>
		/// Push Notifications are not allowed.
		/// </summary>
		Disabled = 0,
		/// <summary>
		/// Push Notifications are allowed.
		/// </summary>
		Enabled = 1
	}
}