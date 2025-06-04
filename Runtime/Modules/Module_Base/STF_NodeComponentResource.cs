
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public abstract class STF_NodeComponentResource: STF_MonoBehaviour
	{
		public override string STF_Kind => "component";
		public List<string> Overrides = new();

		public override void SetFromJson(JObject JsonResource, string STF_Id, ISTF_Resource ContextObject, string DefaultName = "STF Prefab")
		{
			base.SetFromJson(JsonResource, STF_Id, ContextObject, DefaultName);
			if (JsonResource.ContainsKey("overrides")) foreach (var o in JsonResource["overrides"])
					Overrides.Add((string)o);

			foreach (var o in Overrides)
				Debug.Log(o);
		}
	}
}
