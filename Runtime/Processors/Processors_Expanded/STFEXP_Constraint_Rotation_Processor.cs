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
			if(stfConstraint.Sources.Count == 1)
			{
				var ret = stfConstraint.gameObject.AddComponent<RotationConstraint>();

				ret.weight = stfConstraint.Weight;
				ret.rotationAxis = stfConstraint.Axes;

				var stfSource = stfConstraint.Sources[0];
				if (stfSource.SourcePath.Count > 0)
					stfSource.SourceGo = STFUtil.ResolveBinding(Context, stfConstraint, stfSource.SourcePath);
				if (stfSource.SourceGo)
				{
					var source = new ConstraintSource { weight = stfSource.Weight, sourceTransform = stfSource.SourceGo.transform };
					ret.AddSource(source);
					Quaternion rotationOffset = Quaternion.Inverse(source.sourceTransform.rotation) * ret.transform.rotation;
					ret.rotationOffset = rotationOffset.eulerAngles;
				}

				ret.locked = true;
				ret.constraintActive = true;

				return (new() { ret }, null);
			}
			else
			{
				var ret = stfConstraint.gameObject.AddComponent<ParentConstraint>();

				ret.weight = stfConstraint.Weight;
				ret.rotationAxis = stfConstraint.Axes;

				ret.translationAxis = Axis.None;

				int sourceIndex = 0;
				foreach(var stfSource in stfConstraint.Sources)
				{
					if (stfSource.SourcePath.Count > 0)
						stfSource.SourceGo = STFUtil.ResolveBinding(Context, stfConstraint, stfSource.SourcePath);
					if (stfSource.SourceGo)
					{
						ret.AddSource(new ConstraintSource { weight = stfSource.Weight, sourceTransform = stfSource.SourceGo.transform });
						ret.SetRotationOffset(sourceIndex, (Quaternion.Inverse(stfSource.SourceGo.transform.rotation) * ret.transform.rotation).eulerAngles);
						sourceIndex++;
					}
				}

				ret.locked = true;
				ret.constraintActive = true;

				return (new() { ret }, null);
			}
		}
	}
}
