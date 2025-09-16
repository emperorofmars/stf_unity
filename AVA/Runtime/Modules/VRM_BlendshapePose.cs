using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using com.squirrelbite.stf_unity.modules;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.ava
{
	public class VRM_BlendshapePose : STF_DataResource
	{
		public const string _STF_Type = "dev.vrm.blendshape_pose";
		public override string STF_Type => _STF_Type;

		[System.Serializable]
		public class BlendshapeValue { public string Name = ""; public float Value = 0; }

		[System.Serializable]
		public class Target { public STF_NodeResource mesh_instance; public List<BlendshapeValue> values = new(); }

		public List<Target> targets = new();
	}

	public class VRM_BlendshapePose_Module : ISTF_Module
	{
		public string STF_Type => VRM_BlendshapePose._STF_Type;

		public string STF_Kind => "data";

		public int Priority => 1;

		public List<string> LikeTypes => new(){"animation", "pose"};

		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(VRM_BlendshapePose)};

		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return new List<ISTF_Resource>(((STF_Node)ApplicationObject).Components); }

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var ret = ScriptableObject.CreateInstance<VRM_BlendshapePose>();
			ret.SetFromJson(JsonResource, STF_Id, "VRM Blendshape Pose");

			if (JsonResource.ContainsKey("targets") && JsonResource["targets"] is JObject targets)
			{
				foreach((string meshInstanceId, var values) in targets)
				{
					var retTarget = new VRM_BlendshapePose.Target() {mesh_instance = Context.ImportResource(meshInstanceId, "node") as STF_NodeResource};
					foreach((string blendshapeName, var blendshapeValue) in values as JObject)
						retTarget.values.Add(new () { Name = blendshapeName, Value = blendshapeValue.Value<float>() });
					ret.targets.Add(retTarget);
				}
			}

			return (ret, new() { ret });
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			throw new System.NotImplementedException();
		}
	}

#if UNITY_EDITOR
	[InitializeOnLoad]
	class Register_VRM_BlendshapePose_Module
	{
		static Register_VRM_BlendshapePose_Module()
		{
			STF_Module_Registry.RegisterModule(new VRM_BlendshapePose_Module());
		}
	}
#endif
}