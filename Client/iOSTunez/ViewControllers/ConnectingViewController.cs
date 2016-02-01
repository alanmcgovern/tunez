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
	public partial class ConnectingViewController : UIViewController
	{
		bool isConnecting;
		string loadingMessage = "";

		public string LoadingMessage {
			get { return loadingMessage; }
			set {
				loadingMessage = value;
				if (loadingLabel != null)
					loadingLabel.Text = value;
			}
		}

		public bool IsConnecting {
			get { return isConnecting; }
			set {
				isConnecting = value;
				if (loadingSpinner != null) {
					loadingSpinner.Hidden = !value;
					if (value)
						loadingSpinner.StartAnimating ();
					else
						loadingSpinner.StopAnimating ();
				}
			}
		}

		public ConnectingViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			IsConnecting = isConnecting;
			LoadingMessage = loadingMessage;
		}
	}
}