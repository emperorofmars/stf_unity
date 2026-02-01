using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Animations;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.modules.stfexp;

namespace com.squirrelbite.stf_unity.processors.stfexp
{
	public class STFEXP_Constraint_Parent_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STFEXP_Constraint_Parent);

		public uint Order => 10;

		public int Priority => 1;

		public (List<UnityEngine.Object>, List<UnityEngine.Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var stfConstraint = STFResource as STFEXP_Constraint_Parent;
			var ret = stfConstraint.gameObject.AddComponent<ParentConstraint>();

			ret.weight = stfConstraint.Weight;
			ret.translationAxis = stfConstraint.TranslationAxes;
			ret.rotationAxis = stfConstraint.RotationAxes;

			int sourceIndex = 0;
			foreach(var stfSource in stfConstraint.Sources)
			{
				if (stfSource.SourcePath.Count > 0)
					stfSource.SourceGo = STFUtil.ResolveBinding(Context, stfConstraint, stfSource.SourcePath);
				if (stfSource.SourceGo)
				{
					ret.AddSource(new ConstraintSource { weight = stfSource.Weight, sourceTransform = stfSource.SourceGo.transform });
					ret.SetTranslationOffset(sourceIndex, ret.transform.position - stfSource.SourceGo.transform.position);
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
