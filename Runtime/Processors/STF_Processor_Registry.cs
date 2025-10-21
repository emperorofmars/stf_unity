using System.Collections.Generic;
using System.Linq;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.processors.stfexp;

namespace com.squirrelbite.stf_unity
{
	public static class STF_Processor_Registry
	{
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
					new STFEXP_Humanoid_Armature_Processor(),
					new STFEXP_Constraint_Twist_Processor(),
					new STFEXP_Camera_Processor(),
					new STFEXP_Light_Processor(),
					new STFEXP_LightprobeAnchor_Processor(),
					new STFEXP_Collider_Sphere_Processor(),
					new STFEXP_Collider_Capsule_Processor(),
				}
			},
		};

		private static readonly Dictionary<string, Dictionary<System.Type, ISTF_Processor>> RegisteredProcessors = new();

		private static readonly Dictionary<string, Dictionary<System.Type, ISTF_GlobalProcessor>> RegisteredGlobalProcessors = new();

		private static readonly Dictionary<string, STF_ApplicationContextDefinition> RegisteredApplicationContexts = new();

		public static void RegisterProcessor(string Context, ISTF_Processor Processor)
		{
			if (!RegisteredProcessors.ContainsKey(Context))
				RegisteredProcessors.Add(Context, new Dictionary<System.Type, ISTF_Processor> { { Processor.TargetType, Processor } });
			else if (!RegisteredProcessors[Context].ContainsKey(Processor.TargetType) || RegisteredProcessors[Context][Processor.TargetType].Priority < Processor.Priority)
				RegisteredProcessors[Context].Add(Processor.TargetType, Processor);
		}

		public static void RegisterGlobalProcessor(string Context, ISTF_GlobalProcessor Processor)
		{
			if (!RegisteredGlobalProcessors.ContainsKey(Context))
				RegisteredGlobalProcessors.Add(Context, new Dictionary<System.Type, ISTF_GlobalProcessor> { { Processor.TargetType, Processor } });
			else if (!RegisteredGlobalProcessors[Context].ContainsKey(Processor.TargetType) || RegisteredGlobalProcessors[Context][Processor.TargetType].Priority < Processor.Priority)
				RegisteredGlobalProcessors[Context].Add(Processor.TargetType, Processor);
		}

		public static void RegisterContext(STF_ApplicationContextDefinition ContextDefinition)
		{
			if (!RegisteredApplicationContexts.ContainsKey(ContextDefinition.ContextId))
				RegisteredApplicationContexts.Add(ContextDefinition.ContextId, ContextDefinition);
			else
				RegisteredApplicationContexts[ContextDefinition.ContextId] = ContextDefinition;
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

		public static Dictionary<System.Type, ISTF_GlobalProcessor> GetGlobalProcessors(string Context)
		{
			var ret = new Dictionary<System.Type, ISTF_GlobalProcessor>(RegisteredGlobalProcessors.ContainsKey(Context) ? RegisteredGlobalProcessors[Context] : new Dictionary<System.Type, ISTF_GlobalProcessor>());
			/*if(DefaultGlobalProcessors.ContainsKey(Context))
				foreach(var processor in DefaultGlobalProcessors[Context])
					if(!ret.ContainsKey(processor.TargetType))
						ret.Add(processor.TargetType, processor);*/
			return ret;
		}

		public static STF_ApplicationContextDefinition GetApplicationContextDefinition(string Context)
		{
			if (RegisteredApplicationContexts.ContainsKey(Context))
				return RegisteredApplicationContexts[Context];
			else
				return null;
		}

		public static ProcessorContextBase CreateApplicationContext(string Context, ProcessorState State)
		{
			if (RegisteredApplicationContexts.ContainsKey(Context))
				return RegisteredApplicationContexts[Context].Create(State);
			else
				return new ProcessorContextBase(State);
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
			{
				if (context == "default")
					ret.Add((context, "Unity Default"));
				else
					ret.Add((context, RegisteredApplicationContexts.ContainsKey(context) ? RegisteredApplicationContexts[context].DisplayName : context));
			}

			ret.Sort((a, b) => string.Compare(a.Item2.ToLower(), b.Item2.ToLower()));
			return ret;
		}
	}
}
