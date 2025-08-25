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
	public class BASIS_AVA_Visemes_Blendshape_Processor : ISTF_GlobalProcessor
	{
		public Type TargetType => typeof(AVA_Visemes_Blendshape);

		public const uint _Order = 1000;
		public uint Order => _Order;

		public int Priority => 1;

		public List<UnityEngine.Object> Process(ProcessorContextBase Context)
		{
			var avatar = Context.Root.GetComponent<BasisAvatar>();
			if (!avatar) Context.Report(new STFReport("No Avatar Component created!", ErrorSeverity.FATAL_ERROR, AVA_Visemes_Blendshape._STF_Type));

			var stfMeshInstance = (Context as AVAContext).PrimaryMeshInstance;
			var smr = stfMeshInstance ? stfMeshInstance.GetComponent<SkinnedMeshRenderer>() : null;
			var visemesBlendshape = stfMeshInstance ? stfMeshInstance.Mesh.Components.Find(c => c.GetType() == typeof(AVA_Visemes_Blendshape)) as AVA_Visemes_Blendshape : null;

			if (!visemesBlendshape)
			{
				var visemesBlendshapeResources = Context.GetResourceByType(typeof(AVA_Visemes_Blendshape));
				if (visemesBlendshapeResources != null && visemesBlendshapeResources.Count == 1)
				{
					visemesBlendshape = visemesBlendshapeResources[0] as AVA_Visemes_Blendshape;
				}
			}

			if (!smr && visemesBlendshape)
			{
				var mesh = visemesBlendshape.ParentObject as STF_Mesh;
				var meshInstance = Context.Root.GetComponentsInChildren<STF_Instance_Mesh>().FirstOrDefault(e => e.Mesh == mesh);
				if (meshInstance)
					smr = meshInstance.GetComponent<SkinnedMeshRenderer>();
			}

			if (smr && visemesBlendshape)
			{
				Setup(avatar, visemesBlendshape, smr);
				visemesBlendshape.ProcessedObjects.Add(avatar);
			}

			return null;
		}

		private void Setup(BasisAvatar avatar, AVA_Visemes_Blendshape visemesBlendshape, SkinnedMeshRenderer smr)
		{
			avatar.FaceVisemeMesh = smr;

			var viseme_indices = new int[15];

			viseme_indices[0] = GetBlendshapeIndex(smr.sharedMesh, visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "sil")]);
			viseme_indices[1] = GetBlendshapeIndex(smr.sharedMesh, visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "pp")]);
			viseme_indices[2] = GetBlendshapeIndex(smr.sharedMesh, visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "ff")]);
			viseme_indices[3] = GetBlendshapeIndex(smr.sharedMesh, visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "th")]);
			viseme_indices[4] = GetBlendshapeIndex(smr.sharedMesh, visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "dd")]);
			viseme_indices[5] = GetBlendshapeIndex(smr.sharedMesh, visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "kk")]);
			viseme_indices[6] = GetBlendshapeIndex(smr.sharedMesh, visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "ch")]);
			viseme_indices[7] = GetBlendshapeIndex(smr.sharedMesh, visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "ss")]);
			viseme_indices[8] = GetBlendshapeIndex(smr.sharedMesh, visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "nn")]);
			viseme_indices[9] = GetBlendshapeIndex(smr.sharedMesh, visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "rr")]);
			viseme_indices[10] = GetBlendshapeIndex(smr.sharedMesh, visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "aa")]);
			viseme_indices[11] = GetBlendshapeIndex(smr.sharedMesh, visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "e")]);
			viseme_indices[12] = GetBlendshapeIndex(smr.sharedMesh, visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "ih")]);
			viseme_indices[13] = GetBlendshapeIndex(smr.sharedMesh, visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "oh")]);
			viseme_indices[14] = GetBlendshapeIndex(smr.sharedMesh, visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "ou")]);

			avatar.FaceVisemeMovement = viseme_indices;
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
	public class Register_AVA_Visemes_Blendshape_VRC
	{
		static Register_AVA_Visemes_Blendshape_VRC()
		{
			STF_Processor_Registry.RegisterGlobalProcessor(DetectorBASISVR.STF_BASISVR_AVATAR_CONTEXT, new BASIS_AVA_Visemes_Blendshape_Processor());
		}
	}
}

#endif
#endif
