
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	public abstract class STF_PrefabResource : STF_MonoBehaviour
	{
		public override string STF_Kind => "data";

		public List<STF_MonoBehaviour> Components = new();

		public override void SetFromJson(JObject JsonResource, string STF_Id, ISTF_Resource ContextObject, string DefaultName = "STF ScriptableObject")
		{
			this.STF_Id = STF_Id;
			this.STF_Name = JsonResource.ContainsKey("name") ? (string)JsonResource["name"] : null;
			this.name = STFUtil.DetermineName(JsonResource, DefaultName);
			this.Degraded = (bool)(JsonResource.GetValue("degraded") ?? false);
			if(ContextObject is STF_MonoBehaviour) this.STF_Owner = ContextObject as STF_MonoBehaviour;
		}


		public override (string RelativePath, System.Type Type, List<string> PropertyNames, System.Func<List<float>, List<float>> ConvertValueFunc) ConvertPropertyPath(List<string> STFPath)
		{
			if(STFPath.Count > 1)
			{
				var nodeId = STFPath[0];
				var target = this.gameObject.GetComponentsInChildren<STF_NodeResource>().FirstOrDefault(c => c.STF_Owner == this && c.STF_Id == nodeId);
				if(target)
				{
					var ret = UnityUtil.getPath(this.transform, target.transform, true);

					(string retRelativePath, System.Type retType, List<string> retPropNames, System.Func<List<float>, List<float>> convertValueFunc) = target.ConvertPropertyPath(STFPath.GetRange(1, STFPath.Count - 1));
					return (string.IsNullOrEmpty(retRelativePath) ? ret : ret + "/" + retRelativePath, retType, retPropNames, convertValueFunc);
				}
			}
			return ("", null, null, null);
		}

		public override List<string> ConvertPropertyPath(string UnityPath)
		{
			throw new System.NotImplementedException();
		}
	}
}
