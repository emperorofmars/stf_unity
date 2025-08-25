#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using UnityEditor;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.modules;
using UnityEngine;
using VRC.SDK3.Dynamics.Constraint.Components;
using com.squirrelbite.stf_unity.modules.stfexp;

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

			if (stfConstraint.Target.Count > 0)
				stfConstraint.TargetGo = STFUtil.ResolveBinding(Context, stfConstraint, stfConstraint.Target);
			else if(stfConstraint.transform.parent && stfConstraint.transform.parent.parent)
				stfConstraint.TargetGo = stfConstraint.transform.parent.parent.gameObject;

			if (stfConstraint.TargetGo)
			{
				var ret = CreateConstraint(stfConstraint.gameObject, stfConstraint.TargetGo.transform, stfConstraint.Weight);
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
