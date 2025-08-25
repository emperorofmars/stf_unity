#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND
#if D4RKPL4Y3R_AVATAROPTIMIZER_FOUND

using VRC.SDK3.Avatars.Components;
using UnityEditor;
using System;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;

namespace com.squirrelbite.stf_unity.ava.vrchat.processors
{
	public class VRC_AVA_AvatarOptimizer_Processor : ISTF_GlobalProcessor
	{
		public Type TargetType => typeof(d4rkAvatarOptimizer);

		public const uint _Order = 110;
		public uint Order => _Order;

		public int Priority => 1;

		public List<UnityEngine.Object> Process(ProcessorContextBase Context)
		{
			var avatar = Context.Root.GetComponent<VRCAvatarDescriptor>();
			if (!avatar) Context.Report(new STFReport("No Avatar Component created!", ErrorSeverity.FATAL_ERROR, AVA_Visemes_Blendshape._STF_Type));

			var optimizer = avatar.gameObject.AddComponent<d4rkAvatarOptimizer>();

			return new List<UnityEngine.Object> { optimizer };
		}
	}

	[InitializeOnLoad]
	public class Register_VRC_AVA_AvatarOptimizer_Processor
	{
		static Register_VRC_AVA_AvatarOptimizer_Processor()
		{
			STF_Processor_Registry.RegisterGlobalProcessor(DetectorVRC.STF_VRC_AVATAR_CONTEXT, new VRC_AVA_AvatarOptimizer_Processor());
		}
	}
}

#endif
#endif
#endif
