
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace com.squirrelbite.stf_unity
{
	public abstract class STF_PrefabResource : STF_MonoBehaviour
	{
		public override string STF_Kind => "data";
		public ISTF_Resource Fallback;

		public List<STF_MonoBehaviour> Components = new();

		public override void SetFromJson(JObject JsonResource, string STF_Id, ISTF_Resource ContextObject, string DefaultName = "STF ScriptableObject")
		{
			this.STF_Id = STF_Id;
			this.STF_Name = JsonResource.ContainsKey("name") ? (string)JsonResource["name"] : null;
			this.name = STFUtil.DetermineName(JsonResource, DefaultName);
			this.Degraded = (bool)(JsonResource.GetValue("degraded") ?? false);
			if(ContextObject is STF_MonoBehaviour) this.STF_Owner = ContextObject as STF_MonoBehaviour;
		}
	}
}
