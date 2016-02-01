using System;
using Tunez;
using UIKit;

namespace iOSTunez
{
	public static class StoryboardHelper
	{
		public static class Main
		{
			static UIStoryboard MainStoryboard = UIStoryboard.FromName ("Main", null);

			public static ConnectingViewController CreateConnectingViewController ()
			{
				return (ConnectingViewController) MainStoryboard.InstantiateViewController ("ConnectingViewController");
			}

			public static CatalogController CreateCatalogController (Catalog catalog, PlayQueue<StreamingPlayer> playQueue)
			{
				var vc = (CatalogController) MainStoryboard.InstantiateViewController ("CatalogController");
				vc.Catalog = catalog;
				vc.PlayQueue = playQueue;
				return vc;
			}

			public static LastFMLoginController CreateLastFMLoginController (Scrobbler scrobbler)
			{
				var vc = (LastFMLoginController) MainStoryboard.InstantiateViewController ("LastFMLoginController");
				vc.Scrobbler = scrobbler;
				return vc;
			}

			public static NavigationMenuController CreateNavigationMenuController ()
			{
				return (NavigationMenuController) MainStoryboard.InstantiateViewController ("NavigationMenuController");
			}

			public static SelectServerViewController CreateSelectServerViewController ()
			{
				return (SelectServerViewController) MainStoryboard.InstantiateViewController ("SelectServerViewController");
			}

		}
	}
}

