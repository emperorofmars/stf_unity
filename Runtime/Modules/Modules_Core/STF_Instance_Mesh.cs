using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public class STF_Instance_Mesh : STF_InstanceResource
	{
		public const string STF_TYPE = "stf.instance.mesh";
		public override string STF_Type => STF_TYPE;

		public STF_Mesh Mesh;
		public List<STF_Material> Materials = new();
		public List<(bool, float)> BlendshapeValues = new();
		public GameObject ArmatureInstance;
	}

	public class STF_Instance_Mesh_Module : ISTF_Module
	{
		public string STF_Type => STF_Instance_Mesh.STF_TYPE;

		public string STF_Kind => "instance";

		public int Priority => 0;

		public List<string> LikeTypes => new() { "instance.mesh", "instance" };

		public List<System.Type> UnderstoodApplicationTypes => new() { typeof(STF_Instance_Mesh) };

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = (STF_Node)ContextObject;
			var ret = go.gameObject.AddComponent<STF_Instance_Mesh>();
			//var ret = ScriptableObject.CreateInstance<STF_Instance_Mesh>();
			go.Instance = ret;
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "STF Instance Mesh");

			if (JsonResource.ContainsKey("materials"))
			{
				ret.Materials = new List<STF_Material>(new STF_Material[JsonResource["materials"].Count()]);
				for (int matIdx = 0; matIdx < JsonResource["materials"].Count(); matIdx++)
				{
					if (Context.ImportResource((string)JsonResource["materials"][matIdx], "data") is var stfMaterial)
					{
						ret.Materials[matIdx] = stfMaterial as STF_Material;
					}
				}
			}
			if (JsonResource.ContainsKey("blendshape_values"))
				foreach (var value in JsonResource["blendshape_values"])
					if (value.Type != JTokenType.Null)
						ret.BlendshapeValues.Add((true, (float)value));
					else
						ret.BlendshapeValues.Add((false, 0));

			Context.AddTask(new Task(() =>
			{
				if (JsonResource.ContainsKey("armature_instance"))
				{
					ret.ArmatureInstance = ((STF_MonoBehaviour)Context.ImportResource((string)JsonResource["armature_instance"], "instance")).gameObject;
				}

				ret.Mesh = (STF_Mesh)Context.ImportResource((string)JsonResource["mesh"], "data");
			}));

			return (ret, null);
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			var PrefabObject = ApplicationObject as STF_Mesh;
			var ret = new JObject {
				{"type", STF_Type},
				{"name", PrefabObject.STF_Name},
			};

			return (ret, PrefabObject.STF_Id);
		}
	}
}
