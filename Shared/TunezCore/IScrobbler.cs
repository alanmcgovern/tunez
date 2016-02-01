using System.Threading.Tasks;

namespace Tunez
{
	public interface IScrobbler
	{
		Task<bool> Scrobble (Track track);
	}
}
