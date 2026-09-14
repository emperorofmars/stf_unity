#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using com.squirrelbite.stf_unity.processors;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.resources.stfexp;

namespace com.squirrelbite.stf_unity.ava.univrm0
{
	public class UNIVRM0ContextFactory : STF_ApplicationContextDefinition
	{
		public string ContextId => DetectorUNIVRM0.STF_UNIVRM0_AVATAR_CONTEXT;

		public string DisplayName => "VRM 0 Avatar";

		public ProcessorContextBase Create(ProcessorState State)
		{
			return new AVAContext(State);
		}
	}

	[InitializeOnLoad, ExecuteInEditMode]
	public class DetectorUNIVRM0
	{
		const string STF_AVA_UNIVRM0_FOUND = "STF_AVA_UNIVRM0_FOUND";
		public const string STF_UNIVRM0_AVATAR_CONTEXT = "univrm0";

		public static readonly List<System.Type> Ignores = new() { typeof(STFEXP_Collider_Sphere), typeof(STFEXP_Collider_Capsule), typeof(STFEXP_Collider_Plane), };

		static DetectorUNIVRM0()
		{
#if STF_AVA_UNIVRM0_FOUND
			Debug.Log("AVA: Found UNIVRM0 SDK");
			STF_Processor_Registry.RegisterContext(new UNIVRM0ContextFactory());

			foreach((var _, var processor) in STF_Processor_Registry.GetProcessors("default"))
				if(!Ignores.Contains(processor.TargetType))
					STF_Processor_Registry.RegisterProcessor(STF_UNIVRM0_AVATAR_CONTEXT, processor);
#endif
		}
	}
}

#endif
