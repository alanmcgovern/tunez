using System;
using System.Collections.Generic;
using System.IO;

namespace Tunez
{
	public static class LoggingService
	{
		static List<TextWriter> outputs = new List<TextWriter> ();

		static LoggingService ()
		{
			// Add some event handlers for unhandled exceptions
			AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
				LogError (e.ExceptionObject as Exception, "AppDomain unhandled exception");
			};

			System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (sender, e) => {
				LogError (e.Exception, "TaskScheduler unhandled exception");
			};
			outputs.Add (TextWriter.Synchronized (Console.Out));
		}

		public static void AppendOut (string logDirectory)
		{
			// First make sure the log directory exists
			Directory.CreateDirectory (logDirectory);

			// Delete all stale files
			foreach (var file in Directory.GetFiles (logDirectory)) {
				if (File.GetCreationTimeUtc (file) < DateTime.UtcNow.Subtract (TimeSpan.FromDays (7)))
					Files.Delete (file);
			}

			// Prepare the log file for this session
			var logfile = Path.Combine (logDirectory, string.Format ("{0}.{1}.log", "TunezClient", DateTime.UtcNow.ToString ("yyyy-MM-dd__HH-mm-ss")));
			SetOut (new StreamWriter (File.OpenWrite (logfile)) { AutoFlush = true });
		}

		static void SetOut (TextWriter writer)
		{
			lock (outputs)
				outputs.Add (TextWriter.Synchronized (writer));
		}

		public static void LogInfo (string message, params object[] formatting)
		{
			Log ("info", string.Format (message, formatting));
		}

		public static void LogError (Exception exception, string message, params object[] formatting)
		{
			message = string.Format (message, formatting);
			Log ("error", string.Format ("{0}{1}{2}", message, Environment.NewLine, exception));
		}

		static void Log (string prefix, string message)
		{
			var result = string.Format ("[{0}] {1}: {2}", DateTime.Now, prefix, message);
			foreach (var output in outputs)
				output.WriteLine (result);
		}
	}
}
