using AppKit;
using Foundation;
using Tunez;

namespace MacTunez
{
	[Register ("AppDelegate")]
	public class AppDelegate : NSApplicationDelegate
	{
		NSWindow MainWindow {
			get; set;
		}

		public AppDelegate ()
		{
		}

		public override bool ApplicationShouldHandleReopen(NSApplication sender, bool hasVisibleWindows)
		{
			// This could be invoked before DidFinishLaunching
			if (MainWindow != null) {
				MainWindow.IsVisible = true;
				MainWindow.MakeKeyAndOrderFront (this);
			}
			return false;
		}

		public override void DidFinishLaunching (NSNotification notification)
		{
			MainWindow = NSApplication.SharedApplication.Windows [0];
		}

		public override void WillTerminate (NSNotification notification)
		{
			// Insert code here to tear down your application
		}
	}
}

