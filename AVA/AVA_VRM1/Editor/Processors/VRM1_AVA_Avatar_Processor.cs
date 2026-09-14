#if UNITY_EDITOR
#if STF_AVA_VRM1_FOUND

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using UniVRM10;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.resources;
using System.Linq;

namespace com.squirrelbite.stf_unity.ava.vrm1
{
	public class VRM1_AVA_Avatar_Processor : ISTF_Processor
	{
		public Type TargetType => typeof(AVA_Avatar);

		public uint Order => 100;

		public int Priority => 1;

		public (List<UnityEngine.Object>, List<UnityEngine.Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var avaAvatar = STFResource as AVA_Avatar;

			if (!Context.Root.TryGetComponent<Animator>(out var animator))
			{
				animator = Context.Root.AddComponent<Animator>();
			}
			animator.applyRootMotion = true;
			animator.updateMode = AnimatorUpdateMode.Normal;
			animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;

			var vrmMetaComponent = Context.Root.AddComponent<Vrm10Instance>();
			var vrmMeta = ScriptableObject.CreateInstance<VRM10Object>();
			vrmMeta.name = "VRM_Meta";
			vrmMetaComponent.Vrm = vrmMeta;

			if (Context.GetMeta() is var meta && meta != null)
			{
				vrmMeta.Meta.Name = meta.AssetName;
				vrmMeta.Meta.Authors = new List<string> { meta.Author };
				vrmMeta.Meta.Version = meta.Version;
				vrmMeta.Meta.ContactInformation = meta.URL;
				vrmMeta.Meta.OtherLicenseUrl = meta.LicenseURL;
				vrmMeta.Meta.References = new List<string> { meta.DocumentationURL };
			}
			else
			{
				vrmMeta.Meta.Name = Context.Root.name;
				vrmMeta.Meta.Version = "0.0.1";
			}

			/*var vrmBlendshapeProxy = Context.Root.AddComponent<VRMBlendShapeProxy>();
			var vrmBlendShapeAvatar = ScriptableObject.CreateInstance<BlendShapeAvatar>();
			vrmBlendShapeAvatar.name = "VRM_BlendshapeAvatar";

			vrmBlendshapeProxy.BlendShapeAvatar = vrmBlendShapeAvatar;

			var neutralClip = BlendshapeClipUtil.CreateEmpty(BlendShapePreset.Neutral);
			vrmBlendShapeAvatar.Clips.Add(neutralClip);
			*/

			var secondary = new GameObject {name = "VRM_secondary"};
			secondary.transform.SetParent(Context.Root.transform, false);
			(Context as AVAContext).AddMessage("VRM_secondary", secondary);

			if (avaAvatar.Viewport)
			{
				vrmMeta.FirstPerson.SetDefault(avaAvatar.Viewport.transform.parent);
				vrmMeta.LookAt.OffsetFromHead = avaAvatar.Viewport.transform.localPosition;
			}
			else if (animator && animator.isHuman)
			{
				var headHumanoid = animator.avatar.humanDescription.human.FirstOrDefault(hb => hb.humanName == HumanBodyBones.Head.ToString());
				if (headHumanoid.boneName != null)
				{
					vrmMeta.FirstPerson.SetDefault(Context.Root.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == headHumanoid.boneName));
				}
			}

			vrmMetaComponent.enabled = avaAvatar.enabled;

			/*
			vrmBlendshapeProxy.enabled = avaAvatar.enabled;

			return (new() { vrmMetaComponent, vrmBlendshapeProxy, vrmFirstPerson, vrmLookAt, vrmMeta, vrmBlendShapeAvatar, neutralClip }, new() { vrmMeta, vrmBlendShapeAvatar, neutralClip });
			*/

			return (new() { vrmMetaComponent, vrmMeta }, new() { vrmMeta });
		}
	}

	[InitializeOnLoad]
	public class Register_UNIVRM0_AVA_Avatar
	{
		static Register_UNIVRM0_AVA_Avatar()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorVRM1.STF_VRM1_AVATAR_CONTEXT, new VRM1_AVA_Avatar_Processor());
		}
	}
}

#endif
#endif
