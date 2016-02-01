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
	public partial class NavigationMenuController : UIViewController
	{
		public event EventHandler CatalogSelected;
		public event EventHandler LastFmSelected;
		public event EventHandler SelectServerSelected;

		public NavigationMenuController (IntPtr handle) : base (handle)
		{
		}

		partial void CatalogButton_TouchUpInside(UIButton sender)
		{
			if (CatalogSelected != null)
				CatalogSelected (this, EventArgs.Empty);
		}

		partial void LastFMButton_TouchUpInside(UIButton sender)
		{
			if (LastFmSelected != null)
				LastFmSelected (this, EventArgs.Empty);
		}

		partial void SelectServerButton_TouchUpInside(UIButton sender)
		{
			if (SelectServerSelected != null)
				SelectServerSelected (this, EventArgs.Empty);
		}
	}
}