#if UNITY_EDITOR
#if STF_AVA_UNIVRM0_FOUND

using System.Collections.Generic;
using UnityEngine;
using UniVRM10;
using com.squirrelbite.stf_unity.processors;

namespace com.squirrelbite.stf_unity.ava.vrm1
{
	public static class BlendshapeClipUtil
	{
		public static MorphTargetBinding CreateBinding(ProcessorContextBase Context, SkinnedMeshRenderer Renderer, string BlendshapeName, float Weight)
		{
			return new MorphTargetBinding()
			{
				Index = Renderer.sharedMesh.GetBlendShapeIndex(BlendshapeName),
				RelativePath = UnityUtil.getPath(Context.Root.transform, Renderer.transform, true),
				Weight = Weight
			};
		}

		public static VRM10Expression CreateEmpty(ExpressionPreset BlendshapePreset)
		{
			var clip = ScriptableObject.CreateInstance<VRM10Expression>();
			clip.name = "VRM_Clip_" + BlendshapePreset.ToString();
			/*clip.BlendShapeName = BlendshapePreset.ToString();
			clip.Preset = BlendshapePreset;
			clip.Values = new MorphTargetBinding[] {};*/
			return clip;
		}

		public static VRM10Expression CreateSimple(ProcessorContextBase Context, ExpressionKey BlendshapePreset, SkinnedMeshRenderer Renderer, string BlendshapeName)
		{
			var clip = ScriptableObject.CreateInstance<VRM10Expression>();
			clip.name = "VRM_Clip_" + BlendshapePreset.ToString();
			/*clip.BlendShapeName = BlendshapePreset.ToString();
			clip.Preset = BlendshapePreset;*/

			var bindingsList = new MorphTargetBinding[] {CreateBinding(Context, Renderer, BlendshapeName, 100)};
			clip.MorphTargetBindings = bindingsList;

			return clip;
		}

		public static VRM10Expression Create(ProcessorContextBase Context, ExpressionKey BlendshapePreset, string ClipName, List<(SkinnedMeshRenderer Renderer, List<(string Name, float Weight)> Blendshapes)> Blendshapes)
		{
			var clip = ScriptableObject.CreateInstance<VRM10Expression>();
			clip.name = "VRM_Clip_" + ClipName;
			/*clip.BlendShapeName = ClipName;
			clip.Preset = BlendshapePreset;*/

			var bindingList = new List<MorphTargetBinding>();
			foreach(var renderer in Blendshapes) foreach(var blendshape in renderer.Blendshapes)
			{
				bindingList.Add(CreateBinding(Context, renderer.Renderer, blendshape.Name, blendshape.Weight));
			}
			clip.MorphTargetBindings = bindingList.ToArray();
			return clip;
		}
	}
}

#endif
#endif
