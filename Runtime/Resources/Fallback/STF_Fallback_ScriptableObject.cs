using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.resources
{
	[CreateAssetMenu(menuName = "STF/Resources/Fallback/STF_Fallback_ScriptableObject")]
	public class STF_Fallback_ScriptableObject : STF_ScriptableObject, IJsonFallback
	{
		public string _FallbackType;
		public string FallbackType => _FallbackType;
		public override string STF_Type => FallbackType;

		[TextArea(1, 5)]
		public string _FallbackJson;
		public string FallbackJson => _FallbackJson;

		public List<STF_ScriptableObject> _ReferencedScriptableObjectResources = new();
		public List<STF_ScriptableObject> ReferencedScriptableObjectResources => _ReferencedScriptableObjectResources;

		public List<STF_MonoBehaviour> _ReferencedMonoBehaviourResources = new();
		public List<STF_MonoBehaviour> ReferencedMonoBehaviourResources => _ReferencedMonoBehaviourResources;


		public List<STF_Buffer> _ReferencedBuffers = new();
		public List<STF_Buffer> ReferencedBuffers => _ReferencedBuffers;

		public override string STF_Category => "fallback";
	}

	public static class STF_Fallback_ScriptableObject_Handler
	{

		public static ISTF_Resource Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var ret = ScriptableObject.CreateInstance<STF_Fallback_ScriptableObject>();
			ret.SetFromJson(JsonResource, STF_Id);
			ret._FallbackType = (string)JsonResource.GetValue("type");
			ret._FallbackJson = JsonResource.ToString();

			Context.AddTask(new Task(() => {
				IJsonFallback.ImportReferences(Context, JsonResource, ContextObject, ret);
			}));

			return ret;
		}

		public static (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			var FallbackObject = ApplicationObject as STF_Fallback_ScriptableObject;
			// TODO export buffers and resources
			return (JObject.Parse(FallbackObject.FallbackJson), FallbackObject.STF_Type);
		}
	}
}
