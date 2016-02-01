using System;
using Tunez;

using CoreGraphics;
using UIKit;

namespace iOSTunez
{
	public class NowPlayingMonitor : TrackMonitor
	{
		UIView SidebarView {
			get;
		}

		NowPlayingStatusView StatusView {
			get;
		}

		UIView Superview {
			get;
		}

		public NowPlayingMonitor (UIView superview, UIView sidebarView, NowPlayingStatusView view)
		{
			SidebarView = sidebarView;
			Superview = superview;
			StatusView = view;
		}

		public override void Initialize (Track track)
		{
			base.Initialize(track);

			if (track == null) {
				SidebarView.Frame = SidebarView.Frame.Expand (0, 0, 0, StatusView.IntrinsicContentSize.Height);
				StatusView.RemoveFromSuperview ();
			} else {
				if (StatusView.Superview == null) {
					SidebarView.Frame = SidebarView.Frame.Expand (0, 0, 0, -StatusView.IntrinsicContentSize.Height);
					StatusView.Layer.ShadowOpacity = 0.5f;
					StatusView.Layer.ShadowColor = UIColor.Black.CGColor;
					StatusView.TranslatesAutoresizingMaskIntoConstraints = false;
					Superview.AddSubview (StatusView);
					Superview.AddConstraints (new [] {
						NSLayoutConstraint.Create (StatusView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, Superview, NSLayoutAttribute.Bottom, 1, 0),
						NSLayoutConstraint.Create (StatusView, NSLayoutAttribute.Left, NSLayoutRelation.Equal, Superview, NSLayoutAttribute.Left, 1, 0),
						NSLayoutConstraint.Create (StatusView, NSLayoutAttribute.Right, NSLayoutRelation.Equal, Superview, NSLayoutAttribute.Right, 1, 0),
					});
				}
				StatusView.Track = track.Name;
				StatusView.Artist = track.TrackArtist;
			}
		}
	}
}

