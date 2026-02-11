using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace com.squirrelbite.stf_unity.modules
{
	public abstract class STF_NodeComponentResource : STF_MonoBehaviour, ISTF_ComponentResource
	{
		public override string STF_Kind => "component";

		public string _ExclusionGroup = null;
		public string ExclusionGroup => this._ExclusionGroup;

		public override void SetFromJson(JObject JsonResource, string STF_Id, ISTF_Resource ContextObject, string DefaultName = "STF Prefab")
		{
			base.SetFromJson(JsonResource, STF_Id, ContextObject, DefaultName);
			if (JsonResource.ContainsKey("exclusion_group")) _ExclusionGroup = JsonResource.Value<string>("exclusion_group");
		}
	}
}
