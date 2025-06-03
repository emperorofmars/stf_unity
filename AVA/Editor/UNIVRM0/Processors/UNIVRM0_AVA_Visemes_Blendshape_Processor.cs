#if UNITY_EDITOR
#if STF_AVA_UNIVRM0_FOUND

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using VRM;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.processors;
using System.Threading.Tasks;

namespace com.squirrelbite.stf_unity.ava.univrm0.processors
{
	public class UNIVRM0_AVA_Visemes_Blendshape_Processor : ISTF_Processor
	{
		public Type TargetType => typeof(AVA_Visemes_Blendshape);

		public uint Order => 1000;

		public int Priority => 1;

		public List<UnityEngine.Object> Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var blendshapeProxy = Context.Root.GetComponent<VRMBlendShapeProxy>();
			if (!blendshapeProxy) Context.Report(new STFReport("No Blendshape Proxy Component created!", ErrorSeverity.FATAL_ERROR, AVA_FaceMesh._STF_Type));

			var visemesBlendshape = STFResource as AVA_Visemes_Blendshape;

			foreach (var meshInstance in Context.Root.GetComponentsInChildren<STF_Instance_Mesh>())
			{
				if (meshInstance.Mesh.Components.Contains(visemesBlendshape) && meshInstance.GetComponent<SkinnedMeshRenderer>() is var smr && smr != null)
				{
					(Context as AVAContext).AddTask("visemes.blendshape", STFResource.STF_Id, new Task(() => {
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
					}));
				}
			}

			return null;
		}
	}

	[InitializeOnLoad]
	public class Register_UNIVRM0_AVA_Visemes_Blendshape_Processor
	{
		static Register_UNIVRM0_AVA_Visemes_Blendshape_Processor()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorUNIVRM0.STF_UNIVRM0_AVATAR_CONTEXT, new UNIVRM0_AVA_Visemes_Blendshape_Processor());
		}
	}
}

#endif
#endif
