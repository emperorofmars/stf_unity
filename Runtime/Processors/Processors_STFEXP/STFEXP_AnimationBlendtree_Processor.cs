using System.Collections.Generic;
using com.squirrelbite.stf_unity.resources;
using com.squirrelbite.stf_unity.resources.stfexp;
using UnityEditor.Animations;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	public class STFEXP_AnimationBlendtree_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STFEXP_AnimationBlendtree);
		public uint Order => STF_Animation_Processor._Order + 100;
		public int Priority => 1;

		public (List<Object>, List<Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var stfBlendtree = STFResource as STFEXP_AnimationBlendtree;
			var ret = new BlendTree {
				name = stfBlendtree.STF_Name,
				blendType = stfBlendtree.blendtree_type == "2d" ? BlendTreeType.FreeformDirectional2D : BlendTreeType.Simple1D,
			};
			foreach (var mapping in stfBlendtree.Animations)
			{
				ret.AddChild(mapping.Animation.ProcessedObjects.Find(e => e is AnimationClip) as AnimationClip, mapping.Position);
			}

			return (new() { ret }, new() { ret });
		}
	}
}
