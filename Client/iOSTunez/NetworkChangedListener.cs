using System;
using System.Net;

using Tunez;
using CoreFoundation;
using SystemConfiguration;

namespace Tunez
{
	public class NetworkChangedListener : INetworkChangedListener
	{
		public event Action NetworkChanged;
		NetworkReachability reachabilityChecker;


		public NetworkChangedListener ()
		{
			reachabilityChecker = new NetworkReachability (new IPAddress(0));
			reachabilityChecker.SetNotification (OnReachabilityChanged);
			reachabilityChecker.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
		}

		public bool IsNetworkAvailable ()
		{
			NetworkReachabilityFlags flags;
			return reachabilityChecker.TryGetFlags (out flags) && flags.HasFlag (NetworkReachabilityFlags.Reachable);
		}

		void OnReachabilityChanged (NetworkReachabilityFlags flags)
		{
			var handler = NetworkChanged;
			if (handler != null)
				handler ();
		}

		public void Dispose ()
		{
			reachabilityChecker.Unschedule ();
			reachabilityChecker.Dispose ();
		}
	}
}
