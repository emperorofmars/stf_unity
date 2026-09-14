#if UNITY_EDITOR
#if STF_AVA_VRM1_FOUND

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using UniVRM10;
using com.squirrelbite.stf_unity.processors;

namespace com.squirrelbite.stf_unity.ava.vrm1
{
	public class VRM1_AVA_EyeRotation_Bone_Processor : ISTF_GlobalProcessor
	{
		public Type TargetType => typeof(AVA_EyeRotation_Bone);

		public uint Order => 1000;

		public int Priority => 1;

		public List<UnityEngine.Object> Process(ProcessorContextBase Context)
		{
			var animator = Context.Root.GetComponent<Animator>();
			var avatar = Context.Root.GetComponent<Vrm10Instance>();
			if (!avatar)
			{
				Context.Report(new STFReport("No VRM Instance Component created!", ErrorSeverity.ERROR, AVA_EyeRotation_Bone._STF_Type));
				return null;
			}
			if (!animator)
			{
				Context.Report(new STFReport("No Animator Component created!", ErrorSeverity.ERROR, AVA_EyeRotation_Bone._STF_Type));
				return null;
			}
			if (!animator.avatar)
			{
				Context.Report(new STFReport("No Unity Avatar created!", ErrorSeverity.ERROR, AVA_EyeRotation_Bone._STF_Type));
				return null;
			}
			if (!animator.avatar.isHuman)
			{
				Context.Report(new STFReport("Unity Avatar is not human!", ErrorSeverity.ERROR, AVA_EyeRotation_Bone._STF_Type));
				return null;
			}
			var avaContext = Context as AVAContext;

			var eyeRotation = avaContext.PrimaryArmatureInstance ? avaContext.PrimaryArmatureInstance.Armature.Components.Find(c => c.GetType() == typeof(AVA_EyeRotation_Bone)) as AVA_EyeRotation_Bone : null;

			if(eyeRotation)
			{
				avatar.DrawLookAtGizmo = true;
				avatar.Vrm.LookAt.LookAtType = UniGLTF.Extensions.VRMC_vrm.LookAtType.bone;
				// This implementation could be wrong. The VRM documentation on this is effectively non existent: https://vrm.dev/en/univrm/lookat/lookat_bone/
				avatar.Vrm.LookAt.VerticalUp.CurveYRangeDegree = eyeRotation.limits_up * Mathf.Rad2Deg;
				avatar.Vrm.LookAt.VerticalDown.CurveYRangeDegree = eyeRotation.limits_down * Mathf.Rad2Deg;
				avatar.Vrm.LookAt.HorizontalInner.CurveYRangeDegree = eyeRotation.limits_in * Mathf.Rad2Deg;
				avatar.Vrm.LookAt.HorizontalOuter.CurveYRangeDegree = eyeRotation.limits_out * Mathf.Rad2Deg;
			}

			return null;
		}
	}

	[InitializeOnLoad]
	public class Register_VRM1_AVA_EyeRotation_Bone_Processor
	{
		static Register_VRM1_AVA_EyeRotation_Bone_Processor()
		{
			STF_Processor_Registry.RegisterGlobalProcessor(DetectorVRM1.STF_VRM1_AVATAR_CONTEXT, new VRM1_AVA_EyeRotation_Bone_Processor());
		}
	}
}

#endif
#endif
