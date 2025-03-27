
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public class STF_Prefab : MonoBehaviour
	{
		public List<STF_Component> Components = new();
	}

	public class STF_Prefab_Module : STF_Module
	{
		public const string _STF_Type = "stf.prefab";
		public string STF_Type => _STF_Type;

		public string STF_Kind => "data";

		public int Priority => 0;

		public List<string> LikeTypes => new(){"prefab", "hierarchy"};

		public List<Type> UnderstoodApplicationTypes => new(){typeof(STF_Prefab)};

		public int CanHandleApplicationObject(object ApplicationObject) { return 0; }

		public List<STF_Component> GetComponents(object ApplicationObject) { return ((STF_Prefab)ApplicationObject).Components; }

		public object Import(ImportContext Context, JObject Json, string ID, object ContextObject)
		{
			var ret = new GameObject((string)Json.GetValue("name") ?? "STF Prefab");

			foreach(var nodeID in Json["root_nodes"])
			{
				if(Context.ImportResource((string)nodeID, ret) is GameObject nodeGo)
				{
					nodeGo.transform.SetParent(ret.transform);
				}
				else
				{
					// TODO report error
					Debug.LogError("Invalid Node: " + nodeID);
				}
			}

			return ret;
		}

		public (JObject Json, string ID) Export(ExportContext Context, object ApplicationObject, object ContextObject)
		{
			var PrefabObject = ApplicationObject as GameObject;
			var ret = new JObject {
				{"type", _STF_Type},
				{"name", PrefabObject.name},
			};

			return (ret, "");
		}
	}
}
