using System;
using System.IO;

using Tunez;
using UIKit;

namespace iOSTunez
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main(string[] args)
		{
			LoggingService.AppendOut (Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments)));

			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main(args, null, "AppDelegate");
		}
	}
}
