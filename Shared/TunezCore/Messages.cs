using System;

namespace Tunez
{
	public class Messages
	{
		public static string FetchCatalog = "fetch_catalog";
		public static string FetchGzipCompressedCatalog = "fetch_gzip_catalog";
		public static string FetchTrack = "fetch_track=";

	}

	public class FetchCatalogMessage
	{
		// If the (cached) local catalog and remote catalog have the same UUID, then there's no
		// need to re-request it
		public long UUID {
			get; set;
		}
	}

	public class FetchTrackMessage
	{
		public int UUID {
			get; set;
		}

		public long Offset {
			get; set;
		}
	}
}
