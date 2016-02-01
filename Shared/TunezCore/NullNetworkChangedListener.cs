using System;
namespace Tunez
{
	public class NullNetworkChangedListener : INetworkChangedListener
	{
		#pragma warning disable 0067
		public event Action NetworkChanged;
		#pragma warning restore 0067

		public void Dispose ()
		{
		}
	}
}

