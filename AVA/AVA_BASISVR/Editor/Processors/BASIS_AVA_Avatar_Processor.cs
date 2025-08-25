#if UNITY_EDITOR
#if STF_AVA_BASISVR_FOUND

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.modules;
using Basis.Scripts.BasisSdk;

namespace com.squirrelbite.stf_unity.ava.basisvr.processors
{
	public class AVA_Avatar_BASIS_Processor : ISTF_Processor
	{
		public Type TargetType => typeof(AVA_Avatar);

		public const uint _Order = 100;
		public uint Order => _Order;

		public int Priority => 1;

		public (List<UnityEngine.Object>, List<UnityEngine.Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var avaAvatar = STFResource as AVA_Avatar;

			var avatar = Context.Root.AddComponent<BasisAvatar>();

			avatar.BasisBundleDescription = new BasisBundleDescription();
			if (Context.GetMeta() is var meta && meta != null)
			{
				avatar.BasisBundleDescription.AssetBundleName = meta.AssetName;
				avatar.BasisBundleDescription.AssetBundleDescription = $"Version {meta.Version}, by {meta.Author}";
			}
			else
			{
				avatar.BasisBundleDescription.AssetBundleName = Context.Root.name;
			}

			if (!Context.Root.TryGetComponent<Animator>(out var animator))
			{
				animator = Context.Root.AddComponent<Animator>();
			}
			animator.applyRootMotion = true;
			animator.updateMode = AnimatorUpdateMode.Normal;
			animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;

			if (avaAvatar.Viewport)
			{
				avatar.AvatarEyePosition.x = (avaAvatar.Viewport.transform.position - Context.Root.transform.position).y;
				avatar.AvatarEyePosition.y = (avaAvatar.Viewport.transform.position - Context.Root.transform.position).z;
			}
			if (!Context.ImportConfig.AuthoringImport)
				Context.AddTrash(avaAvatar.Viewport);
			
			if(avaAvatar.PrimaryMeshInstance)
			{
				avatar.FaceVisemeMesh = avatar.FaceBlinkMesh = avaAvatar.PrimaryMeshInstance.ProcessedObjects.Count > 0 ? avaAvatar.PrimaryMeshInstance.ProcessedObjects[0] as SkinnedMeshRenderer : null;
			}

			avatar.enabled = avaAvatar.enabled;

			return (new() { avatar }, null);
		}
	}

	[InitializeOnLoad]
	public class Register_AVA_Avatar_BASIS
	{
		static Register_AVA_Avatar_BASIS()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorBASISVR.STF_BASISVR_AVATAR_CONTEXT, new AVA_Avatar_BASIS_Processor());
		}
	}
}

#endif
#endif
