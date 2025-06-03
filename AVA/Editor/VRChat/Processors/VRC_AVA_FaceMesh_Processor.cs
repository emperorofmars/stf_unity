#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using UnityEngine;
using VRC.SDK3.Avatars.Components;
using UnityEditor;
using System;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.modules;

namespace com.squirrelbite.stf_unity.ava.vrchat.processors
{
	public class VRC_AVA_FaceMesh_Processor : ISTF_Processor
	{
		public Type TargetType => typeof(AVA_FaceMesh);

		public const uint _Order = 1000;
		public uint Order => _Order;

		public int Priority => 1;

		public List<UnityEngine.Object> Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var avatar = Context.Root.GetComponent<VRCAvatarDescriptor>();
			if (!avatar) Context.Report(new STFReport("No Avatar Component created!", ErrorSeverity.FATAL_ERROR, AVA_FaceMesh._STF_Type));

			var faceMesh = STFResource as AVA_FaceMesh;

			if (faceMesh.STF_Owner is STF_Node node && node?.Instance is STF_Instance_Mesh mesh && mesh != null && faceMesh.GetComponent<SkinnedMeshRenderer>() is var smr && smr != null)
				if (mesh.Mesh.Components.Find(c => c is AVA_Visemes_Blendshape) is AVA_Visemes_Blendshape visemesBlendshape && visemesBlendshape != null)
					(Context as AVAContext).SetPreferred("visemes.blendshape", visemesBlendshape.STF_Id);

			return null;
		}
	}

	[InitializeOnLoad]
	public class Register_AVA_FaceMesh_VRC
	{
		static Register_AVA_FaceMesh_VRC()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorVRC.STF_VRC_AVATAR_CONTEXT, new VRC_AVA_FaceMesh_Processor());
		}
	}
}

#endif
#endif
