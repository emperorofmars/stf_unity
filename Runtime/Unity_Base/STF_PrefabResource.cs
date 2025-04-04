
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	public abstract class STF_PrefabResource : STF_MonoBehaviour
	{
		public override string STF_Kind => "data";

		public List<STF_ComponentResource> Components = new();

		public override void SetFromJson(JObject JsonResource, string STF_Id)
		{
			this.STF_Id = STF_Id;
			this.STF_Name = (string)JsonResource.GetValue("name") ?? "STF Prefab";
			this.name = STF_Name;
			this.Degraded = (bool)(JsonResource.GetValue("degraded") ?? false);
		}
	}
}
