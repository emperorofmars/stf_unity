#if UNITY_EDITOR
#if STF_AVA_UNIVRM0_FOUND

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using VRM;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.processors;
using System.Linq;

namespace com.squirrelbite.stf_unity.ava.univrm0.processors
{
	public class UNIVRM0_AVA_Visemes_Blendshape_Processor : ISTF_GlobalProcessor
	{
		public Type TargetType => typeof(AVA_Visemes_Blendshape);

		public uint Order => 1000;

		public int Priority => 1;

		public List<UnityEngine.Object> Process(ProcessorContextBase Context)
		{
			var blendshapeProxy = Context.Root.GetComponent<VRMBlendShapeProxy>();
			if (!blendshapeProxy) Context.Report(new STFReport("No Blendshape Proxy Component created!", ErrorSeverity.FATAL_ERROR, AVA_Visemes_Blendshape._STF_Type));

			var stfMeshInstance = (Context as AVAContext).PrimaryMeshInstance;
			var smr = stfMeshInstance ? stfMeshInstance.GetComponent<SkinnedMeshRenderer>() : null;
			var visemesBlendshape = stfMeshInstance ? stfMeshInstance.Mesh.Components.Find(c => c.GetType() == typeof(AVA_Visemes_Blendshape)) as AVA_Visemes_Blendshape : null;

			if (!visemesBlendshape)
			{
				var visemesBlendshapeResources = Context.GetResourceByType(typeof(AVA_Visemes_Blendshape));
				if (visemesBlendshapeResources.Count == 1)
				{
					visemesBlendshape = visemesBlendshapeResources[0] as AVA_Visemes_Blendshape;
				}
			}

			if (!smr)
			{
				var mesh = visemesBlendshape.ParentObject as STF_Mesh;
				var meshInstance = Context.Root.GetComponentsInChildren<STF_Instance_Mesh>().FirstOrDefault(e => e.Mesh == mesh);
				if (meshInstance)
					smr = meshInstance.GetComponent<SkinnedMeshRenderer>();
			}

			if (smr && visemesBlendshape)
			{
				Setup(Context, blendshapeProxy, visemesBlendshape, smr);
			}

			return null;
		}

		private void Setup(ProcessorContextBase Context, VRMBlendShapeProxy blendshapeProxy, AVA_Visemes_Blendshape visemesBlendshape, SkinnedMeshRenderer smr)
		{
			void TryApplyViseme(string Viseme, BlendShapePreset Preset)
			{
				if (visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == Viseme)] is string vis && !string.IsNullOrWhiteSpace(vis))
				{
					var clip = BlendshapeClipUtil.CreateSimple(Context, Preset, smr, vis);
					blendshapeProxy.BlendShapeAvatar.Clips.Add(clip);
					Context.AddUnityObject(visemesBlendshape, clip);
				}
			}
			TryApplyViseme("aa", BlendShapePreset.A);
			TryApplyViseme("e", BlendShapePreset.E);
			TryApplyViseme("ih", BlendShapePreset.I);
			TryApplyViseme("oh", BlendShapePreset.O);
			TryApplyViseme("ou", BlendShapePreset.U);
		}
	}

	[InitializeOnLoad]
	public class Register_UNIVRM0_AVA_Visemes_Blendshape_Processor
	{
		static Register_UNIVRM0_AVA_Visemes_Blendshape_Processor()
		{
			STF_Processor_Registry.RegisterGlobalProcessor(DetectorUNIVRM0.STF_UNIVRM0_AVATAR_CONTEXT, new UNIVRM0_AVA_Visemes_Blendshape_Processor());
		}
	}
}

#endif
#endif
