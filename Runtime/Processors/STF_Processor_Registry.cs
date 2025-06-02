
using System.Collections.Generic;
using System.Linq;
using com.squirrelbite.stf_unity.processors;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	public static class STF_Processor_Registry
	{
		public static readonly Dictionary<string, string> ContextDisplayNames = new() {
			{"default", "Unity Default"},
		};

		public static readonly Dictionary<string, List<ISTF_Processor>> DefaultProcessors = new() {
			{
				"default", new () {
					new STF_Prefab_Processor(),
					new STF_Node_Processor(),
					new STF_Bone_Processor(),
					new STF_Mesh_Processor(),
					new STF_Instance_Mesh_Processor(),
					new STF_Instance_Armature_Processor(),
					new STF_Material_Processor(),
					new STF_Image_Processor(),
					new STF_Animation_Processor(),
				}
			},
		};

		private static readonly Dictionary<string, Dictionary<System.Type, ISTF_Processor>> RegisteredProcessors = new();

		public static void RegisterProcessor(string Context, ISTF_Processor Processor)
		{
			if(!RegisteredProcessors.ContainsKey(Context))
				RegisteredProcessors.Add(Context, new Dictionary<System.Type, ISTF_Processor> {{Processor.TargetType, Processor}});
			else if(!RegisteredProcessors[Context].ContainsKey(Processor.TargetType) || RegisteredProcessors[Context][Processor.TargetType].Priority <= Processor.Priority)
				RegisteredProcessors[Context].Add(Processor.TargetType, Processor);
		}

		public static Dictionary<System.Type, ISTF_Processor> GetProcessors(string Context)
		{
			var ret = new Dictionary<System.Type, ISTF_Processor>(RegisteredProcessors.ContainsKey(Context) ? RegisteredProcessors[Context] : new Dictionary<System.Type, ISTF_Processor>());
			if(DefaultProcessors.ContainsKey(Context))
				foreach(var processor in DefaultProcessors[Context])
					if(!ret.ContainsKey(processor.TargetType))
						ret.Add(processor.TargetType, processor);
			return ret;
		}

		public static List<string> GetAvaliableContexts()
		{
			var ret = new HashSet<string>();
			foreach (var entry in RegisteredProcessors)
				if (!ret.Contains(entry.Key))
					ret.Add(entry.Key);
			foreach (var entry in DefaultProcessors)
				if (!ret.Contains(entry.Key))
					ret.Add(entry.Key);
			return ret.ToList();
		}

		public static List<(string, string)> GetAvaliableContextDisplayNames()
		{
			var contexts = new HashSet<string>();
			foreach(var entry in RegisteredProcessors)
				if(!contexts.Contains(entry.Key))
					contexts.Add(entry.Key);
			foreach(var entry in DefaultProcessors)
				if(!contexts.Contains(entry.Key))
					contexts.Add(entry.Key);

			var ret = new List<(string, string)>();
			foreach (var context in contexts)
				ret.Add((context, ContextDisplayNames.ContainsKey(context) ? ContextDisplayNames[context] : context));

			return ret;
		}
	}
}
