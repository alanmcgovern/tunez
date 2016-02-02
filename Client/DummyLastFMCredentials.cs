namespace Tunez
{
	/// <summary>
	/// This is only included in the build when no 'lastfm.cs' file
	/// is found. This declares the necessary properties but does not
	/// initialize them, thus allowing compilation to succeed but the
	/// app will not be able to report anything to last.fm.
	/// </summary>
	public static class LastFMAppCredentials
	{
		public const string ApplicationName = null;
		public const string APIKey = null;
		public const string SharedSecret = null;
		public const string RegisteredTo = null;
	}
}

