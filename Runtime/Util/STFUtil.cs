using com.squirrelbite.stf_unity.modules;
using Newtonsoft.Json.Linq;

namespace com.squirrelbite.stf_unity
{
	public static class STFUtil
	{
		public static string DetermineName(JObject JsonResource, string DefaultName)
		{
			return JsonResource.ContainsKey("name") && !string.IsNullOrWhiteSpace((string)JsonResource["name"]) ? (string)JsonResource["name"] : DefaultName;
		}

		public static T GetProcessed<T>(ISTF_Resource Resource, int Index = 0)
		{
			if (Resource?.ProcessedObjects != null && Resource.ProcessedObjects.Count > Index && Resource.ProcessedObjects[Index] is T)
				return (T)Resource.ProcessedObjects[Index];
			else
				return default;
		}
	}
}
