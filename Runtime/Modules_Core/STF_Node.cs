
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

	public class STF_Node_Module : STF_Module
	{
		public const string _STF_Type = "stf.node";
		public string STF_Type => _STF_Type;

		public string STF_Kind => "node";

		public int Priority => 0;

		public List<string> LikeTypes => new(){"node"};

		public List<Type> UnderstoodApplicationTypes => new(){typeof(STF_Node_Spatial)};

		public int CanHandleApplicationObject(object ApplicationObject) { return 0; }

		public List<STF_Component> GetComponents(object ApplicationObject) { return ((STF_Node_Spatial)ApplicationObject).Components; }

		public object Import(ImportContext Context, JObject Json, string ID, object ContextObject)
		{
			var ret = new GameObject((string)Json.GetValue("name") ?? "STF Node");

			TRSUtil.ParseTRS(ret, Json);

			if(Json.ContainsKey("children")) foreach(var childID in (JArray)Json["children"])
			{
				if(Context.ImportResource((string)childID) is GameObject childObject)
				{
					childObject.transform.SetParent(ret.transform);
				}
			}

			return ret;
		}

		public (JObject Json, string ID) Export(ExportContext Context, object ApplicationObject, object ContextObject)
		{
			var Go = ApplicationObject as GameObject;
			var ret = new JObject {
				{"type", _STF_Type},
				{"name", Go.name},
				{"trs", TRSUtil.SerializeTRS(Go)}
			};

			return (ret, "");
		}
	}
}
