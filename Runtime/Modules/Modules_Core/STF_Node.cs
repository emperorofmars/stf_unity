
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public class STF_Node : STF_NodeResource
	{
		public const string STF_TYPE = "stf.node";
		public override string STF_Type => STF_TYPE;
		public STF_InstanceResource Instance;
		public List<string> ParentBinding = new();
	}

	public class STF_Node_Module : ISTF_Module
	{
		public string STF_Type => STF_Node.STF_TYPE;

		public string STF_Kind => "node";

		public int Priority => 0;

		public List<string> LikeTypes => new(){"node"};

		public List<Type> UnderstoodApplicationTypes => new(){typeof(STF_Node)};

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 1; }

		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return new List<ISTF_Resource>(((STF_Node)ApplicationObject).Components); }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = new GameObject((string)JsonResource.GetValue("name") ?? "STF Node");
			var ret = go.AddComponent<STF_Node>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "STF Node");

			TRSUtil.ParseTRS(ret.transform, JsonResource);

			if(JsonResource.ContainsKey("children"))
				foreach(var childID in (JArray)JsonResource["children"])
					if(Context.ImportResource((string)childID, "node", ContextObject) is STF_Node childObject && childObject != null)
						childObject.transform.SetParent(ret.transform, false);

			if(JsonResource.ContainsKey("instance") && Context.ImportResource((string)JsonResource["instance"], "instance", ret) is STF_InstanceResource instanceResource && instanceResource != null)
				ret.Instance = instanceResource;

			if(JsonResource.ContainsKey("parent_binding"))
			{
				Context.AddTask(new Task(() => {
					ret.ParentBinding = JsonResource["parent_binding"].ToObject<List<string>>();

					// TODO make more legit
					var parent = ret.transform.parent.gameObject.GetComponent<STF_InstanceResource>();

					var target = parent.gameObject.GetComponentsInChildren<STF_Bone>().FirstOrDefault(c => c.STF_Owner == parent && c.STF_Id ==  ret.ParentBinding[2]);
					if(target) ret.transform.SetParent(target.transform, false);
				}));
			}

			if (JsonResource.ContainsKey("enabled") && JsonResource.Value<bool>("enabled") == false)
				go.SetActive(false);

			return (ret, null);
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			var node = ApplicationObject as STF_Node;
			var ret = new JObject {
				{"type", STF_Type},
				{"name", node.STF_Name},
				{"trs", TRSUtil.SerializeTRS(node.transform)}
			};

			// TODO stuff

			return (ret, node.STF_Id);
		}
	}
}
