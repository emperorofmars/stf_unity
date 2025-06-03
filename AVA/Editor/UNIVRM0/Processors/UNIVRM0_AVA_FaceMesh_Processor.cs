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
			var faceMesh = STFResource as AVA_FaceMesh;

			if (faceMesh.STF_Owner is STF_Node node && node?.Instance is STF_Instance_Mesh mesh && mesh != null && faceMesh.GetComponent<SkinnedMeshRenderer>() is var smr && smr != null)
				if (mesh.Mesh.Components.Find(c => c is AVA_Visemes_Blendshape) is AVA_Visemes_Blendshape visemesBlendshape && visemesBlendshape != null)
					(Context as AVAContext).SetPreferred("visemes.blendshape", visemesBlendshape.STF_Id);

			return null;
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
