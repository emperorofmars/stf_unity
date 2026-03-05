#if UNITY_EDITOR
#if STF_FINALIK_FOUND

using UnityEngine;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.handlers;
using com.squirrelbite.stf_unity.handlers.stfexp;
using RootMotion.FinalIK;


namespace com.squirrelbite.stf_unity.processors.finalik
{
	public class FinalIK_STFEXP_Constraint_IK_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STFEXP_Constraint_IK);

		public uint Order => 10;

		public int Priority => 1;

		public (List<UnityEngine.Object>, List<UnityEngine.Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var stfConstraint = STFResource as STFEXP_Constraint_IK;

			if (stfConstraint.TargetPath.Count > 0)
				stfConstraint.TargetGo = STFUtil.ResolveBinding(Context, stfConstraint, stfConstraint.TargetPath);
			if (stfConstraint.PolePath.Count > 0)
				stfConstraint.PoleGo = STFUtil.ResolveBinding(Context, stfConstraint, stfConstraint.PolePath);

			if(stfConstraint.ChainLength == 2 && stfConstraint.TargetGo)
			{
				var ret = stfConstraint.gameObject.AddComponent<LimbIK>();
				ret.solver.target = stfConstraint.TargetGo.transform;

				if(stfConstraint.PoleGo)
				{
					ret.solver.bendModifier = IKSolverLimb.BendModifier.Goal;
					ret.solver.bendGoal = stfConstraint.PoleGo.transform;
				}

				ret.solver.bone1.transform = stfConstraint.transform.parent;
				ret.solver.bone2.transform = stfConstraint.transform;

				if(stfConstraint.gameObject.GetComponent<STF_Bone>() is STF_Bone stfBone)
				{
					var ikEndGo = new GameObject(stfBone.name + " - IK End");
					ikEndGo.transform.SetParent(stfConstraint.transform, false);
					ikEndGo.transform.Translate(0, stfBone.Length, 0);

					ret.solver.bone3.transform = ikEndGo.transform;
				}

				return (new() { ret }, null);
			}
			// TODO many more ways to do IK

			return (null, null);
		}
	}
}

#endif
#endif
