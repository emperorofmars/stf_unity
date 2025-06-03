#if UNITY_EDITOR
#if STF_AVA_UNIVRM0_FOUND

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using VRM;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.processors;

namespace com.squirrelbite.stf_unity.ava.univrm0.processors
{
	public class UNIVRM0_AVA_FaceMesh_Processor : ISTF_Processor
	{
		public Type TargetType => typeof(AVA_FaceMesh);

		public uint Order => 1000;

		public int Priority => 1;

		public List<UnityEngine.Object> Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var blendshapeProxy = Context.Root.GetComponent<VRMBlendShapeProxy>();
			if (!blendshapeProxy) Context.Report(new STFReport("No Blendshape Proxy Component created!", ErrorSeverity.FATAL_ERROR, AVA_FaceMesh._STF_Type));

			var faceMesh = STFResource as AVA_FaceMesh;
			var ret = new List<UnityEngine.Object>();

			if (faceMesh.STF_Owner is STF_Node node && node?.Instance is STF_Instance_Mesh mesh && mesh != null && faceMesh.GetComponent<SkinnedMeshRenderer>() is var smr && smr != null)
			{
				if (mesh.Mesh.Components.Find(c => c is AVA_Visemes_Blendshape) is AVA_Visemes_Blendshape visemesBlendshape && visemesBlendshape != null)
				{
					void TryApplyViseme(string Viseme, BlendShapePreset Preset)
					{
						if (visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == Viseme)] is string vis && !string.IsNullOrWhiteSpace(vis))
						{
							var clip = BlendshapeClipUtil.CreateSimple(Context, Preset, smr, vis);
							blendshapeProxy.BlendShapeAvatar.Clips.Add(clip);
							ret.Add(clip);
						}
					}
					TryApplyViseme("aa", BlendShapePreset.A);
					TryApplyViseme("e", BlendShapePreset.E);
					TryApplyViseme("ih", BlendShapePreset.I);
					TryApplyViseme("oh", BlendShapePreset.O);
					TryApplyViseme("ou", BlendShapePreset.U);
				}
			}

			return ret;
		}
	}

	[InitializeOnLoad]
	public class Register_UNIVRM0_AVA_FaceMesh
	{
		static Register_UNIVRM0_AVA_FaceMesh()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorUNIVRM0.STF_UNIVRM0_AVATAR_CONTEXT, new UNIVRM0_AVA_FaceMesh_Processor());
		}
	}
}

#endif
#endif
