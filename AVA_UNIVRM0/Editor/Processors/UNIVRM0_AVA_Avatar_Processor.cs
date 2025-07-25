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

		public (List<UnityEngine.Object>, List<UnityEngine.Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
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
			(Context as AVAContext).AddMessage("VRM_secondary", secondary);


			if (!Context.Root.TryGetComponent<Animator>(out var animator))
			{
				animator = Context.Root.AddComponent<Animator>();
			}
			animator.applyRootMotion = true;
			animator.updateMode = AnimatorUpdateMode.Normal;
			animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;

			VRMFirstPerson vrmFirstPerson = null;
			VRMLookAtHead vrmLookAt = null;
			if (avaAvatar.Viewport)
			{
				vrmFirstPerson = Context.Root.AddComponent<VRMFirstPerson>();
				vrmFirstPerson.FirstPersonBone = avaAvatar.Viewport.transform.parent;
				vrmFirstPerson.FirstPersonOffset = avaAvatar.Viewport.transform.localPosition;

				if (animator && animator.isHuman)
				{
					var headHumanoid = animator.avatar.humanDescription.human.FirstOrDefault(hb => hb.humanName == HumanBodyBones.Head.ToString());
					if (headHumanoid.boneName != null)
					{
						vrmLookAt = Context.Root.AddComponent<VRMLookAtHead>();
						vrmLookAt.Head = Context.Root.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == headHumanoid.boneName);
					}
				}

				if (!Context.ImportConfig.AuthoringImport)
					Context.AddTrash(avaAvatar.Viewport);
			}

			vrmMetaComponent.enabled = avaAvatar.enabled;
			vrmBlendshapeProxy.enabled = avaAvatar.enabled;
			vrmFirstPerson.enabled = avaAvatar.enabled;
			vrmLookAt.enabled = avaAvatar.enabled;

			return (new() { vrmMetaComponent, vrmBlendshapeProxy, vrmFirstPerson, vrmLookAt, vrmMeta, vrmBlendShapeAvatar, neutralClip }, new() { vrmMeta, vrmBlendShapeAvatar, neutralClip });
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
