using System;

namespace Tunez
{
	public interface INetworkChangedListener : IDisposable
	{
		event Action NetworkChanged;
	}
}

