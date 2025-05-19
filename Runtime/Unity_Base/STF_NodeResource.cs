
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	public abstract class STF_NodeResource : STF_MonoBehaviour
	{
		public override string STF_Kind => "node";

		public List<STF_MonoBehaviour> Components = new();

		public override void SetFromJson(JObject JsonResource, string STF_Id, ISTF_Resource ContextObject, string DefaultName = "STF Node")
		{
			this.STF_Id = STF_Id;
			this.STF_Name = JsonResource.ContainsKey("name") ? (string)JsonResource["name"] : null;
			this.name = STFUtil.DetermineName(JsonResource, DefaultName);
			this.Degraded = (bool)(JsonResource.GetValue("degraded") ?? false);
			if(ContextObject is STF_MonoBehaviour) this.STF_Owner = ContextObject as STF_MonoBehaviour;
		}

		public override (string RelativePath, System.Type Type, List<string> PropertyNames, System.Func<List<float>, List<float>> ConvertValueFunc) ConvertPropertyPath(List<string> STFPath)
		{
			var convert = new System.Func<List<float>, List<float>>(Values => {
				if(STFPath[0] == "t")
				{
					var ret = this.transform.localPosition + new Vector3(-Values[0], Values[1], Values[2]);
					return new () {ret.x, ret.y, ret.z};
				}
				else if(STFPath[0] == "r")
				{
					var ret = this.transform.localRotation * new Quaternion(Values[0], -Values[1], -Values[2], Values[3]);
					return new () {ret.x, ret.y, ret.z, ret.w};
				}
				else return Values;
			});

			if(STFPath.Count > 0)
			{
				if(STFPath[0] == "t") return ("", typeof(Transform), new(){"m_LocalPosition.x", "m_LocalPosition.y", "m_LocalPosition.z"}, convert);
				else if(STFPath[0] == "r") return ("", typeof(Transform), new(){"m_LocalRotation.x", "m_LocalRotation.y", "m_LocalRotation.z", "m_LocalRotation.w"}, convert);
				else if(STFPath[0] == "s") return ("", typeof(Transform), new(){"m_LocalScale.x", "m_LocalScale.y", "m_LocalScale.z"}, convert);
				else if(STFPath[0] == "instance")
				{
					var instance = this.gameObject.GetComponent<STF_InstanceResource>();
					if(instance)
					{
						(string retRelativePath, System.Type retType, List<string> retPropNames, System.Func<List<float>, List<float>> convertValueFunc) = instance.ConvertPropertyPath(STFPath.GetRange(1, STFPath.Count - 1));
						return (retRelativePath, retType, retPropNames, convertValueFunc);
					}
				}
				else if(STFPath[0] == "component")
				{
					var component = this.gameObject.GetComponents<STF_MonoBehaviour>().FirstOrDefault(c => c.STF_Owner == this && c.STF_Id == STFPath[1]);
					if(component)
					{
						(string retRelativePath, System.Type retType, List<string> retPropNames, System.Func<List<float>, List<float>> convertValueFunc) = component.ConvertPropertyPath(STFPath.GetRange(2, STFPath.Count - 2));
						return (retRelativePath, retType, retPropNames, convertValueFunc);
					}
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
