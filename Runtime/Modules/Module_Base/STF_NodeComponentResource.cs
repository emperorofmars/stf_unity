using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace com.squirrelbite.stf_unity.modules
{
	public interface STF_InstanceModHandler
	{
		void HandleInstanceMod(ImportContext Context, ISTF_ComponentResource Resource, JObject JsonResource);
	}

	public abstract class STF_NodeComponentResource : STF_MonoBehaviour, ISTF_ComponentResource
	{
		public override string STF_Kind => "component";
		public List<string> _Overrides = new();
		public List<string> Overrides => this._Overrides;

		// When this component happens to be on a bone inside an armature, it will get instantiated with the armature. Armature instances have 'component-mods', which can override values.
		// If this component supports that, set a STF_InstanceModHandler implementation here. Each Armature instance will call it.
		public STF_InstanceModHandler InstanceModHandler = null;

		public override void SetFromJson(JObject JsonResource, string STF_Id, ISTF_Resource ContextObject, string DefaultName = "STF Prefab")
		{
			base.SetFromJson(JsonResource, STF_Id, ContextObject, DefaultName);
			if (JsonResource.ContainsKey("overrides")) foreach (var o in JsonResource["overrides"])
					Overrides.Add((string)o);
		}
	}
}
