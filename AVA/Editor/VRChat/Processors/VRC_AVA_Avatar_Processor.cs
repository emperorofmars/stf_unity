#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using UnityEngine;
using VRC.SDK3.Avatars.Components;
using UnityEditor;
using System;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.modules;

namespace com.squirrelbite.stf_unity.ava.vrchat.processors
{
	public class AVA_Avatar_VRC_Processor : ISTF_Processor
	{
		public Type TargetType => typeof(AVA_Avatar);

		public const uint _Order = 100;
		public uint Order => _Order;

		public int Priority => 1;

		public (List<UnityEngine.Object>, List<UnityEngine.Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var avaAvatar = STFResource as AVA_Avatar;
			Context.Root.AddComponent<VRC.Core.PipelineManager>();
			var avatar = Context.Root.AddComponent<VRCAvatarDescriptor>();

			if (!Context.Root.TryGetComponent<Animator>(out var animator))
			{
				animator = Context.Root.AddComponent<Animator>();
			}
			animator.applyRootMotion = true;
			animator.updateMode = AnimatorUpdateMode.Normal;
			animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;

			if (avaAvatar.Viewport) avatar.ViewPosition = avaAvatar.Viewport.transform.position - Context.Root.transform.position;
			if (!Context.ImportConfig.AuthoringImport)
				Context.AddTrash(avaAvatar.Viewport);

			return (new() { avatar }, null);
		}
	}

	[InitializeOnLoad]
	public class Register_AVA_Avatar_VRC
	{
		static Register_AVA_Avatar_VRC()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorVRC.STF_VRC_AVATAR_CONTEXT, new AVA_Avatar_VRC_Processor());
		}
	}
}

#endif
#endif
