using AppKit;
using System;
using System.IO;
using Tunez;

namespace MacTunez
{
	static class MainClass
	{
		static void Main (string[] args)
		{
			LoggingService.AppendOut (Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "Library", "Logs", "tunez"));
			NSApplication.Init ();
			NSApplication.Main (args);
		}
	}
}
