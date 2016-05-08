using System;
using System.IO;
using System.Threading.Tasks;

namespace Tunez
{
	public interface IConnection
	{
		Task<byte[]> GetResponse (string message, string payload);
		Task<Stream> GetStreamingResponse (string message, string payload);
	}
}

