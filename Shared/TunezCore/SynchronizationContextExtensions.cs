using System;
using System.Threading;

namespace Tunez
{
	public static class SynchronizationContextExtensions
	{
		public static void Send (this SynchronizationContext context, Action action)
		{
			context.Send (d => action (), null);
		}
	}
}
