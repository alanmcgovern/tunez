using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Security.Cryptography;

namespace Tunez
{
	public class QueryParameters : List<Tuple<string, string>>
	{
		/// <summary>
		/// Appends an index to the key and then adds the keypair to the internal list. If 'index' is 7
		/// and the key is 'artist', then 'artist[7]' will be generated.
		/// </summary>
		/// <param name="key">The key to add an index to</param>
		/// <param name="name">The value</param>
		/// <param name="index">The index to be added to the key</param>
		public void AddIndexed (string key, string value, int index)
		{
			Add (Tuple.Create (key + '[' + index + ']', value));
		}

		public string GenerateQueryString (string sharedSecret)
		{
			var apiSignature = GenerateApiSignature (sharedSecret);

			// Now add in the signature query param
			Add (Tuple.Create ("api_sig", apiSignature));
			// And make it json, cos I like json
			Add (Tuple.Create ("format", "json"));

			var parameters = this.Select (q => WebUtility.UrlEncode (q.Item1) + "=" + WebUtility.UrlEncode (q.Item2));
			var queryString = string.Join ("&", parameters);
			return queryString;
		}

		string GenerateApiSignature (string sharedSecret)
		{
			var query = this.OrderBy (p => p.Item1, StringComparer.Ordinal).Select (p => p.Item1 + p.Item2);
			var fullQuery = string.Join ("", query) + sharedSecret;
			var bytes = Encoding.UTF8.GetBytes (fullQuery);
			bytes = MD5.Create ().ComputeHash (bytes);
			return BitConverter.ToString (bytes).Replace ("-", "");
		}
	}
}
