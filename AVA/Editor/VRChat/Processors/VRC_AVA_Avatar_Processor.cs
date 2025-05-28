#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using UnityEngine;
using VRC.SDK3.Avatars.Components;
using System.Linq;
using UnityEditor;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity;
using System;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.ava;

namespace nna.ava.vrchat
{
	public class AVA_Avatar_VRC_Processor : ISTF_Processor
	{
		public Type TargetType => typeof(AVA_Avatar);

		public uint Order => 100;

		public int Priority => 1;

		public List<UnityEngine.Object> Process(ProcessorContext Context, ISTF_Resource STFResource)
		{
			var avaAvatar = STFResource as AVA_Avatar;
			var avatar = Context.Root.AddComponent<VRCAvatarDescriptor>();
			if(!Context.Root.TryGetComponent<Animator>(out var animator))
			{
				animator = Context.Root.AddComponent<Animator>();
			}
			animator.applyRootMotion = true;
			animator.updateMode = AnimatorUpdateMode.Normal;
			animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;

			if(avaAvatar.Viewport) avatar.ViewPosition = avaAvatar.Viewport.transform.position - Context.Root.transform.position;
			if(!Context.ImportConfig.AuthoringImport)
				Context.AddTrash(avaAvatar.Viewport.transform);

			return new() { avatar };
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