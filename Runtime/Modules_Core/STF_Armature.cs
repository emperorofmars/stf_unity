
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public class STF_Armature : STF_PrefabResource
	{
		public const string STF_TYPE = "stf.armature";
		public override string STF_Type => STF_TYPE;
		public List<Matrix4x4> Bindposes = new();
		public List<string> BindOrder = new();
	}

	public class STF_Armature_Module : ISTF_Module
	{
		public string STF_Type => STF_Armature.STF_TYPE;

		public string STF_Kind => "data";

		public int Priority => 0;

		public List<string> LikeTypes => new(){"armature", "prefab"};

		public List<Type> UnderstoodApplicationTypes => new(){typeof(STF_Armature)};

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public List<STF_ComponentResource> GetComponents(ISTF_Resource ApplicationObject) { return null; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = new GameObject((string)JsonResource.GetValue("name") ?? "STF Armature");
			var ret = go.gameObject.AddComponent<STF_Armature>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "STF Armature");

			foreach(var nodeID in JsonResource["root_bones"])
			{
				if(Context.ImportResource((string)nodeID, ret) is STF_NodeResource nodeGo)
				{
					nodeGo.transform.SetParent(go.transform);
				}
				else
				{
					Context.Report(new STFReport("Invalid Node: " + nodeID, ErrorSeverity.FATAL_ERROR, STF_Type, go, null));
				}
			}

			foreach(var bone in go.GetComponentsInChildren<STF_Bone>())
			{
				ret.BindOrder.Add(bone.STF_Id);
				ret.Bindposes.Add(bone.transform.worldToLocalMatrix);
			}

			return (ret, new(){go});
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			var ArmatureObject = ApplicationObject as STF_Armature;
			var ret = new JObject {
				{"type", STF_Type},
				{"name", ArmatureObject.STF_Name},
			};

			return (ret, ArmatureObject.STF_Id);
		}
	}
}
