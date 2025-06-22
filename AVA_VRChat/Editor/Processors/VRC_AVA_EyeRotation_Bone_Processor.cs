#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using UnityEngine;
using VRC.SDK3.Avatars.Components;
using UnityEditor;
using System;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using System.Linq;
using System.Threading.Tasks;
using com.squirrelbite.stf_unity.modules;

namespace com.squirrelbite.stf_unity.ava.vrchat.processors
{
	public class VRC_AVA_EyeRotation_Bone_Processor : ISTF_GlobalProcessor
	{
		public Type TargetType => typeof(AVA_EyeRotation_Bone);

		public const uint _Order = 1000;
		public uint Order => _Order;

		public int Priority => 1;

		public List<UnityEngine.Object> Process(ProcessorContextBase Context)
		{
			var avatar = Context.Root.GetComponent<VRCAvatarDescriptor>();
			var animator = Context.Root.GetComponent<Animator>();
			if (!avatar) Context.Report(new STFReport("No Avatar Component created!", ErrorSeverity.FATAL_ERROR, AVA_EyeRotation_Bone._STF_Type));
			if (!animator) Context.Report(new STFReport("No Animator Component created!", ErrorSeverity.FATAL_ERROR, AVA_EyeRotation_Bone._STF_Type));

			var eyeRotation = (Context as AVAContext).PrimaryArmatureInstance?.Armature.Components.Find(c => c.GetType() == typeof(AVA_EyeRotation_Bone)) as AVA_EyeRotation_Bone;

			var humanEyeL = animator.avatar.humanDescription.human.FirstOrDefault(hb => hb.humanName == HumanBodyBones.LeftEye.ToString());
			var humanEyeR = animator.avatar.humanDescription.human.FirstOrDefault(hb => hb.humanName == HumanBodyBones.RightEye.ToString());

			if(humanEyeL.boneName != null && humanEyeR.boneName != null)
			{
				avatar.enableEyeLook = true;

				avatar.customEyeLookSettings.leftEye = animator.avatarRoot.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == humanEyeL.boneName);
				avatar.customEyeLookSettings.rightEye = animator.avatarRoot.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == humanEyeR.boneName);

				avatar.customEyeLookSettings.eyesLookingUp = new VRCAvatarDescriptor.CustomEyeLookSettings.EyeRotations
						{left = Quaternion.Euler(-eyeRotation.limits_up, 0f, 0f), right = Quaternion.Euler(-eyeRotation.limits_up, 0f, 0f), linked = true};
				avatar.customEyeLookSettings.eyesLookingDown = new VRCAvatarDescriptor.CustomEyeLookSettings.EyeRotations
						{left = Quaternion.Euler(eyeRotation.limits_down, 0f, 0f), right = Quaternion.Euler(eyeRotation.limits_down, 0f, 0f), linked = true};
				avatar.customEyeLookSettings.eyesLookingLeft = new VRCAvatarDescriptor.CustomEyeLookSettings.EyeRotations
						{left = Quaternion.Euler(0f, -eyeRotation.limits_out, 0f), right = Quaternion.Euler(0f, -eyeRotation.limits_in, 0f), linked = Mathf.Approximately(eyeRotation.limits_out, eyeRotation.limits_in)};
				avatar.customEyeLookSettings.eyesLookingRight = new VRCAvatarDescriptor.CustomEyeLookSettings.EyeRotations
						{left = Quaternion.Euler(0f, eyeRotation.limits_in, 0f), right = Quaternion.Euler(0f, eyeRotation.limits_out, 0f), linked = Mathf.Approximately(eyeRotation.limits_out, eyeRotation.limits_in)};

				eyeRotation.ProcessedObjects.Add(avatar);
			}

			return null;
		}
	}

	[InitializeOnLoad]
	public class Register_VRC_AVA_EyeRotation_Bone
	{
		static Register_VRC_AVA_EyeRotation_Bone()
		{
			STF_Processor_Registry.RegisterGlobalProcessor(DetectorVRC.STF_VRC_AVATAR_CONTEXT, new VRC_AVA_EyeRotation_Bone_Processor());
		}
	}
}

#endif
#endif
