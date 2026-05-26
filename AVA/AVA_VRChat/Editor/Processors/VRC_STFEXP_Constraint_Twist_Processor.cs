#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using UnityEditor;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.resources;
using UnityEngine;
using VRC.SDK3.Dynamics.Constraint.Components;
using com.squirrelbite.stf_unity.resources.stfexp;

namespace com.squirrelbite.stf_unity.ava.vrchat.processors
{
	public class VRC_STFEXP_Constraint_Twist_Converter : ComponentAnimationConverterBase<VRCRotationConstraint>
	{
		public override ImportPropertyPathPart ConvertComponentPropertyPath(ISTF_Resource STFResource, List<string> STFPath)
		{
			var stfConstraint = STFResource as STFEXP_Constraint_Twist;
			if (STFPath.Count == 1 && STFPath[0] == "weight")
				return new ImportPropertyPathPart(typeof(VRCRotationConstraint), new() { "Sources.source0.Weight" });
			return null;
		}
	}
	public class VRC_STFEXP_Constraint_Twist_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STFEXP_Constraint_Twist);
		public const uint _Order = 10;
		public uint Order => _Order;

		public int Priority => 100;

		public (List<Object> ProcessedObjects, List<Object> ObjectsToRegister) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var stfConstraint = STFResource as STFEXP_Constraint_Twist;

			if (stfConstraint.SourcePath.Count > 0)
				stfConstraint.SourceGo = STFUtil.ResolveBinding(Context, stfConstraint, stfConstraint.SourcePath);
			else if(stfConstraint.transform.parent && stfConstraint.transform.parent.parent)
				stfConstraint.SourceGo = stfConstraint.transform.parent.parent.gameObject;

			if (stfConstraint.SourceGo)
			{
				var ret = CreateConstraint(stfConstraint.gameObject, stfConstraint.SourceGo.transform, stfConstraint.Weight);
				ret.enabled = stfConstraint.enabled;

				stfConstraint.PropertyConverter = new VRC_STFEXP_Constraint_Twist_Converter();
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
