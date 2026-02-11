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

		public string _ExclusionGroup = null;
		public string ExclusionGroup => this._ExclusionGroup;

		public UnityEngine.Object ParentObject;

		public override void SetFromJson(JObject JsonResource, string STF_Id, string DefaultName = "STF Prefab")
		{
			base.SetFromJson(JsonResource, STF_Id, DefaultName);
			if (JsonResource.ContainsKey("exclusion_group")) _ExclusionGroup = JsonResource.Value<string>("exclusion_group");
		}
	}
}
