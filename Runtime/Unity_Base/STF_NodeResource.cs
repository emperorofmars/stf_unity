
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	public abstract class STF_NodeResource : STF_MonoBehaviour
	{
		public override string STF_Kind => "node";

		public List<STF_ComponentResource> Components = new();

		public override void SetFromJson(JObject JsonResource, string STF_Id, string DefaultName = "STF Node")
		{
			this.STF_Id = STF_Id;
			this.STF_Name = JsonResource.ContainsKey("name") ? (string)JsonResource["name"] : null;
			this.name = STFUtil.DetermineName(JsonResource, DefaultName);
			this.Degraded = (bool)(JsonResource.GetValue("degraded") ?? false);
		}
	}
}
