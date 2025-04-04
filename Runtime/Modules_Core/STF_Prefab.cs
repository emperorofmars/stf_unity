
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public class STF_Prefab : STF_PrefabResource
	{
		public const string STF_TYPE = "stf.prefab";
		public override string STF_Type => STF_TYPE;
	}

	public class STF_Prefab_Module : ISTF_Module
	{
		public string STF_Type => STF_Prefab.STF_TYPE;

		public string STF_Kind => "data";

		public int Priority => 0;

		public List<string> LikeTypes => new(){"prefab", "hierarchy"};

		public List<Type> UnderstoodApplicationTypes => new(){typeof(STF_Prefab)};

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public List<STF_ComponentResource> GetComponents(ISTF_Resource ApplicationObject) { return ((STF_Prefab)ApplicationObject).Components; }

		public ISTF_Resource Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = new GameObject((string)JsonResource.GetValue("name") ?? "STF Prefab");
			var ret = go.AddComponent<STF_Prefab>();
			ret.SetFromJson(JsonResource, STF_Id);

			foreach(var nodeID in JsonResource["root_nodes"])
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

			return ret;
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			var PrefabObject = ApplicationObject as STF_Prefab;
			var ret = new JObject {
				{"type", STF_Type},
				{"name", PrefabObject.STF_Name},
			};

			return (ret, PrefabObject.STF_Id);
		}
	}
}
