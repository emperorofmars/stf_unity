#if UNITY_EDITOR
#if STF_AVA_UNIVRM0_FOUND

using UnityEditor;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.modules;
using UnityEngine;
using VRM;
using System.Linq;

namespace com.squirrelbite.stf_unity.ava.univrm0.processors
{
	public class UNIVRM0_AVA_Springbone_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(VRM_Springbone);

		public const uint _Order = 1000;
		public uint Order => _Order;

		public int Priority => 10;

		public (List<Object> ProcessedObjects, List<Object> ObjectsToRegister) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var stfSpringbone = STFResource as VRM_Springbone;

			var springBone = (Context as AVAContext).GetMessage<GameObject>("VRM_secondary").AddComponent<VRMSpringBone>();
			springBone.RootBones.Add(stfSpringbone.transform);
			springBone.m_comment = !string.IsNullOrWhiteSpace(stfSpringbone.STF_Name) ? stfSpringbone.STF_Name : stfSpringbone.gameObject.name;

			springBone.m_stiffnessForce = stfSpringbone.stiffness;
			springBone.m_gravityPower = stfSpringbone.gravityPower;
			springBone.m_gravityDir = stfSpringbone.gravityDir;
			springBone.m_dragForce = stfSpringbone.dragForce;
			if(STFUtil.ResolvePath(stfSpringbone.STF_Owner, stfSpringbone.center.Target) is var centerTarget && centerTarget != null)
				springBone.m_center = centerTarget.transform;
			springBone.m_hitRadius = stfSpringbone.hitRadius;

			var colliderGroups = new List<VRMSpringBoneColliderGroup>();
			if(stfSpringbone.Colliders.Count > 0) foreach(var colliderPath in stfSpringbone.Colliders)
			{
				if(STFUtil.ResolvePath(stfSpringbone.STF_Owner, colliderPath.Target) is var collider && collider != null)
				{
					foreach (VRMSpringBoneColliderGroup vrmCollider in collider.ProcessedObjects.Where(o => o is VRMSpringBoneColliderGroup).Cast<VRMSpringBoneColliderGroup>())
					{
						colliderGroups.Add(vrmCollider);
					}
				}
			}
			if(colliderGroups.Count > 0)
				springBone.ColliderGroups = colliderGroups.ToArray();

			springBone.enabled = stfSpringbone.enabled;

			return (new() { springBone }, null);
		}
	}

	[InitializeOnLoad]
	public class Register_UNIVRM0_AVA_Springbone_Processor
	{
		static Register_UNIVRM0_AVA_Springbone_Processor()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorUNIVRM0.STF_UNIVRM0_AVATAR_CONTEXT, new UNIVRM0_AVA_Springbone_Processor());
		}
	}
}

#endif
#endif
