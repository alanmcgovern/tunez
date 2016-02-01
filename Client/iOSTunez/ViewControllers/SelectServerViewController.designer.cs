// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace iOSTunez
{
	[Register ("SelectServerViewController")]
	partial class SelectServerViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton manualAdd { get; set; }
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITextField manualHostname { get; set; }
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITextField manualPort { get; set; }
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		iOSTunez.ServerTableView serverTableView { get; set; }

		[Action ("manualAdd_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void manualAdd_TouchUpInside (UIKit.UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (manualAdd != null) {
				manualAdd.Dispose ();
				manualAdd = null;
			}

			if (manualHostname != null) {
				manualHostname.Dispose ();
				manualHostname = null;
			}

			if (manualPort != null) {
				manualPort.Dispose ();
				manualPort = null;
			}

			if (serverTableView != null) {
				serverTableView.Dispose ();
				serverTableView = null;
			}
		}
	}
}