
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public class STF_Node_Fallback : MonoBehaviour
	{
		public List<STF_Component> Components = new();
	}

	public class STF_Node_Fallback_Module : IJsonFallback_Module
	{
		public string _STF_Type = "";
		public string STF_Type => _STF_Type;

		public string STF_Kind => "node";

		public (object ApplicationObject, IImportContext Context) Import(IImportContext Context, JObject Json, string ID, object ParentApplicationObject)
		{
			Debug.Log("WOOOOOOOOOOOOOO");

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
