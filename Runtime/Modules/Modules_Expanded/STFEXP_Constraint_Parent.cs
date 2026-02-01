using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Animations;

namespace com.squirrelbite.stf_unity.modules.stfexp
{
	public class STFEXP_Constraint_Parent : STF_NodeComponentResource
	{
		public const string _STF_Type = "stfexp.constraint.parent";
		public override string STF_Type => _STF_Type;

		[System.Serializable]
		public class Source
		{
			public float Weight = 0.5f;
			public List<string> SourcePath = new();
			public GameObject SourceGo;
		}

		public float Weight = 1;
		public Axis TranslationAxes = Axis.X | Axis.Y | Axis.Z;
		public Axis RotationAxes = Axis.X | Axis.Y | Axis.Z;
		public List<Source> Sources = new();
	}

	public class STFEXP_Constraint_Parent_InstanceModHandler : STF_InstanceModHandler
	{
		public void HandleInstanceMod(ImportContext Context, ISTF_ComponentResource Resource, JObject JsonResource)
		{
			var ret = Resource as STFEXP_Constraint_Rotation;
			ret.Sources = new();
			if(JsonResource.ContainsKey("sources")) foreach(JObject jsonSource in JsonResource["sources"] as JArray)
			{
				var source = new STFEXP_Constraint_Rotation.Source();
				source.Weight = jsonSource.Value<float>("weight");
				if (jsonSource.ContainsKey("source"))
					source.SourcePath = STFUtil.ConvertResourcePath(JsonResource, jsonSource["source"]);
				ret.Sources.Add(source);

				Context.AddTask(new Task(() => {
					if (source.SourcePath.Count > 0)
						source.SourceGo = STFUtil.ResolveBinding(Context, ret, source.SourcePath);
					else if(ret.transform.parent && ret.transform.parent.parent)
						source.SourceGo = ret.transform.parent.parent.gameObject;
				}));
			}
		}
	}

	public class STFEXP_Constraint_Parent_Module : ISTF_Module
	{
		public string STF_Type => STFEXP_Constraint_Parent._STF_Type;
		public string STF_Kind => "component";
		public int Priority => 1;
		public List<string> LikeTypes => new(){"constraint", "constraint.parent"};
		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(STFEXP_Constraint_Parent)};
		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }
		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<STFEXP_Constraint_Parent>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "STFEXP Constraint Parent");

			if(JsonResource.ContainsKey("weight")) ret.Weight = JsonResource.Value<float>("weight");
			if(JsonResource.ContainsKey("translation_axes") && JsonResource["translation_axes"] is JArray jsonTranslationAxes && jsonTranslationAxes != null)
			{
				var axes = Axis.None;
				if(jsonTranslationAxes[0].Value<bool>()) axes |= Axis.X;
				if(jsonTranslationAxes[1].Value<bool>()) axes |= Axis.Z;
				if(jsonTranslationAxes[2].Value<bool>()) axes |= Axis.Y;
				ret.TranslationAxes = axes;
			}
			if(JsonResource.ContainsKey("rotation_axes") && JsonResource["rotation_axes"] is JArray jsonRotationAxes && jsonRotationAxes != null)
			{
				var axes = Axis.None;
				if(jsonRotationAxes[0].Value<bool>()) axes |= Axis.X;
				if(jsonRotationAxes[1].Value<bool>()) axes |= Axis.Z;
				if(jsonRotationAxes[2].Value<bool>()) axes |= Axis.Y;
				ret.RotationAxes = axes;
			}

			if(JsonResource.ContainsKey("sources")) foreach(JObject jsonSource in (JsonResource["sources"] as JArray).Cast<JObject>())
			{
				var source = new STFEXP_Constraint_Parent.Source();
				source.Weight = jsonSource.Value<float>("weight");
				if (jsonSource.ContainsKey("source"))
					source.SourcePath = STFUtil.ConvertResourcePath(JsonResource, jsonSource["source"]);
				ret.Sources.Add(source);

				Context.AddTask(new Task(() => {
					if (source.SourcePath.Count > 0)
						source.SourceGo = STFUtil.ResolveBinding(Context, ret, source.SourcePath);
					else if(ret.transform.parent && ret.transform.parent.parent)
						source.SourceGo = ret.transform.parent.parent.gameObject;
				}));
			}

			if (JsonResource.ContainsKey("enabled") && JsonResource.Value<bool>("enabled") == false)
				ret.enabled = false;

			ret.InstanceModHandler = new STFEXP_Constraint_Rotation_InstanceModHandler();
			return (ret, null);
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			throw new System.NotImplementedException();
		}
	}
}
