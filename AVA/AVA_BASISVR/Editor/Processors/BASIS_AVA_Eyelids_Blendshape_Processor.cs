#if UNITY_EDITOR
#if STF_AVA_BASISVR_FOUND

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.modules;
using System.Linq;
using Basis.Scripts.BasisSdk;

namespace com.squirrelbite.stf_unity.ava.basisvr.processors
{
	public class BASIS_AVA_Eyelids_Blendshape_Processor : ISTF_GlobalProcessor
	{
		public Type TargetType => typeof(AVA_Eyelids_Blendshape);

		public const uint _Order = 1000;
		public uint Order => _Order;

		public int Priority => 1;

		public List<UnityEngine.Object> Process(ProcessorContextBase Context)
		{
			var avatar = Context.Root.GetComponent<BasisAvatar>();
			if (!avatar) Context.Report(new STFReport("No Avatar Component created!", ErrorSeverity.FATAL_ERROR, AVA_Eyelids_Blendshape._STF_Type));

			var stfMeshInstance = (Context as AVAContext).PrimaryMeshInstance;
			var smr = stfMeshInstance ? stfMeshInstance.GetComponent<SkinnedMeshRenderer>() : null;
			var eyelidBlendshape = stfMeshInstance ? stfMeshInstance.Mesh.Components.Find(c => c.GetType() == typeof(AVA_Eyelids_Blendshape)) as AVA_Eyelids_Blendshape : null;

			if (!eyelidBlendshape)
			{
				var eyelidBlendshapeResources = Context.GetResourceByType(typeof(AVA_Eyelids_Blendshape));
				if (eyelidBlendshapeResources != null && eyelidBlendshapeResources.Count == 1)
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
				Setup(avatar, eyelidBlendshape, smr);
				eyelidBlendshape.ProcessedObjects.Add(avatar);
			}

			return null;
		}

		private void Setup(BasisAvatar avatar, AVA_Eyelids_Blendshape eyelidsBlendshape, SkinnedMeshRenderer smr)
		{
			avatar.FaceBlinkMesh = smr;
			var blink_visemes = new int[1];

			if (eyelidsBlendshape.Mappings[Array.FindIndex(AVA_Eyelids_Blendshape._EyelidShapes, e => e == "closed")] is var mapping && GetBlendshapeIndex(smr.sharedMesh, mapping) >= 0)
				blink_visemes[0] = GetBlendshapeIndex(smr.sharedMesh, mapping);

			avatar.BlinkViseme = blink_visemes;
		}

		private int GetBlendshapeIndex(Mesh mesh, string name)
		{
			for(int i = 0; i < mesh.blendShapeCount; i++)
			{
				var bName = mesh.GetBlendShapeName(i);
				if(bName == name) return i;
			}
			return -1;
		}
	}

	[InitializeOnLoad]
	public class Register_VRC_AVA_Eyelids_Blendshape_Processor
	{
		static Register_VRC_AVA_Eyelids_Blendshape_Processor()
		{
			STF_Processor_Registry.RegisterGlobalProcessor(DetectorBASISVR.STF_BASISVR_AVATAR_CONTEXT, new BASIS_AVA_Eyelids_Blendshape_Processor());
		}
	}
}

#endif
#endif
