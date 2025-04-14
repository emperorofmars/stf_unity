using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	public static class STFUtil
	{
		public static string DetermineName(JObject JsonResource, string DefaultName)
		{
			return JsonResource.ContainsKey("name") && !string.IsNullOrWhiteSpace((string)JsonResource["name"]) ? (string)JsonResource["name"] : DefaultName;
		}
	}
}
