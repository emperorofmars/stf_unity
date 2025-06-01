using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.stfexp
{
	public class STFEXP_Constraint_Twist : STF_NodeComponentResource
	{
		public const string _STF_Type = "stfexp.constraint.twist";
		public override string STF_Type => _STF_Type;

		public float Weight = 0.5f;
		public List<string> Target = new();

		public override (string RelativePath, Type Type, List<string> PropertyNames, Func<List<float>, List<float>> ConvertValueFunc) ConvertPropertyPath(List<string> STFPath)
		{
			throw new NotImplementedException();
		}

		public override List<string> ConvertPropertyPath(string UnityPath)
		{
			throw new NotImplementedException();
		}
	}

	public class STFEXP_Constraint_Twist_Module : ISTF_Module
	{
		public string STF_Type => STFEXP_Constraint_Twist._STF_Type;

		public string STF_Kind => "component";

		public int Priority => 1;

		public List<string> LikeTypes => new(){"constraint"};

		public List<Type> UnderstoodApplicationTypes => new(){typeof(STFEXP_Constraint_Twist)};

		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<STFEXP_Constraint_Twist>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "STFEXP Constraint Twist");

			ret.Weight = JsonResource.Value<float>("weight");

			if (JsonResource.ContainsKey("target"))
				ret.Target = JsonResource["target"].ToObject<List<string>>();

			return (ret, null);
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			throw new NotImplementedException();
		}
	}

#if UNITY_EDITOR
	[InitializeOnLoad]
	class Register_STFEXP_Constraint_Twist
	{
		static Register_STFEXP_Constraint_Twist()
		{
			STF_Module_Registry.RegisterModule(new STFEXP_Constraint_Twist_Module());
		}
	}
#endif
}