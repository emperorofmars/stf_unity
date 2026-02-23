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
	public class BASIS_AVA_VoicePosition_Processor : ISTF_Processor
	{
		public Type TargetType => typeof(AVA_VoicePosition);

		public const uint _Order = AVA_Avatar_BASIS_Processor._Order + 10;
		public uint Order => _Order;

		public int Priority => 1;

		public (List<UnityEngine.Object>, List<UnityEngine.Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var avatar = Context.Root.GetComponent<BasisAvatar>();
			if (!avatar) Context.Report(new STFReport("No Avatar Component created!", ErrorSeverity.FATAL_ERROR, AVA_Visemes_Blendshape._STF_Type));

			var avaVoice = STFResource as AVA_VoicePosition;

			if (avaVoice.VoicePosition)
			{
				avatar.AvatarMouthPosition.x = (avaVoice.VoicePosition.transform.position - Context.Root.transform.position).y;
				avatar.AvatarMouthPosition.y = (avaVoice.VoicePosition.transform.position - Context.Root.transform.position).z;
				if (!Context.ImportConfig.AuthoringImport)
					Context.AddTrash(avaVoice.VoicePosition);
			}

			return (new() { avatar }, null);
		}
	}

	[InitializeOnLoad]
	public class Register_AVA_VoicePosition_BASIS
	{
		static Register_AVA_VoicePosition_BASIS()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorBASISVR.STF_BASISVR_AVATAR_CONTEXT, new BASIS_AVA_VoicePosition_Processor());
		}
	}
}

#endif
#endif
