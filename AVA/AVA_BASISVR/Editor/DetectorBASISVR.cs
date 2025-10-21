#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.processors.stfexp;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules.stfexp;

namespace com.squirrelbite.stf_unity.ava.basisvr
{
	public class BasisContextFactory : STF_ApplicationContextDefinition
	{
		public string ContextId => DetectorBASISVR.STF_BASISVR_AVATAR_CONTEXT;

		public string DisplayName => "Basis Avatar";

		public ProcessorContextBase Create(ProcessorState State)
		{
			return new AVAContext(State);
		}
	}

	[InitializeOnLoad, ExecuteInEditMode]
	public class DetectorBASISVR
	{
		public const string STF_BASISVR_AVATAR_CONTEXT = "basisvr";

		public static readonly List<System.Type> Ignores = new() { typeof(STFEXP_Collider_Sphere), typeof(STFEXP_Collider_Capsule), typeof(STFEXP_Collider_Plane), };

		static DetectorBASISVR()
		{
#if STF_AVA_BASISVR_FOUND
			Debug.Log("AVA: Found BASISVR SDK");
			STF_Processor_Registry.RegisterContext(new BasisContextFactory());
					
			foreach ((var _, var processor) in STF_Processor_Registry.GetProcessors("default"))
				if(!Ignores.Contains(processor.TargetType))
					STF_Processor_Registry.RegisterProcessor(STF_BASISVR_AVATAR_CONTEXT, processor);
#else
			Debug.Log("AVA: Didn't find BASISVR SDK");
#endif
		}
	}
}

#endif
