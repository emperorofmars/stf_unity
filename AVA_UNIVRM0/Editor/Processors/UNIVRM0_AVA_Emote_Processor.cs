#if UNITY_EDITOR
#if STF_AVA_UNIVRM0_FOUND

using UnityEditor;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.ava.univrm0;
using VRM;
using UnityEngine;

namespace com.squirrelbite.stf_unity.ava.vrchat.processors
{
	public class UNIVRM0_AVA_Emote_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(AVA_Emotes);

		public const uint _Order = 1000;
		public uint Order => _Order;

		public int Priority => 1;

		public (List<Object>, List<Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var avaEmotes = STFResource as AVA_Emotes;

			var blendshapeProxy = Context.Root.GetComponent<VRMBlendShapeProxy>();
			if (!blendshapeProxy) Context.Report(new STFReport("No Blendshape Proxy Component created!", ErrorSeverity.FATAL_ERROR, AVA_Visemes_Blendshape._STF_Type));

			foreach (var emote in avaEmotes.emotes)
			{
				var values = new List<(SkinnedMeshRenderer, List<(string, float)>)>();
				foreach (var emoteBlendshape in emote.BlendshapeFallback)
				{
					if (values.Find(v => v.Item1 == emoteBlendshape.MeshInstance.GetComponent<SkinnedMeshRenderer>()) is var item && item.Item1 != null)
					{
						item.Item2.Add((emoteBlendshape.Name, emoteBlendshape.Value * 100));
					}
					else
					{
						values.Add((emoteBlendshape.MeshInstance.GetComponent<SkinnedMeshRenderer>(), new List<(string, float)> { (emoteBlendshape.Name, emoteBlendshape.Value * 100) }));
					}
				}

				var preset = emote.meaning switch
				{
					"smile" => BlendShapePreset.Joy,
					"happy" => BlendShapePreset.Joy,
					"blep" => BlendShapePreset.Fun,
					"silly" => BlendShapePreset.Fun,
					"angry" => BlendShapePreset.Angry,
					"grumpy" => BlendShapePreset.Angry,
					"sad" => BlendShapePreset.Sorrow,
					_ => BlendShapePreset.Unknown,
				};
				var clip = BlendshapeClipUtil.Create(Context, preset, preset != BlendShapePreset.Unknown ? preset.ToString() : emote.meaning, values);
				blendshapeProxy.BlendShapeAvatar.Clips.Add(clip);
				Context.AddUnityObject(avaEmotes, clip);
			}

			return (null, null);
		}
	}

	[InitializeOnLoad]
	public class Register_UNIVRM0_AVA_Emote_Processor
	{
		static Register_UNIVRM0_AVA_Emote_Processor()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorUNIVRM0.STF_UNIVRM0_AVATAR_CONTEXT, new UNIVRM0_AVA_Emote_Processor());
		}
	}
}

#endif
#endif
