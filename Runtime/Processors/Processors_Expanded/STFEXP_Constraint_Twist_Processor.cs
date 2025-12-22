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
				var ret = stfConstraint.gameObject.AddComponent<RotationConstraint>();
				if(!ret)
				{
					return (null, null); //TODO Report Warning
				}

				ret.weight = stfConstraint.Weight;
				ret.rotationAxis = Axis.Y;

				var source = new ConstraintSource
				{
					weight = 1,
					sourceTransform = stfConstraint.SourceGo.transform,
				};
				ret.AddSource(source);

				Quaternion rotationOffset = Quaternion.Inverse(source.sourceTransform.rotation) * ret.transform.rotation;
				ret.rotationOffset = rotationOffset.eulerAngles;

				ret.locked = true;
				ret.constraintActive = true;

				ret.enabled = stfConstraint.enabled;
				return (new() { ret }, null);
			}
			else
				return (null, null);
		}
	}
}
