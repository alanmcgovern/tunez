using System;
using UIKit;

namespace iOSTunez
{
	public class ContentViewController : UIViewController
	{
		UIViewController content;

		public ContentViewController ()
		{
			View = new ContentView ();
		}

		public UIViewController Content {
			get { return content; }
			set {
				if (content != null) {
					content.View.RemoveFromSuperview ();
					content.RemoveFromParentViewController ();
				}
				content = value;
				if (content != null) {
					AddChildViewController (content);
					View.AddSubview (content.View);
					Content.View.Frame = View.Frame;
					Tunez.LoggingService.LogInfo ("Frame: {0}", View.Frame);
				}
			}
		}
	}

	class ContentView : UIView
	{
		public override CoreGraphics.CGRect Frame
		{
			get
			{
				return base.Frame;
			}
			set
			{
				base.Frame = value;
				Tunez.LoggingService.LogInfo ("Setting frame to: {0}", value);
			}
		}
	}
}

