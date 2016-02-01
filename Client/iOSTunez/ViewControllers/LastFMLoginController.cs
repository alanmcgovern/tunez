// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.IO;
using System.CodeDom.Compiler;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
using Tunez;

namespace iOSTunez
{
    public partial class LastFMLoginController : UIViewController
    {
		CancellationTokenSource cancellation = new CancellationTokenSource ();
		public Scrobbler Scrobbler {
			get; set;
		}

		public string Username {
			get { return UsernameEntry.Text; }
			set { UsernameEntry.Text = value; }
		}

		public string Password {
			get { return PasswordEntry.Text; }
			set { PasswordEntry.Text = value; }
		}

		public LastFMLoginController (IntPtr handle) : base (handle)
        {
		}

		async partial void LoginButton_TouchUpInside(UIButton sender)
		{
			cancellation.Cancel ();
			cancellation = new CancellationTokenSource ();
			Scrobbler.Username = Username;
			Scrobbler.Password = Password;
			InvalidLoginLabel.Hidden = true;
			await CheckLogin (cancellation.Token).WaitOrCanceled ();
		}

		public async override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			Username = Scrobbler.Username;
			Password = Scrobbler.Password;
			await CheckLogin (cancellation.Token).WaitOrCanceled ();
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
			cancellation.Cancel ();
		}

		async Task CheckLogin (CancellationToken token)
		{
			try {
				if (string.IsNullOrEmpty (Username) || string.IsNullOrEmpty (Password))
					return;

				var result = await Scrobbler.Login ();
				InvalidLoginLabel.Hidden = false;
				if (result) {
					InvalidLoginLabel.TextColor = UIColor.Black;
					InvalidLoginLabel.Text = "Login successful";
				} else {
					InvalidLoginLabel.TextColor = UIColor.Red;
					InvalidLoginLabel.Text = "Could not log in to Last.FM with the specified username/password";
				}
			} catch (OperationCanceledException) {
				InvalidLoginLabel.Hidden = true;
			} catch {
				InvalidLoginLabel.TextColor = UIColor.Red;
				InvalidLoginLabel.Hidden = false;
				InvalidLoginLabel.Text = "Could not contact Last.FM";
			}
		}
	}
}