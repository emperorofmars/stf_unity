
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

		public (object ApplicationObject, IImportContext Context) Import(IImportContext Context, JObject Json, string ID, object ParentApplicationObject)
		{
			var ret = new GameObject((string)Json.GetValue("name") ?? "STF Prefab");
			var resourceContext = new ResourceImportContext(Context, ret);

			return (ret, resourceContext);
		}

		public (JObject Json, string ID, IExportContext Context) Export(IExportContext Context, object ApplicationObject, object ParentApplicationObject)
		{
			var PrefabObject = ApplicationObject as GameObject;
			var ret = new JObject {
				{"type", _STF_Type},
			};
			var resourceContext = new ResourceExportContext(Context, ret);

			return (ret, "", resourceContext);
		}
	}
}
