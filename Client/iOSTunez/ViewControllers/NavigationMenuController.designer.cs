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
	[Register ("NavigationMenuController")]
	partial class NavigationMenuController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton catalogButton { get; set; }
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton lastFMButton { get; set; }
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton selectServerButton { get; set; }

		[Action ("CatalogButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void CatalogButton_TouchUpInside (UIKit.UIButton sender);
		[Action ("LastFMButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void LastFMButton_TouchUpInside (UIKit.UIButton sender);
		[Action ("SelectServerButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void SelectServerButton_TouchUpInside (UIKit.UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (catalogButton != null) {
				catalogButton.Dispose ();
				catalogButton = null;
			}

			if (lastFMButton != null) {
				lastFMButton.Dispose ();
				lastFMButton = null;
			}

			if (selectServerButton != null) {
				selectServerButton.Dispose ();
				selectServerButton = null;
			}
		}
	}
}