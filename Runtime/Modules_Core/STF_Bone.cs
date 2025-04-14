
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.XR;

namespace com.squirrelbite.stf_unity.modules
{
	public class STF_Bone : STF_NodeResource
	{
		public const string STF_TYPE = "stf.bone";
		public override string STF_Type => STF_TYPE;

		public bool Connected = false;
		public float Length = 0;
	}

	public class STF_Bone_Module : ISTF_Module
	{
		public string STF_Type => STF_Bone.STF_TYPE;

		public string STF_Kind => "node";

		public int Priority => 0;

		public List<string> LikeTypes => new(){"bone", "node"};

		public List<Type> UnderstoodApplicationTypes => new(){typeof(STF_Bone)};

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 1; }

		public List<STF_ComponentResource> GetComponents(ISTF_Resource ApplicationObject) { return ((STF_Bone)ApplicationObject).Components; }

		public (ISTF_Resource STFResource, object ApplicationObject) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = new GameObject(STFUtil.DetermineName(JsonResource, "STF Bone"));
			var ret = go.AddComponent<STF_Bone>();
			ret.SetFromJson(JsonResource, STF_Id);

			ret.Connected = JsonResource.ContainsKey("connected") && (bool)JsonResource["connected"];
			ret.Length = (float)JsonResource["length"];

			go.transform.position = TRSUtil.ParseLocation((JArray)JsonResource["translation"]);
			go.transform.rotation = TRSUtil.ParseRotation((JArray)JsonResource["rotation"]);
			go.transform.Rotate(Vector3.right, -90);

			if(JsonResource.ContainsKey("children")) foreach(var childID in (JArray)JsonResource["children"])
			{
				if(Context.ImportResource((string)childID) is STF_Bone childObject)
				{
					childObject.transform.SetParent(ret.transform);
				}
			}

			if(JsonResource.ContainsKey("instance"))
			{
				Context.ImportResource((string)JsonResource["instance"], ret);
			}

			return (ret, null);
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			var node = ApplicationObject as STF_Bone;
			var ret = new JObject {
				{"type", STF_Type},
				{"name", node.STF_Name},
				//{"trs", TRSUtil.SerializeTRS(node.transform)}
			};

			// TODO stuff

			return (ret, node.STF_Id);
		}
	}
}
