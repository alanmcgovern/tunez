using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using Tunez;

namespace iOSTunez
{
	partial class TrackCell : UITableViewCell
	{
		Track track;
		public Track Track {
			get { return track; }
			set {
				track = value;
				TextLabel.Text = string.Format ("{0}. {1}", track.Number, track.Name);
			}
		}

		public TrackCell (IntPtr handle) : base (handle)
		{
		}
	}
}
