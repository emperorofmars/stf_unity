using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Animations;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.modules.stfexp;

namespace com.squirrelbite.stf_unity.processors.stfexp
{
	public class STFEXP_Constraint_Twist_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STFEXP_Constraint_Twist);

		public uint Order => 10;

		public int Priority => 1;

		public (List<UnityEngine.Object>, List<UnityEngine.Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var stfConstraint = STFResource as STFEXP_Constraint_Twist;

			if (stfConstraint.SourcePath.Count > 0)
				stfConstraint.SourceGo = STFUtil.ResolveBinding(Context, stfConstraint, stfConstraint.SourcePath);
			else if(stfConstraint.transform.parent && stfConstraint.transform.parent.parent)
				stfConstraint.SourceGo = stfConstraint.transform.parent.parent.gameObject;

			if (stfConstraint.SourceGo)
			{
				var ret = CreateConstraint(stfConstraint.gameObject, stfConstraint.SourceGo, stfConstraint.Weight);
				ret.enabled = stfConstraint.enabled;
				return (new() { ret }, null);
			}
			else
				return (null, null);
		}

		public static RotationConstraint CreateConstraint(GameObject Node, GameObject Source, float Weight)
		{
			var converted = Node.AddComponent<RotationConstraint>();

			converted.weight = Weight;
			converted.rotationAxis = Axis.Y;

			var source = new ConstraintSource
			{
				weight = 1,
				sourceTransform = Source.transform,
			};
			converted.AddSource(source);

			Quaternion rotationOffset = Quaternion.Inverse(source.sourceTransform.rotation) * converted.transform.rotation;
			converted.rotationOffset = rotationOffset.eulerAngles;

			converted.locked = true;
			converted.constraintActive = true;

			return converted;
		}
	}
}
