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

					// todo figure this out
					sourceTransformQuat *= Quaternion.SlerpUnclamped(Quaternion.identity, stfSource.SourceGo.transform.rotation, stfSource.Weight);
				}
			}

			// todo figure this out
			ret.rotationOffset = (Quaternion.Inverse(sourceTransformQuat) * ret.transform.rotation).eulerAngles;
			ret.rotationAtRest = ret.transform.rotation.eulerAngles;

			ret.locked = true;
			ret.constraintActive = true;

			//typeof(RotationConstraint).GetMethod("ActivateAndPreserveOffset", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(ret, null);
			//typeof(RotationConstraint).GetMethod("UserUpdateOffset", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(ret, null);

			return (new() { ret }, null);
		}
	}
}
