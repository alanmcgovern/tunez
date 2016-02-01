// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.Threading;
using System.Threading.Tasks;

using CoreGraphics;
using ObjCRuntime;
using UIKit;

namespace iOSTunez
{
    public partial class NowPlayingStatusView : UIView
    {
		public event EventHandler PlayPausePressed;

		UIImage pauseImage;
		UIImage PauseImage {
			get {
				if (pauseImage == null)
					pauseImage = UIImage.FromBundle ("Pause");
				return pauseImage;
			}
		}

		UIImage playImage;
		UIImage PlayImage {
			get {
				if (playImage == null)
					playImage = UIImage.FromBundle ("Play");
				return playImage;
			}
		}

		public override CGSize IntrinsicContentSize {
			get { return new CGSize (-1, 50); }
		}

		public string Artist {
			get { return ArtistLabel.Text; }
			set { ArtistLabel.Text = value; }
		}

		CancellationTokenSource bufferingCancellation = new CancellationTokenSource ();
		bool isBuffering;
		public bool IsBuffering {
			get { return isBuffering; }
			set {
				if (isBuffering == value)
					return;

				isBuffering = value;
				if (value) {
					Task.Delay (TimeSpan.FromSeconds (2), bufferingCancellation.Token).ContinueWith (t => {
						bufferingLabel.Hidden = false;
						bufferingIndicator.Hidden = false;
						bufferingIndicator.StartAnimating ();
					}, CancellationToken.None, TaskContinuationOptions.NotOnCanceled, TaskScheduler.FromCurrentSynchronizationContext ());
				} else {
					bufferingCancellation.Cancel ();
					bufferingCancellation = new CancellationTokenSource ();
					bufferingLabel.Hidden = true;
					bufferingIndicator.Hidden = true;
					bufferingIndicator.StopAnimating ();
				}
			}
		}

		public bool IsPaused {
			get { return PlayPauseButton.ImageForState (UIControlState.Normal) == PlayImage; }
			set { PlayPauseButton.SetImage (value ? PlayImage : PauseImage, UIControlState.Normal); }
		}

		public string Track {
			get { return TrackLabel.Text; }
			set { TrackLabel.Text = value; }
		}

        public NowPlayingStatusView (IntPtr handle) : base (handle)
        {
        }

		public override void AwakeFromNib()
		{
			bufferingLabel.Hidden = true;
			bufferingIndicator.Hidden = true;
			PlayPauseButton.TouchUpInside += (object sender, EventArgs e) => {
				if (PlayPausePressed != null)
					PlayPausePressed (this, EventArgs.Empty);
			};
		}

		public static NowPlayingStatusView Create ()
		{
			return NSBundle.MainBundle
				           .LoadNib ("NowPlayingStatusView", null, null)
				           .GetItem<NowPlayingStatusView> (0);
		}
    }
}