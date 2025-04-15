
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public class STF_Mesh : STF_DataResource
	{
		public const string STF_TYPE = "stf.mesh";
		public override string STF_Type => STF_TYPE;
	}

	public class STF_Mesh_Module : ISTF_Module
	{
		public string STF_Type => STF_Mesh.STF_TYPE;

		public string STF_Kind => "data";

		public int Priority => 0;

		public List<string> LikeTypes => new(){"mesh"};

		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(STF_Mesh)};

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public List<STF_ComponentResource> GetComponents(ISTF_Resource ApplicationObject) { return null; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var ret = ScriptableObject.CreateInstance<STF_Mesh>();
			ret.SetFromJson(JsonResource, STF_Id, "STF Mesh");

			return (ret, new(){ret});
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			var MeshObject = ApplicationObject as STF_Mesh;
			var ret = new JObject {
				{"type", STF_Type},
				{"name", MeshObject.STF_Name},
			};

			return (ret, MeshObject.STF_Id);
		}
	}
}
