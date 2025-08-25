#if UNITY_EDITOR
#if STF_AVA_UNIVRM0_FOUND

using UnityEditor;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.modules;
using UnityEngine;
using VRM;

namespace com.squirrelbite.stf_unity.ava.univrm0.processors
{
	public class UNIVRM0_AVA_SecondaryMotion_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(AVA_SecondaryMotion);

		public const uint _Order = 1000;
		public uint Order => _Order;

		public int Priority => 1;

		public (List<Object> ProcessedObjects, List<Object> ObjectsToRegister) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var stfSecondaryMotion = STFResource as AVA_SecondaryMotion;

			var springBone = (Context as AVAContext).GetMessage<GameObject>("VRM_secondary").AddComponent<VRMSpringBone>();
			springBone.RootBones.Add(stfSecondaryMotion.transform);
			springBone.m_comment = !string.IsNullOrWhiteSpace(stfSecondaryMotion.STF_Name) ? stfSecondaryMotion.STF_Name : stfSecondaryMotion.gameObject.name;

			springBone.enabled = stfSecondaryMotion.enabled;

			return (new() { springBone }, null);
		}
	}

	[InitializeOnLoad]
	public class Register_UNIVRM0_AVA_SecondaryMotion_Processor
	{
		static Register_UNIVRM0_AVA_SecondaryMotion_Processor()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorUNIVRM0.STF_UNIVRM0_AVATAR_CONTEXT, new UNIVRM0_AVA_SecondaryMotion_Processor());
		}
	}
}

#endif
#endif
