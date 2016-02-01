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
    [Register ("NowPlayingStatusView")]
    partial class NowPlayingStatusView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ArtistLabel { get; set; }
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIActivityIndicatorView bufferingIndicator { get; set; }
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel bufferingLabel { get; set; }
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton PlayPauseButton { get; set; }
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TrackLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ArtistLabel != null) {
                ArtistLabel.Dispose ();
                ArtistLabel = null;
            }

            if (bufferingIndicator != null) {
                bufferingIndicator.Dispose ();
                bufferingIndicator = null;
            }

            if (bufferingLabel != null) {
                bufferingLabel.Dispose ();
                bufferingLabel = null;
            }

            if (PlayPauseButton != null) {
                PlayPauseButton.Dispose ();
                PlayPauseButton = null;
            }

            if (TrackLabel != null) {
                TrackLabel.Dispose ();
                TrackLabel = null;
            }
        }
    }
}