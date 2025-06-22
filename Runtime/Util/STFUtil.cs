using System.Collections.Generic;
using System.Linq;
using com.squirrelbite.stf_unity.modules;
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
			if (Resource?.ProcessedObjects != null && Resource.ProcessedObjects.Count > Index && Resource.ProcessedObjects[Index] is T)
				return (T)Resource.ProcessedObjects[Index];
			else
				return default;
		}

		public static GameObject ResolveBinding(ImportContext Context, STF_MonoBehaviour Node, List<string> STFBinding)
		{
			if (STFBinding.Count == 1 && Node.STF_Owner is STF_Bone)
			{
				var ownerGo = (Node.STF_Owner as STF_Bone).STF_Owner;
				return ownerGo.GetComponentsInChildren<STF_Bone>().FirstOrDefault(b => b.STF_Id == STFBinding[0] && b.STF_Owner == ownerGo)?.gameObject;
			}
			else if (STFBinding.Count == 1)
			{
				return (Context.ImportResource(STFBinding[0], "node") as STF_MonoBehaviour)?.gameObject;
			}
			else if (STFBinding.Count == 3)
			{
				var ownerGo = (Context.ImportResource(STFBinding[0], "node") as STF_MonoBehaviour)?.gameObject;
				var armatureInstance = ownerGo.GetComponent<STF_Instance_Armature>();
				return ownerGo.GetComponentsInChildren<STF_Bone>().FirstOrDefault(b => b.STF_Id == STFBinding[2] && b.STF_Owner == armatureInstance)?.gameObject;
			}
			else
			{
				return null;
			}
		}
	}
}
