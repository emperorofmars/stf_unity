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
	public class VRC_AVA_Eyelids_Blendshape_Processor : ISTF_GlobalProcessor
	{
		public Type TargetType => typeof(AVA_Eyelids_Blendshape);

		public const uint _Order = 1000;
		public uint Order => _Order;

		public int Priority => 1;

		public List<UnityEngine.Object> Process(ProcessorContextBase Context)
		{
			var avatar = Context.Root.GetComponent<VRCAvatarDescriptor>();
			if (!avatar) Context.Report(new STFReport("No Avatar Component created!", ErrorSeverity.FATAL_ERROR, AVA_Eyelids_Blendshape._STF_Type));

			var stfMeshInstance = (Context as AVAContext).PrimaryMeshInstance;
			var smr = stfMeshInstance?.GetComponent<SkinnedMeshRenderer>();
			var eyelidBlendshape = stfMeshInstance?.Mesh.Components.Find(c => c.GetType() == typeof(AVA_Eyelids_Blendshape)) as AVA_Eyelids_Blendshape;

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
					smr = meshInstance?.GetComponent<SkinnedMeshRenderer>();
			}

			if (smr && eyelidBlendshape)
			{
				Setup(avatar, eyelidBlendshape, smr);
			}

			return null;
		}

		private void Setup(VRCAvatarDescriptor avatar, AVA_Eyelids_Blendshape eyelidsBlendshape, SkinnedMeshRenderer smr)
		{
			avatar.customEyeLookSettings.eyelidType = VRCAvatarDescriptor.EyelidType.Blendshapes;
			avatar.customEyeLookSettings.eyelidsSkinnedMesh = smr;
			avatar.customEyeLookSettings.eyelidsBlendshapes = new int[3];

			{
				if (eyelidsBlendshape.Mapppings[Array.FindIndex(AVA_Eyelids_Blendshape._EyelidShapes, e => e == "closed")] is var mapping && GetBlendshapeIndex(smr.sharedMesh, mapping) >= 0)
					avatar.customEyeLookSettings.eyelidsBlendshapes[0] = GetBlendshapeIndex(smr.sharedMesh, mapping);
			}
			{
				if (eyelidsBlendshape.Mapppings[Array.FindIndex(AVA_Eyelids_Blendshape._EyelidShapes, e => e == "up")] is var mapping && GetBlendshapeIndex(smr.sharedMesh, mapping) >= 0)
					avatar.customEyeLookSettings.eyelidsBlendshapes[1] = GetBlendshapeIndex(smr.sharedMesh, mapping);
			}
			{
				if (eyelidsBlendshape.Mapppings[Array.FindIndex(AVA_Eyelids_Blendshape._EyelidShapes, e => e == "down")] is var mapping && GetBlendshapeIndex(smr.sharedMesh, mapping) >= 0)
					avatar.customEyeLookSettings.eyelidsBlendshapes[2] = GetBlendshapeIndex(smr.sharedMesh, mapping);
			}
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
			STF_Processor_Registry.RegisterGlobalProcessor(DetectorVRC.STF_VRC_AVATAR_CONTEXT, new VRC_AVA_Eyelids_Blendshape_Processor());
		}
	}
}

#endif
#endif
