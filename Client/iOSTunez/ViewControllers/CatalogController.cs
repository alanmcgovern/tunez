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
	public partial class CatalogController : UINavigationController
	{
		public Catalog Catalog {
			get; set;
		}

		public PlayQueue<StreamingPlayer> PlayQueue {
			get; set;
		}

		public CatalogController (IntPtr handle) : base (handle)
		{
		}
	}
}