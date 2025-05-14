
using System.Collections.Generic;
using System.Linq;
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

		public override (string RelativePath, System.Type Type, string PropertyName) ConvertPropertyPath(List<string> STFPath)
		{
			if(STFPath.Count > 1)
			{
				if(STFPath[0] == "t") return ("", typeof(Transform), "m_LocalPosition." + STFPath[1]);
				else if(STFPath[0] == "r") return ("", typeof(Transform), "m_LocalRotation." + STFPath[1]);
				else if(STFPath[0] == "s") return ("", typeof(Transform), "m_LocalScale." + STFPath[1]);
				else if(STFPath[0] == "instance")
				{
					var instance = this.gameObject.GetComponent<STF_InstanceResource>();
					if(instance)
					{
						(string retRelativePath, System.Type retType, string retPropName) = instance.ConvertPropertyPath(STFPath.GetRange(1, STFPath.Count - 1));
						return (retRelativePath, retType, retPropName);
					}
				}
				else if(STFPath[0] == "component")
				{
					var component = this.gameObject.GetComponents<STF_MonoBehaviour>().FirstOrDefault(c => c.STF_Owner == this && c.STF_Id == STFPath[1]);
					if(component)
					{
						(string retRelativePath, System.Type retType, string retPropName) = component.ConvertPropertyPath(STFPath.GetRange(2, STFPath.Count - 2));
						return (retRelativePath, retType, retPropName);
					}
				}
			}
			return ("", null, null);
		}

		public override List<string> ConvertPropertyPath(string UnityPath)
		{
			throw new System.NotImplementedException();
		}
	}
}
