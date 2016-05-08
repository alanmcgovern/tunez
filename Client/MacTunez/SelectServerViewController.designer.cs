// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MacTunez
{
	[Register ("SelectServerViewController")]
	partial class SelectServerViewController
	{
		[Outlet]
		AppKit.NSButton AddServerButton { get; set; }

		[Outlet]
		AppKit.NSTextField PortTextField { get; set; }

		[Outlet]
		AppKit.NSTextField ServerAddressTextField { get; set; }

		[Outlet]
		MacTunez.ServerBrowser ServerBrowser { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ServerBrowser != null) {
				ServerBrowser.Dispose ();
				ServerBrowser = null;
			}

			if (ServerAddressTextField != null) {
				ServerAddressTextField.Dispose ();
				ServerAddressTextField = null;
			}

			if (PortTextField != null) {
				PortTextField.Dispose ();
				PortTextField = null;
			}

			if (AddServerButton != null) {
				AddServerButton.Dispose ();
				AddServerButton = null;
			}
		}
	}
}
