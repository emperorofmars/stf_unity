#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using UnityEditor;
using System;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.ava.vrchat.util;
using UnityEngine;

namespace com.squirrelbite.stf_unity.ava.vrchat.processors
{
	public class VRC_AVA_Emotes_Processor : ISTF_Processor
	{
		public Type TargetType => typeof(AVA_Emotes);

		public const uint _Order = STF_Animation_Processor._Order + 1000;
		public uint Order => _Order;

		public int Priority => 1;

		public (List<UnityEngine.Object>, List<UnityEngine.Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var avaEmotes = STFResource as AVA_Emotes;
			var baseSetup = Context.Root.GetComponent<AVA_AvatarBehaviourSetup>();

			foreach (var emote in avaEmotes.emotes)
			{
				if (emote.animation.ProcessedObjects.Count == 1 && emote.animation.ProcessedObjects[0] is AnimationClip animationClip)
				{
					baseSetup.AddAvatarEmote(new AvatarEmote() {
						Emote = emote.meaning,
						Animation = animationClip,
					});
				}
			}

			return (new() { baseSetup }, null);
		}
	}

	[InitializeOnLoad]
	public class Register_VRC_AVA_Emotes_Processor
	{
		static Register_VRC_AVA_Emotes_Processor()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorVRC.STF_VRC_AVATAR_CONTEXT, new VRC_AVA_Emotes_Processor());
		}
	}
}

#endif
#endif
