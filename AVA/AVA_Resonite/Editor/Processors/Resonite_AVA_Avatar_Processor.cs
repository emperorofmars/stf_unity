#if UNITY_EDITOR
#if STF_AVA_RESONITE_FOUND

using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.resources;
using UnityEditor;

namespace com.squirrelbite.stf_unity.ava.resonite
{
	public class Resonite_AVA_Avatar_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(AVA_Avatar);

		public const uint _Order = 100;
		public uint Order => _Order;

		public int Priority => 1;

		public (List<UnityEngine.Object>, List<UnityEngine.Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var avaAvatar = STFResource as AVA_Avatar;

			var avatar = Context.Root.AddComponent<BipedAvatarDescriptor>();


			avatar.enabled = avaAvatar.enabled;

			return (new() { avatar }, null);
		}
	}

	[InitializeOnLoad]
	public class Register_Resonite_AVA_Avatar_Processor
	{
		static Register_Resonite_AVA_Avatar_Processor()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorResonite.STF_RESONITE_AVATAR_CONTEXT, new Resonite_AVA_Avatar_Processor());
		}
	}
}

#endif
#endif
