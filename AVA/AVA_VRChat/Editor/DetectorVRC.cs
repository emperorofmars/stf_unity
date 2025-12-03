#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.processors.stfexp;
using com.squirrelbite.stf_unity.modules.stfexp;
using System.Collections.Generic;

namespace com.squirrelbite.stf_unity.ava.vrchat
{
	public class VRCContextFactory : STF_ApplicationContextDefinition
	{
		public string ContextId => DetectorVRC.STF_VRC_AVATAR_CONTEXT;

		public string DisplayName => "VRChat Avatar";

		public ProcessorContextBase Create(ProcessorState State)
		{
			return new AVAContext(State);
		}
	}

	[InitializeOnLoad, ExecuteInEditMode]
	public class DetectorVRC
	{
		public const string STF_VRC_AVATAR_CONTEXT = "vrchat_avatar3";

		public static readonly List<System.Type> Ignores = new() { typeof(STFEXP_Collider_Sphere), typeof(STFEXP_Collider_Capsule), typeof(STFEXP_Collider_Plane), typeof(STFEXP_Constraint_Twist), typeof(STFEXP_Constraint_Rotation), };

		static DetectorVRC()
		{
#if STF_AVA_VRCSDK3_FOUND
			Debug.Log("AVA: Found VRC SDK 3");
			STF_Processor_Registry.RegisterContext(new VRCContextFactory());

			foreach ((var _, var processor) in STF_Processor_Registry.GetProcessors("default"))
				if(!Ignores.Contains(processor.TargetType))
					STF_Processor_Registry.RegisterProcessor(STF_VRC_AVATAR_CONTEXT, processor);
#else
			Debug.Log("AVA: Didn't find VRC SDK 3");
#endif
		}
	}
}

#endif
