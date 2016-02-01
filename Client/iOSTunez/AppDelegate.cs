using System;
using System.IO;
using Tunez;

using AVFoundation;
using Foundation;
using UIKit;

namespace iOSTunez
{
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		public override UIWindow Window
		{
			get;
			set;
		}

		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			AVAudioSession.SharedInstance ().SetCategory (AVAudioSessionCategory.Playback);
			return true;
		}

		public override void OnResignActivation(UIApplication application)
		{
		}

		public override void DidEnterBackground(UIApplication application)
		{
		}

		public override void WillEnterForeground(UIApplication application)
		{
		}

		public override void OnActivated(UIApplication application)
		{
		}

		public override void WillTerminate(UIApplication application)
		{
		}
	}
}


