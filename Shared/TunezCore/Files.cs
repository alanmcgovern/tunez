using System;
namespace Tunez
{
	public static class Files
	{
		public static void Delete (string path)
		{
			try {
				if (System.IO.File.Exists (path))
				    System.IO.File.Delete (path);
			} catch (Exception ex) {
				LoggingService.LogError (ex, "Could not delete '{0}'", path);
			}
		}
	}
}

