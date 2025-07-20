#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using UnityEditor;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.modules;
using VRC.SDK3.Dynamics.PhysBone.Components;
using UnityEngine;

namespace com.squirrelbite.stf_unity.ava.vrchat.processors
{
	public class VRC_AVA_SecondaryMotion_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(AVA_SecondaryMotion);

		public const uint _Order = 1000;
		public uint Order => _Order;

		public int Priority => 1;

		public (List<Object> ProcessedObjects, List<Object> ObjectsToRegister) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var stfSecondaryMotion = STFResource as AVA_SecondaryMotion;
			var physbone = stfSecondaryMotion.gameObject.AddComponent<VRCPhysBone>();
			physbone.endpointPosition.y = 0.1f; // TODO get bone length from stf.bone if applicable, deal with multiple endbones, ...

			physbone.enabled = stfSecondaryMotion.enabled;

			return (new() { physbone }, null);
		}
	}

	[InitializeOnLoad]
	public class Register_VRC_AVA_SecondaryMotion_Processor
	{
		static Register_VRC_AVA_SecondaryMotion_Processor()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorVRC.STF_VRC_AVATAR_CONTEXT, new VRC_AVA_SecondaryMotion_Processor());
		}
	}
}

#endif
#endif
