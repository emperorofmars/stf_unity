#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using UnityEditor;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.modules;
using VRC.SDK3.Dynamics.PhysBone.Components;
using UnityEngine;
using com.squirrelbite.stf_unity.ava.vrchat.modules;
using System.Linq;
using VRC.Dynamics;

namespace com.squirrelbite.stf_unity.ava.vrchat.processors
{
	public class VRC_Physbone_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(VRC_Physbone);

		public const uint _Order = 1000;
		public uint Order => _Order;

		public int Priority => 1;

		public (List<Object> ProcessedObjects, List<Object> ObjectsToRegister) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var stfPhysbone = STFResource as VRC_Physbone;
			var physbone = stfPhysbone.gameObject.AddComponent<VRCPhysBone>();
			
			JsonUtility.FromJsonOverwrite(stfPhysbone.Json.ToString(), physbone);

			if(stfPhysbone.Ignores.Count > 0) foreach(var ignorePath in stfPhysbone.Ignores)
			{
				if(STFUtil.ResolvePath(stfPhysbone.STF_Owner, ignorePath.Target) is var ignore && ignore != null)
				{
					physbone.ignoreTransforms.Add(ignore.transform);
				}
			}

			if(stfPhysbone.Colliders.Count > 0) foreach(var colliderPath in stfPhysbone.Colliders)
			{
				if(STFUtil.ResolvePath(stfPhysbone.STF_Owner, colliderPath.Target) is var collider && collider != null)
				{
					foreach (VRCPhysBoneColliderBase vrcCollider in collider.ProcessedObjects.Where(o => o is VRCPhysBoneColliderBase).Cast<VRCPhysBoneColliderBase>())
					{
						physbone.colliders.Add(vrcCollider);
					}
				}
			}

			physbone.enabled = stfPhysbone.enabled;

			return (new() { physbone }, null);
		}
	}

	[InitializeOnLoad]
	public class Register_VRC_Physbone_Processor
	{
		static Register_VRC_Physbone_Processor()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorVRC.STF_VRC_AVATAR_CONTEXT, new VRC_Physbone_Processor());
		}
	}
}

#endif
#endif
