using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Animations;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.modules.stfexp;

namespace com.squirrelbite.stf_unity.processors.stfexp
{
	public class STFEXP_Constraint_Rotation_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STFEXP_Constraint_Rotation);

		public uint Order => 10;

		public int Priority => 1;

		public (List<UnityEngine.Object>, List<UnityEngine.Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var stfConstraint = STFResource as STFEXP_Constraint_Rotation;
			var ret = stfConstraint.gameObject.AddComponent<RotationConstraint>();

			var sourceTransformQuat = Quaternion.identity;
			foreach(var stfSource in stfConstraint.Sources)
			{
				if (stfSource.SourcePath.Count > 0)
					stfSource.SourceGo = STFUtil.ResolveBinding(Context, stfConstraint, stfSource.SourcePath);
				if (stfSource.SourceGo)
				{
					ret.AddSource(new ConstraintSource { weight = stfSource.Weight, sourceTransform = stfSource.SourceGo.transform });
					sourceTransformQuat *= Quaternion.SlerpUnclamped(Quaternion.identity, stfSource.SourceGo.transform.rotation, stfSource.Weight);
				}
			}

			Quaternion rotationOffset = Quaternion.Inverse(sourceTransformQuat) * ret.transform.rotation;
			ret.rotationOffset = rotationOffset.eulerAngles;

			ret.constraintActive = true;
			ret.locked = true;

			return (new() { ret }, null);
		}
	}
}
