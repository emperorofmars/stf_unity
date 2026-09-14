#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.resources.stfexp;

namespace com.squirrelbite.stf_unity.ava.vrm1
{
	public class VRM1_ContextFactory : STF_ApplicationContextDefinition
	{
		public string ContextId => DetectorVRM1.STF_VRM1_AVATAR_CONTEXT;

		public string DisplayName => "VRM 1 Avatar";

		public ProcessorContextBase Create(ProcessorState State)
		{
			return new AVAContext(State);
		}
	}

	[InitializeOnLoad, ExecuteInEditMode]
	public class DetectorVRM1
	{
		const string STF_AVA_VRM1_FOUND = "STF_AVA_VRM1_FOUND";
		public const string STF_VRM1_AVATAR_CONTEXT = "vrm1";

		public static readonly List<System.Type> Ignores = new() { typeof(STFEXP_Collider_Sphere), typeof(STFEXP_Collider_Capsule), typeof(STFEXP_Collider_Plane), };

		static DetectorVRM1()
		{
#if STF_AVA_VRM1_FOUND
			Debug.Log("AVA: Found VRM1 SDK");
			STF_Processor_Registry.RegisterContext(new VRM1_ContextFactory());

			foreach((var _, var processor) in STF_Processor_Registry.GetProcessors("default"))
				if(!Ignores.Contains(processor.TargetType))
					STF_Processor_Registry.RegisterProcessor(STF_VRM1_AVATAR_CONTEXT, processor);
#endif
		}
	}
}

#endif
