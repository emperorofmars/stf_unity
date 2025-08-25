#if UNITY_EDITOR
#if STF_AVA_UNIVRM0_FOUND

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.modules;
using System.Linq;
using VRM;

namespace com.squirrelbite.stf_unity.ava.univrm0.processors
{
	public class UNIVRM0_AVA_Eyelids_Blendshape_Processor : ISTF_GlobalProcessor
	{
		public Type TargetType => typeof(AVA_Eyelids_Blendshape);

		public const uint _Order = 1000;
		public uint Order => _Order;

		public int Priority => 1;

		public List<UnityEngine.Object> Process(ProcessorContextBase Context)
		{
			var blendshapeProxy = Context.Root.GetComponent<VRMBlendShapeProxy>();
			if (!blendshapeProxy) Context.Report(new STFReport("No Blendshape Proxy Component created!", ErrorSeverity.FATAL_ERROR, AVA_Visemes_Blendshape._STF_Type));

			var stfMeshInstance = (Context as AVAContext).PrimaryMeshInstance;
			var smr = stfMeshInstance ? stfMeshInstance.GetComponent<SkinnedMeshRenderer>() : null;
			var eyelidBlendshape = stfMeshInstance ? stfMeshInstance.Mesh.Components.Find(c => c.GetType() == typeof(AVA_Eyelids_Blendshape)) as AVA_Eyelids_Blendshape : null;

			if (!eyelidBlendshape)
			{
				var eyelidBlendshapeResources = Context.GetResourceByType(typeof(AVA_Eyelids_Blendshape));
				if (eyelidBlendshapeResources.Count == 1)
				{
					eyelidBlendshape = eyelidBlendshapeResources[0] as AVA_Eyelids_Blendshape;
				}
			}

			if (!smr && eyelidBlendshape)
			{
				var mesh = eyelidBlendshape.ParentObject as STF_Mesh;
				var meshInstance = Context.Root.GetComponentsInChildren<STF_Instance_Mesh>().FirstOrDefault(e => e.Mesh == mesh);
				if (meshInstance)
					smr = meshInstance.GetComponent<SkinnedMeshRenderer>();
			}

			if (smr && eyelidBlendshape)
			{
				Setup(Context, blendshapeProxy, eyelidBlendshape, smr);
			}

			return null;
		}

		private void Setup(ProcessorContextBase Context, VRMBlendShapeProxy blendshapeProxy, AVA_Eyelids_Blendshape eyelidsBlendshape, SkinnedMeshRenderer smr)
		{
			void TryApplyShape(string Shape, BlendShapePreset Preset)
			{
				if (eyelidsBlendshape.Mapppings[Array.FindIndex(AVA_Eyelids_Blendshape._EyelidShapes, e => e == Shape)] is string vis && !string.IsNullOrWhiteSpace(vis))
				{
					var clip = BlendshapeClipUtil.CreateSimple(Context, Preset, smr, vis);
					blendshapeProxy.BlendShapeAvatar.Clips.Add(clip);
					Context.AddUnityObject(eyelidsBlendshape, clip);
				}
			}
			TryApplyShape("closed", BlendShapePreset.Blink);
			TryApplyShape("closed_left", BlendShapePreset.Blink_L);
			TryApplyShape("closed_right", BlendShapePreset.Blink_R);
			TryApplyShape("up", BlendShapePreset.LookUp);
			TryApplyShape("down", BlendShapePreset.LookDown);
			TryApplyShape("left", BlendShapePreset.LookLeft);
			TryApplyShape("right", BlendShapePreset.LookRight);
		}
	}

	[InitializeOnLoad]
	public class Register_UNIVRM0_AVA_Eyelids_Blendshape_Processor
	{
		static Register_UNIVRM0_AVA_Eyelids_Blendshape_Processor()
		{
			STF_Processor_Registry.RegisterGlobalProcessor(DetectorUNIVRM0.STF_UNIVRM0_AVATAR_CONTEXT, new UNIVRM0_AVA_Eyelids_Blendshape_Processor());
		}
	}
}

#endif
#endif
