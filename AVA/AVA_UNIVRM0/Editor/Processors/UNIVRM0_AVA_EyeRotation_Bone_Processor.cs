#if UNITY_EDITOR
#if STF_AVA_UNIVRM0_FOUND

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using VRM;
using System.Linq;
using com.squirrelbite.stf_unity.processors;

namespace com.squirrelbite.stf_unity.ava.univrm0.processors
{
	public class UNIVRM0_AVA_EyeRotation_Bone_Processor : ISTF_GlobalProcessor
	{
		public Type TargetType => typeof(AVA_EyeRotation_Bone);

		public uint Order => 1000;

		public int Priority => 1;

		public List<UnityEngine.Object> Process(ProcessorContextBase Context)
		{
			var animator = Context.Root.GetComponent<Animator>();
			if (!animator) Context.Report(new STFReport("No Animator Component created!", ErrorSeverity.FATAL_ERROR, AVA_EyeRotation_Bone._STF_Type));
			var avaContext = Context as AVAContext;

			var eyeRotation = avaContext.PrimaryArmatureInstance ? avaContext.PrimaryArmatureInstance.Armature.Components.Find(c => c.GetType() == typeof(AVA_EyeRotation_Bone)) as AVA_EyeRotation_Bone : null;

			var humanEyeL = animator.avatar.humanDescription.human.FirstOrDefault(hb => hb.humanName == HumanBodyBones.LeftEye.ToString());
			var humanEyeR = animator.avatar.humanDescription.human.FirstOrDefault(hb => hb.humanName == HumanBodyBones.RightEye.ToString());

			if (eyeRotation && humanEyeL.boneName != null && humanEyeR.boneName != null)
			{
				var eyeL = Context.Root.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == humanEyeL.boneName);
				var eyeR = Context.Root.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == humanEyeR.boneName);

				var vrmLookat = Context.Root.AddComponent<VRMLookAtBoneApplyer>();
				vrmLookat.LeftEye.Transform = eyeL;
				vrmLookat.RightEye.Transform = eyeR;

				// This implementation could be wrong. The VRM documentation on this is effectively non existent: https://vrm.dev/en/univrm/lookat/lookat_bone/
				vrmLookat.VerticalUp.CurveYRangeDegree = eyeRotation.limits_up;
				vrmLookat.VerticalDown.CurveYRangeDegree = eyeRotation.limits_down;
				vrmLookat.HorizontalInner.CurveYRangeDegree = eyeRotation.limits_in;
				vrmLookat.HorizontalOuter.CurveYRangeDegree = eyeRotation.limits_out;

				eyeRotation.ProcessedObjects.Add(vrmLookat);
			}

			return null;
		}
	}

	[InitializeOnLoad]
	public class Register_UNIVRM0_AVA_EyeRotation_Bone_Processor
	{
		static Register_UNIVRM0_AVA_EyeRotation_Bone_Processor()
		{
			STF_Processor_Registry.RegisterGlobalProcessor(DetectorUNIVRM0.STF_UNIVRM0_AVATAR_CONTEXT, new UNIVRM0_AVA_EyeRotation_Bone_Processor());
		}
	}
}

#endif
#endif
