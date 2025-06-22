#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using UnityEngine;
using VRC.SDK3.Avatars.Components;
using UnityEditor;
using System;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.modules;
using System.Linq;

namespace com.squirrelbite.stf_unity.ava.vrchat.processors
{
	public class VRC_AVA_Visemes_Blendshape_Processor : ISTF_GlobalProcessor
	{
		public Type TargetType => typeof(AVA_Visemes_Blendshape);

		public const uint _Order = 1000;
		public uint Order => _Order;

		public int Priority => 1;

		public List<UnityEngine.Object> Process(ProcessorContextBase Context)
		{
			var avatar = Context.Root.GetComponent<VRCAvatarDescriptor>();
			if (!avatar) Context.Report(new STFReport("No Avatar Component created!", ErrorSeverity.FATAL_ERROR, AVA_Visemes_Blendshape._STF_Type));

			var stfMeshInstance = (Context as AVAContext).PrimaryMeshInstance;
			var smr = stfMeshInstance?.GetComponent<SkinnedMeshRenderer>();
			var visemesBlendshape = stfMeshInstance?.Mesh.Components.Find(c => c.GetType() == typeof(AVA_Visemes_Blendshape)) as AVA_Visemes_Blendshape;

			if (!visemesBlendshape)
			{
				var visemesBlendshapeResources = Context.GetResourceByType(typeof(AVA_Visemes_Blendshape));
				if (visemesBlendshapeResources.Count == 1)
				{
					visemesBlendshape = visemesBlendshapeResources[0] as AVA_Visemes_Blendshape;
				}
			}

			if (!smr && visemesBlendshape)
			{
				var mesh = visemesBlendshape.ParentObject as STF_Mesh;
				var meshInstance = Context.Root.GetComponentsInChildren<STF_Instance_Mesh>().FirstOrDefault(e => e.Mesh == mesh);
				if (meshInstance)
					smr = meshInstance?.GetComponent<SkinnedMeshRenderer>();
			}

			if (smr && visemesBlendshape)
			{
				Setup(avatar, visemesBlendshape, smr);
				visemesBlendshape.ProcessedObjects.Add(avatar);
			}

			return null;
		}

		private void Setup(VRCAvatarDescriptor avatar, AVA_Visemes_Blendshape visemesBlendshape, SkinnedMeshRenderer smr)
		{
			avatar.VisemeSkinnedMesh = smr;
			avatar.lipSync = VRC.SDKBase.VRC_AvatarDescriptor.LipSyncStyle.VisemeBlendShape;
			avatar.VisemeBlendShapes = new string[15];
			avatar.VisemeBlendShapes[0] = visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "sil")];
			avatar.VisemeBlendShapes[1] = visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "pp")];
			avatar.VisemeBlendShapes[2] = visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "ff")];
			avatar.VisemeBlendShapes[3] = visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "th")];
			avatar.VisemeBlendShapes[4] = visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "dd")];
			avatar.VisemeBlendShapes[5] = visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "kk")];
			avatar.VisemeBlendShapes[6] = visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "ch")];
			avatar.VisemeBlendShapes[7] = visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "ss")];
			avatar.VisemeBlendShapes[8] = visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "nn")];
			avatar.VisemeBlendShapes[9] = visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "rr")];
			avatar.VisemeBlendShapes[10] = visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "aa")];
			avatar.VisemeBlendShapes[11] = visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "e")];
			avatar.VisemeBlendShapes[12] = visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "ih")];
			avatar.VisemeBlendShapes[13] = visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "oh")];
			avatar.VisemeBlendShapes[14] = visemesBlendshape.Mapppings[Array.FindIndex(AVA_Visemes_Blendshape._Visemes15, e => e == "ou")];
		}
	}

	[InitializeOnLoad]
	public class Register_AVA_Visemes_Blendshape_VRC
	{
		static Register_AVA_Visemes_Blendshape_VRC()
		{
			STF_Processor_Registry.RegisterGlobalProcessor(DetectorVRC.STF_VRC_AVATAR_CONTEXT, new VRC_AVA_Visemes_Blendshape_Processor());
		}
	}
}

#endif
#endif
