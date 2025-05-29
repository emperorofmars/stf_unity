using UnityEngine;
using System;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.stfexp;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.processors.stfexp
{
	public class STFEXP_Humanoid_Armature_Processor : ISTF_Processor
	{
		public Type TargetType => typeof(STFEXP_Humanoid_Armature);

		public uint Order => 10;

		public int Priority => 1;

		public List<UnityEngine.Object> Process(ProcessorContext Context, ISTF_Resource STFResource)
		{
			var humanoid = STFResource as STFEXP_Humanoid_Armature;

			Debug.Log("WOOOOOOOOOOOOOOOOOOOOOOO");

			var avatar = UnityHumanoidMappingUtil.GenerateAvatar(Context, humanoid.transform, humanoid.locomotion_type, humanoid.no_jaw);

			avatar.name = "Unity Avatar";

			Animator animator = Context.Root.GetComponent<Animator>();
			if(!animator) animator = Context.Root.AddComponent<Animator>();

			animator.applyRootMotion = true;
			animator.updateMode = AnimatorUpdateMode.Normal;
			animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
			animator.avatar = avatar;

			Debug.Log(avatar);

			return new() { avatar };
		}
	}

#if UNITY_EDITOR
	[InitializeOnLoad]
	public class Register_STFEXP_Humanoid_Armature_Processor
	{
		static Register_STFEXP_Humanoid_Armature_Processor()
		{
			STF_Processor_Registry.RegisterProcessor("default", new STFEXP_Humanoid_Armature_Processor());
		}
	}
#endif
}