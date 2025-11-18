using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace com.squirrelbite.stf_unity.modules
{
	/// <summary>
	/// Unity doesn't support components on ScriptableObjects like on GameObjects, so they have to be implemented differently.
	/// </summary>
	public abstract class STF_DataComponentResource: STF_ScriptableObject, ISTF_ComponentResource
	{
		public override string STF_Kind => "component";
		public List<string> _Overrides = new();
		public List<string> Overrides => this._Overrides;

		public UnityEngine.Object ParentObject;

		public override void SetFromJson(JObject JsonResource, string STF_Id, string DefaultName = "STF Prefab")
		{
			base.SetFromJson(JsonResource, STF_Id, DefaultName);
			if (JsonResource.ContainsKey("overrides")) foreach (var o in JsonResource["overrides"])
					Overrides.Add((string)o);
		}
	}
}
