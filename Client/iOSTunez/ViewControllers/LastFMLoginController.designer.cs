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
    [Register ("LastFMLoginController")]
    partial class LastFMLoginController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel InvalidLoginLabel { get; set; }
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton LoginButton { get; set; }
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField PasswordEntry { get; set; }
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField UsernameEntry { get; set; }

        [Action ("LoginButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void LoginButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (InvalidLoginLabel != null) {
                InvalidLoginLabel.Dispose ();
                InvalidLoginLabel = null;
            }

            if (LoginButton != null) {
                LoginButton.Dispose ();
                LoginButton = null;
            }

            if (PasswordEntry != null) {
                PasswordEntry.Dispose ();
                PasswordEntry = null;
            }

            if (UsernameEntry != null) {
                UsernameEntry.Dispose ();
                UsernameEntry = null;
            }
        }
    }
}