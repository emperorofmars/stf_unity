#if UNITY_EDITOR
#if STF_AVA_BASISVR_FOUND
#if STF_AVA_BASISVR_HAI_FOUND

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.modules;
using HVR.Basis.Comms;

namespace com.squirrelbite.stf_unity.ava.basisvr.processors
{
	public class BASIS_AVA_FaceTracking_Blendshape_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(AVA_FaceTracking_Blendshape);

		public const uint _Order = 1000;
		public uint Order => _Order;

		public int Priority => 1;

		public (List<UnityEngine.Object>, List<UnityEngine.Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var avaFT = STFResource as AVA_FaceTracking_Blendshape;

			var goFT = new GameObject("Face Tracking");
			goFT.transform.SetParent(Context.Root.transform);
			goFT.AddComponent<AutomaticFaceTracking>();

			return (new() { goFT }, null);
		}
	}

	[InitializeOnLoad]
	public class Register_BASIS_AVA_FaceTracking_Blendshape_Processor
	{
		static Register_BASIS_AVA_FaceTracking_Blendshape_Processor()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorBASISVR.STF_BASISVR_AVATAR_CONTEXT, new BASIS_AVA_FaceTracking_Blendshape_Processor());
		}
	}
}

#endif
#endif
#endif
