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

using Tunez;

namespace iOSTunez
{
	public partial class ServerCell : UITableViewCell
	{
		ServerDetails server;
		public ServerDetails Server {
			get { return server; }
			set {
				server = value;
				AddressLabel.Text = server.FullAddress;
				Accessory = server.Default ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
			}
		}

		public ServerCell (IntPtr handle) : base (handle)
		{
		}
	}
}