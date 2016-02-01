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
	[Register ("ConnectingViewController")]
	partial class ConnectingViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel loadingLabel { get; set; }
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIActivityIndicatorView loadingSpinner { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (loadingLabel != null) {
				loadingLabel.Dispose ();
				loadingLabel = null;
			}

			if (loadingSpinner != null) {
				loadingSpinner.Dispose ();
				loadingSpinner = null;
			}
		}
	}
}