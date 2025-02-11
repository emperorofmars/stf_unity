
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public class STF_Node_Spatial : MonoBehaviour
	{
		public List<STF_Component> Components = new();
	}

	public class STF_Node_Spatial_Module : STF_Module
	{
		public const string _STF_Type = "stf.node.spatial";
		public string STF_Type => _STF_Type;

		public string STF_Kind => "node";

		public int Priority => 0;

		public List<string> LikeTypes => new(){"node.spatial", "node"};

		public List<Type> UnderstoodApplicationTypes => new(){typeof(STF_Node_Spatial)};

		public int CanHandleApplicationObject(object ApplicationObject) { return 0; }

		public List<STF_Component> GetComponents(object ApplicationObject) { return ((STF_Node_Spatial)ApplicationObject).Components; }

		public (object ApplicationObject, IImportContext Context) Import(IImportContext Context, JObject Json, string ID, object ParentApplicationObject)
		{
			var ret = new GameObject((string)Json.GetValue("name") ?? "STF Node");
			var resourceContext = new ResourceImportContext(Context, ret);

			TRSUtil.ParseTRS(ret, Json);

			if(Json.ContainsKey("children")) foreach(var childID in (JArray)Json["children"])
			{
				if(resourceContext.ImportResource((string)childID) is GameObject childObject)
				{
					childObject.transform.SetParent(ret.transform);
				}
			}

			return (ret, resourceContext);
		}

		public (JObject Json, string ID, IExportContext Context) Export(IExportContext Context, object ApplicationObject, object ParentApplicationObject)
		{
			var Go = ApplicationObject as GameObject;
			var ret = new JObject {
				{"type", _STF_Type},
				{"name", Go.name},
				{"trs", TRSUtil.SerializeTRS(Go)}
			};
			var resourceContext = new ResourceExportContext(Context, ret);

			return (ret, "", resourceContext);
		}
	}
}
