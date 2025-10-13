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
			if (STFBinding.Count == 1 && Node.STF_Owner is STF_Bone stfBone)
			{
				var ownerGo = stfBone.STF_Owner;
				var ret = ownerGo.GetComponentsInChildren<STF_Bone>().FirstOrDefault(b => b.STF_Id == STFBinding[0] && b.STF_Owner == ownerGo);
				return ret != null ? ret.gameObject : null;
			}
			else if (STFBinding.Count == 1)
			{
				var ret = Context.ImportResource(STFBinding[0], "node") as STF_MonoBehaviour;
				return ret != null ? ret.gameObject : null;
			}
			else if (STFBinding.Count == 3 && STFBinding[1] == "instance")
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
			else if (STFBinding.Count == 3 && STFBinding[1] == "instance")
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

		public static STF_MonoBehaviour ResolvePath(STF_MonoBehaviour Source, List<string> TargetPath)
		{
			if(TargetPath == null || TargetPath.Count == 0) return null;
			
			var ret = Source.STF_Owner.GetComponentsInChildren<STF_MonoBehaviour>().FirstOrDefault(b => b.STF_Id == TargetPath[0] && b.STF_Owner == Source.STF_Owner);
			if(ret == null) return null;

			if(TargetPath.Count == 1)
			{
				return ret;
			}
			else if(TargetPath.Count > 2)
			{
				if(TargetPath[1] == "instance" && ret is STF_Node stfNode && stfNode.Instance)
				{
					return ResolvePath(stfNode.Instance, TargetPath.GetRange(2, TargetPath.Count - 2));
				}
				else if(TargetPath[1] == "components")
				{
					return ret.GetComponents<STF_NodeComponentResource>().FirstOrDefault(c => c.STF_Id == TargetPath[2]);
				}
			}

			return null;
		}

		public static string GetResourceID(JObject JsonResource, JToken ResourceIDIndex)
		{
			if(ResourceIDIndex == null) return null;
			var index = -1;
			if(ResourceIDIndex.Type == JTokenType.Integer)
				index = ResourceIDIndex.Value<int>();
			else if(ResourceIDIndex.Type == JTokenType.String) // In case the ResourceIDIndex was the key in an Json object, it will be a string, because Json
				index = int.Parse(ResourceIDIndex.Value<string>());
			else
				return null;

			if(JsonResource.ContainsKey("referenced_resources") && JsonResource["referenced_resources"].Type == JTokenType.Array)
			{
				return JsonResource["referenced_resources"][index].Value<string>();
			}
			return null;
		}

		public static ISTF_Resource ImportResource(ImportContext Context, JObject JsonResource, JToken ResourceIDIndex, string ExpectedKind = "data", ISTF_Resource ContextObject = null)
		{
			if(GetResourceID(JsonResource, ResourceIDIndex) is string resourceID && !string.IsNullOrWhiteSpace(resourceID))
				return Context.ImportResource(resourceID, ExpectedKind, ContextObject);
			else
				return null;
		}

		public static string GetBufferID(JObject JsonResource, JToken BufferIDIndex)
		{
			if(JsonResource.ContainsKey("referenced_buffers") && JsonResource["referenced_buffers"].Type == JTokenType.Array && BufferIDIndex != null && BufferIDIndex.Type == JTokenType.Integer)
			{
				return JsonResource["referenced_buffers"][BufferIDIndex.Value<int>()].Value<string>();
			}
			return null;
		}

		public static List<string> ConvertResourcePath(JObject JsonResource, JToken JsonTargetToken)
		{
			var ret = new List<string>();
			foreach(var pathElement in JsonTargetToken)
			{
				if(pathElement.Type == JTokenType.Integer)
					ret.Add(GetResourceID(JsonResource, pathElement));
				else
					ret.Add(pathElement.Value<string>());
			}
			return ret;
		}
	}
}
