#if UNITY_EDITOR
#if STF_AVA_UNIVRM0_FOUND

using UnityEditor;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.modules;
using UnityEngine;
using VRM;
using com.squirrelbite.stf_unity.modules.stfexp;

namespace com.squirrelbite.stf_unity.ava.univrm0.processors
{
	public class UNIVRM0_STFEXP_Collider_Capsule_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STFEXP_Collider_Capsule);

		public const uint _Order = 100;
		public uint Order => _Order;

		public int Priority => 1;

		public (List<Object> ProcessedObjects, List<Object> ObjectsToRegister) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var stfCollider = STFResource as STFEXP_Collider_Capsule;

			var colliderGroup = stfCollider.gameObject.AddComponent<VRMSpringBoneColliderGroup>();
			var colliders = new List<VRMSpringBoneColliderGroup.SphereCollider>();

			System.Action<Vector3> genCollider = (offset) => {
				colliders.Add( new VRMSpringBoneColliderGroup.SphereCollider {
					Radius = stfCollider.radius,
					Offset = offset
				});
			};

			if(stfCollider.height > 0.001 && stfCollider.radius > 0.001 && stfCollider.height / stfCollider.radius > 0.001)
			{
				var fillCount = (float)System.Math.Round(System.Math.Min(System.Math.Max(stfCollider.height / stfCollider.radius, 2), 100), 0);
				var rotation_offset = stfCollider.offset_rotation != null ? stfCollider.offset_rotation : Quaternion.identity;
				var offset = rotation_offset * new Vector3(0, stfCollider.height, 0);
				var pos_start = stfCollider.offset_position - offset * 0.5f;

				for(int i = 0; i < fillCount; i++)
				{
					genCollider(pos_start + offset * (i / fillCount));
				}
			}
			else
			{
				genCollider(stfCollider.offset_position);
			}

			colliderGroup.Colliders = colliders.ToArray();
			colliderGroup.enabled = stfCollider.enabled;
			return (new() { colliderGroup }, null);
		}
	}

	[InitializeOnLoad]
	public class Register_UNIVRM0_STFEXP_Collider_Capsule_Processor
	{
		static Register_UNIVRM0_STFEXP_Collider_Capsule_Processor()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorUNIVRM0.STF_UNIVRM0_AVATAR_CONTEXT, new UNIVRM0_STFEXP_Collider_Capsule_Processor());
		}
	}
}

#endif
#endif
