using System.Collections.Generic;
using System.Linq;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.processors;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	public static class STFUtil
	{
		public static string DetermineName(JObject JsonResource, string DefaultName)
		{
			return JsonResource.ContainsKey("name") && !string.IsNullOrWhiteSpace((string)JsonResource["name"]) ? (string)JsonResource["name"] : DefaultName;
		}

		public static T GetProcessed<T>(ISTF_Resource Resource, int Index = 0)
		{
			if (Resource != null && Resource.ProcessedObjects != null && Resource.ProcessedObjects.Count > Index && Resource.ProcessedObjects[Index] is T processed)
				return processed;
			else
				return default;
		}

		public static GameObject ResolveBinding(ImportContext Context, STF_MonoBehaviour Node, List<string> STFBinding)
		{
			if (STFBinding.Count == 1 && Node.STF_Owner is STF_Bone)
			{
				var ownerGo = (Node.STF_Owner as STF_Bone).STF_Owner;
				var ret = ownerGo.GetComponentsInChildren<STF_Bone>().FirstOrDefault(b => b.STF_Id == STFBinding[0] && b.STF_Owner == ownerGo);
				return ret != null ? ret.gameObject : null;
			}
			else if (STFBinding.Count == 1)
			{
				var ret = Context.ImportResource(STFBinding[0], "node") as STF_MonoBehaviour;
				return ret != null ? ret.gameObject : null;
			}
			else if (STFBinding.Count == 3)
			{
				var ownerTransform = Context.ImportResource(STFBinding[0], "node") as STF_MonoBehaviour;
				if(ownerTransform)
				{
					var armatureInstance = ownerTransform.gameObject.GetComponent<STF_Instance_Armature>();
					var ret = ownerTransform.gameObject.GetComponentsInChildren<STF_Bone>().FirstOrDefault(b => b.STF_Id == STFBinding[2] && b.STF_Owner == armatureInstance);
					return ret != null ? ret.gameObject : null;
				}
				else
					return null;
			}
			else
			{
				return null;
			}
		}

		public static GameObject ResolveBinding(ProcessorContextBase Context, STF_MonoBehaviour Node, List<string> STFBinding)
		{
			if (STFBinding.Count == 1 && Node.STF_Owner is STF_Bone stfBone)
			{
				var ownerGo = stfBone.STF_Owner;
				var ret = ownerGo.GetComponentsInChildren<STF_Bone>().FirstOrDefault(b => b.STF_Id == STFBinding[0] && b.STF_Owner == ownerGo);
				return ret != null ? ret.gameObject : null;
			}
			else if (STFBinding.Count == 1)
			{
				var ret = Context.Root.GetComponentsInChildren<STF_Node>().FirstOrDefault(n => n.STF_Id == STFBinding[0]);
				return ret != null ? ret.gameObject : null;
			}
			else if (STFBinding.Count == 3)
			{
				var ownerTransform = Context.Root.GetComponentsInChildren<STF_Node>().FirstOrDefault(n => n.STF_Id == STFBinding[0]);
				if(ownerTransform)
				{
					var armatureInstance = ownerTransform.gameObject.GetComponent<STF_Instance_Armature>();
					var ret = ownerTransform.transform.GetComponentsInChildren<STF_Bone>().FirstOrDefault(b => b.STF_Id == STFBinding[2] && b.STF_Owner == armatureInstance);
					return ret != null ? ret.gameObject : null;
				}
				else
					return null;
			}
			else
			{
				return null;
			}
		}
	}
}
