using System;
using System.Linq;

using AppKit;
using Foundation;
using Tunez;

namespace MacTunez
{
	public class NSServersItem : NSObject
	{
		public NSServerItem[] Servers {
			get;
		}

		public NSServersItem (ServerDetails[] servers)
		{
			Servers = servers.Select (t => new NSServerItem (t)).ToArray ();
		}
	}

	public class NSServerItem : NSObject
	{
		public override string Description {
			get { return Server.FullAddress; }
		}

		public ServerDetails Server {
			get;
		}

		public NSServerItem (ServerDetails server)
		{
			Server = server;
		}
	}

	public class ServerBrowserDelegate : NSBrowserDelegate
	{
		NSServersItem serversItem;

		public ServerBrowserDelegate ()
		{
			serversItem = new NSServersItem (Caches.LoadKnownServers ());
		}

		public override NSObject RootItemForBrowser (NSBrowser browser)
		{
			return serversItem;
		}

		public override nint CountChildren (NSBrowser browser, Foundation.NSObject item)
		{
			if (item is NSServersItem)
				return ((NSServersItem)item).Servers.Length;
			return 0;
		}

		public override NSObject GetChild (NSBrowser browser, nint index, NSObject item)
		{
			if (item is NSServersItem)
				return ((NSServersItem)item).Servers [(int)index];
			return null;
		}

		public override bool IsLeafItem (NSBrowser browser, NSObject item)
		{
			return item is NSServerItem;
		}

		public override NSObject ObjectValueForItem (NSBrowser browser, NSObject item)
		{
			return (NSString) item.Description;
		}
	}
}

