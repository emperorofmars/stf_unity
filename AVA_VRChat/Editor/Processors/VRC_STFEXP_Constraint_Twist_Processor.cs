#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using UnityEditor;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.modules;
using UnityEngine;
using VRC.SDK3.Dynamics.Constraint.Components;
using com.squirrelbite.stf_unity.modules.stfexp;
using System.Linq;

namespace com.squirrelbite.stf_unity.ava.vrchat.processors
{
	public class VRC_STFEXP_Constraint_Twist_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STFEXP_Constraint_Twist);

		public const uint _Order = 10;
		public uint Order => _Order;

		public int Priority => 100;

		public (List<Object> ProcessedObjects, List<Object> ObjectsToRegister) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var stfConstraint = STFResource as STFEXP_Constraint_Twist;

			Transform target = null;
			if (stfConstraint.Target == null || stfConstraint.Target.Count == 0)
			{
				target = stfConstraint.transform?.parent?.parent;
			}
			else if (stfConstraint.Target.Count == 1 && stfConstraint.STF_Owner is STF_Bone)
			{
				var ownerGo = (stfConstraint.STF_Owner as STF_Bone).STF_Owner;
				target = ownerGo.GetComponentsInChildren<STF_Bone>().FirstOrDefault(b => b.STF_Id == stfConstraint.Target[0] && b.STF_Owner == ownerGo)?.transform;
			}
			else if (stfConstraint.Target.Count == 1)
			{
				target = Context.Root.GetComponentsInChildren<STF_NodeResource>().FirstOrDefault(n => n.STF_Id == stfConstraint.Target[0])?.transform;
			}
			else if (stfConstraint.Target.Count == 3)
			{
				var ownerGo = Context.Root.GetComponentsInChildren<STF_NodeResource>().FirstOrDefault(n => n.STF_Id == stfConstraint.Target[0]);
				var armatureInstance = ownerGo.GetComponent<STF_Instance_Armature>();
				target = ownerGo.GetComponentsInChildren<STF_Bone>().FirstOrDefault(b => b.STF_Id == stfConstraint.Target[2] && b.STF_Owner == armatureInstance)?.transform;
			}

			if (target)
			{
				var ret = CreateConstraint(stfConstraint.gameObject, target, stfConstraint.Weight);
				ret.enabled = stfConstraint.enabled;
				return (new() { ret }, null);
			}
			else
				return (null, null);
		}

		public static VRCRotationConstraint CreateConstraint(GameObject Node, Transform Source, float Weight)
		{
			var converted = Node.AddComponent<VRCRotationConstraint>();

			converted.GlobalWeight = Weight;

			converted.AffectsRotationX = false;
			converted.AffectsRotationY = true;
			converted.AffectsRotationZ = false;

			converted.Sources.Add(new VRC.Dynamics.VRCConstraintSource(Source, 1, Vector3.zero, Vector3.zero));

			converted.RotationOffset = (Quaternion.Inverse(Source.rotation) * converted.transform.rotation).eulerAngles;

			converted.Locked = true;
			converted.IsActive = true;

			return converted;
		}
	}

	[InitializeOnLoad]
	public class Register_VRC_STFEXP_Constraint_Twist_Processor
	{
		static Register_VRC_STFEXP_Constraint_Twist_Processor()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorVRC.STF_VRC_AVATAR_CONTEXT, new VRC_STFEXP_Constraint_Twist_Processor());
		}
	}
}

#endif
#endif
