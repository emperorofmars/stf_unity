using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.modules.stfexp;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors.stfexp
{
	public class STFEXP_Collider_Capsule_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STFEXP_Collider_Capsule);

		public const uint _Order = 100;
		public uint Order => _Order;

		public int Priority => 1;

		public (List<Object> ProcessedObjects, List<Object> ObjectsToRegister) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var stfCollider = STFResource as STFEXP_Collider_Capsule;
			CapsuleCollider collider;

			if (stfCollider.offset_rotation != null && stfCollider.offset_rotation != Quaternion.identity)
			{
				var colliderObject = new GameObject();
				colliderObject.name = string.IsNullOrWhiteSpace(stfCollider.STF_Name) ? stfCollider.name + "_CapsuleCollider" : stfCollider.STF_Name;
				colliderObject.transform.SetParent(stfCollider.transform, false);
				colliderObject.transform.SetLocalPositionAndRotation(stfCollider.offset_position, stfCollider.offset_rotation);

				collider = colliderObject.AddComponent<CapsuleCollider>();
				collider.center = stfCollider.offset_position;
			}
			else
			{
				collider = stfCollider.gameObject.AddComponent<CapsuleCollider>();
			}

			collider.radius = stfCollider.radius;
			collider.height = stfCollider.height;

			collider.enabled = stfCollider.enabled;

			return (new() { collider }, null);
		}
	}
}
