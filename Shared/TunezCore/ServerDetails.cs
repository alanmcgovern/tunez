using System;
namespace Tunez
{
	public class ServerDetails : IEquatable<ServerDetails> {

		public bool Default {
			get; set;
		}

		public string Hostname {
			get; set;
		}

		public string FullAddress {
			get { return string.Format ("http://{0}:{1}", Hostname, Port); }
		}

		public bool ManuallyEntered {
			get; set;
		}

		public int Port {
			get; set;
		}

		public override bool Equals(object obj)
		{
			return Equals (obj as ServerDetails);
		}

		public bool Equals (ServerDetails other)
		{
			return other != null
				&& other.Hostname == Hostname
				        && other.Port == Port;
		}

		public override int GetHashCode()
		{
			return Hostname.GetHashCode ();
		}
	}
}

