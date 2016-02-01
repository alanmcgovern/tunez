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
	[Register ("ServerCell")]
	partial class ServerCell
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel AddressLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (AddressLabel != null) {
				AddressLabel.Dispose ();
				AddressLabel = null;
			}
		}
	}
}