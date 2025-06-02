using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.serialization
{
	public static class Unity_Serializer_Runner
	{
		public static List<SerializerResult> Run(Object Target, UnitySerializerContext Context = null)
		{
			var ret = new List<SerializerResult>();
			if(Target != null)
			{
				if(Target is GameObject @object) ret.AddRange(Run(@object));
				else if(Target is Component component) ret.AddRange(Run(component));
				// TODO Resources Maybe?
			}
			return ret;
		}

		public static List<SerializerResult> Run(Component Target, UnitySerializerContext Context = null)
		{
			var ret = new List<SerializerResult>();
			if(Target != null)
			{
				Context ??= new UnitySerializerContext(Target);
				foreach(var serializer in Unity_Serializer_Registry.Serializers.FindAll(s => Target.GetType() == s.Target))
				{
					var components = serializer.Serialize(Context, Target);
					if(components != null && components.Count > 0) ret.AddRange(serializer.Serialize(Context, Target));
				}
			}
			return ret;
		}

		public static List<SerializerResult> Run(GameObject Target, UnitySerializerContext Context = null)
		{
			var ret = new List<SerializerResult>();
			if(Target != null)
			{
				var targets = Target.transform.GetComponentsInChildren<Component>();
				Context ??= new UnitySerializerContext(targets);
				foreach(var t in Target.transform.GetComponentsInChildren<Component>())
				{
					if(t.GetType() == typeof(Transform)) continue;
					ret.AddRange(Run(t, Context));
				}
			}
			return ret;
		}

		public static bool ValidaValidateResultteJsonResult(SerializerResult Result)
		{
			return
				Result.Result != null && Result.Result.Count > 0
				&& !string.IsNullOrWhiteSpace(Result.STFType);
		}

		public static string CreateSetupString(List<SerializerResult> Results)
		{
			var ret = new JArray();
			foreach(var result in Results)
			{
				var jsonInstruction = new JObject {
					{"instruction_type", "json"},
					{"stf_type", result.STFType},
					{"data", result.Result}
				};
				ret.Add(jsonInstruction);
			}
			return ret.ToString(Newtonsoft.Json.Formatting.None);
		}
	}
}