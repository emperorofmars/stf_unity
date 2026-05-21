using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.resources.stfexp
{
	[AddComponentMenu("STF/Resources/stfexp/stfexp.node.ethereal")]
	[HelpURL("https://docs.stfform.at/resources/stfexp/stfexp_node_ethereal.html")]
	public class STFEXP_Node_Ethereal : STF_NodeComponentResource
	{
		public const string _STF_Type = "stfexp.node.ethereal";
		public override string STF_Type => _STF_Type;

		public bool Preserve = false;
	}

	public class STFEXP_Node_Ethereal_Handler : ISTF_Handler
	{
		public string STF_Type => STFEXP_Node_Ethereal._STF_Type;
		public string STF_Category => "component";
		public int Priority => 1;
		public List<string> LikeTypes => new(){"ethereal"};
		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(STFEXP_Node_Ethereal)};
		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }
		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<STFEXP_Node_Ethereal>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "STFEXP Node Ethereal");
			return (ret, null);
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			throw new System.NotImplementedException();
		}
	}
}
