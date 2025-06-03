
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace com.squirrelbite.stf_unity
{
	public abstract class STF_DataComponentResource: STF_ScriptableObject
	{
		public override string STF_Kind => "component";
		public readonly List<string> Overrides = new();

		public override void SetFromJson(JObject JsonResource, string STF_Id, string DefaultName = "STF Prefab")
		{
			base.SetFromJson(JsonResource, STF_Id, DefaultName);
			if (JsonResource.ContainsKey("overrides")) foreach (var o in JsonResource["overrides"])
				Overrides.Add((string)o);
		}
	}
}
