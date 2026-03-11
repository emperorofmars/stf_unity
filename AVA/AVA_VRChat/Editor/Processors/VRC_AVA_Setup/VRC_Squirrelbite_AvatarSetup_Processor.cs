#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND
#if AVA_BASE_SETUP_FOUND

using UnityEditor;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.resources;
using UnityEngine;
using com.squirrelbite.ava_base_setup.vrchat;
using com.squirrelbite.ava_base_setup;
using VRC.SDK3.Avatars.Components;
using com.squirrelbite.stf_unity.squirrelbite;

namespace com.squirrelbite.stf_unity.ava.vrchat.processors
{
	public class VRC_Squirrelbite_AvatarSetup_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(Squirrelbite_AvatarSetup);

		public const uint _Order = STF_Animation_Processor._Order + 1000;
		public uint Order => _Order;

		public int Priority => 1;

		public (List<UnityEngine.Object>, List<UnityEngine.Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var avatarSetup = STFResource as Squirrelbite_AvatarSetup;
			var baseSetup = InitAvatarBaseSetupVRChat.Init(Context.Root.GetComponent<VRCAvatarDescriptor>());


			return (new() { baseSetup }, null);
		}
	}

	[InitializeOnLoad]
	public class Register_VRC_Squirrelbite_AvatarSetup_Processor
	{
		static Register_VRC_Squirrelbite_AvatarSetup_Processor()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorVRC.STF_VRC_AVATAR_CONTEXT, new VRC_Squirrelbite_AvatarSetup_Processor());
		}
	}
}

#endif
#endif
#endif
