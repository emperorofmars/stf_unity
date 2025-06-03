#if UNITY_EDITOR
#if STF_AVA_UNIVRM0_FOUND

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using VRM;
using System.Linq;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.modules;

namespace com.squirrelbite.stf_unity.ava.univrm0.processors
{
	public class UNIVRM0_AVA_Avatar_Processor : ISTF_Processor
	{
		public Type TargetType => typeof(AVA_Avatar);

		public uint Order => 100;

		public int Priority => 1;

		public List<UnityEngine.Object> Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var avaAvatar = STFResource as AVA_Avatar;

			var vrmMetaComponent = Context.Root.AddComponent<VRMMeta>();
			var vrmMeta = ScriptableObject.CreateInstance<VRMMetaObject>();
			vrmMeta.name = "VRM_Meta";
			vrmMetaComponent.Meta = vrmMeta;

			if (Context.GetMeta() is var meta && meta != null)
			{
				vrmMeta.Title = meta.AssetName;
				vrmMeta.Author = meta.Author;
				vrmMeta.Version = meta.Version;
				vrmMeta.ContactInformation = meta.URL;
				vrmMeta.OtherLicenseUrl = meta.LicenseURL;
				vrmMeta.Reference = meta.DocumentationURL;
			}
			else
			{
				vrmMeta.Title = Context.Root.name;
				vrmMeta.Version = "0.0.1";
				vrmMeta.Author = "Unknown";
			}

			var vrmBlendshapeProxy = Context.Root.AddComponent<VRMBlendShapeProxy>();
			var vrmBlendShapeAvatar = ScriptableObject.CreateInstance<BlendShapeAvatar>();
			vrmBlendShapeAvatar.name = "VRM_BlendshapeAvatar";

			vrmBlendshapeProxy.BlendShapeAvatar = vrmBlendShapeAvatar;

			var neutralClip = BlendshapeClipUtil.CreateEmpty(BlendShapePreset.Neutral);
			vrmBlendShapeAvatar.Clips.Add(neutralClip);

			var secondary = new GameObject {name = "VRM_secondary"};
			secondary.transform.SetParent(Context.Root.transform, false);


			if(!Context.Root.TryGetComponent<Animator>(out var animator))
			{
				animator = Context.Root.AddComponent<Animator>();
			}
			animator.applyRootMotion = true;
			animator.updateMode = AnimatorUpdateMode.Normal;
			animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;


			if(avaAvatar.Viewport)
			{
				var vrmFirstPerson = Context.Root.AddComponent<VRMFirstPerson>();
				vrmFirstPerson.FirstPersonBone = avaAvatar.Viewport.transform.parent;
				vrmFirstPerson.FirstPersonOffset = avaAvatar.Viewport.transform.localPosition;

				if(animator && animator.isHuman)
				{
					var headHumanoid = animator.avatar.humanDescription.human.FirstOrDefault(hb => hb.humanName == HumanBodyBones.Head.ToString());
					if(headHumanoid.boneName != null)
					{
						var vrmLookAt = Context.Root.AddComponent<VRMLookAtHead>();
						vrmLookAt.Head = Context.Root.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == headHumanoid.boneName);
					}
				}

				if (!Context.ImportConfig.AuthoringImport)
					Context.AddTrash(avaAvatar.Viewport);
			}

			if (avaAvatar.PrimaryArmatureInstance && avaAvatar.PrimaryArmatureInstance.Instance is STF_Instance_Armature instance && instance != null && instance.Armature)
			{
				if (instance.Armature.Components.Find(c => c is AVA_EyeRotation_Bone) is AVA_EyeRotation_Bone eyeRotation && eyeRotation != null)
				{
					var humanEyeL = animator.avatar.humanDescription.human.FirstOrDefault(hb => hb.humanName == HumanBodyBones.LeftEye.ToString());
					var humanEyeR = animator.avatar.humanDescription.human.FirstOrDefault(hb => hb.humanName == HumanBodyBones.RightEye.ToString());

					if (humanEyeL.boneName != null && humanEyeR.boneName != null)
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
					}
				}
			}


			return new() { vrmMeta, vrmBlendShapeAvatar, neutralClip };
		}
	}

	[InitializeOnLoad]
	public class Register_UNIVRM0_AVA_Avatar
	{
		static Register_UNIVRM0_AVA_Avatar()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorUNIVRM0.STF_UNIVRM0_AVATAR_CONTEXT, new UNIVRM0_AVA_Avatar_Processor());
		}
	}
}

#endif
#endif
