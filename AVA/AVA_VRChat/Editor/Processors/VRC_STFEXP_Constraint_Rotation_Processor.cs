#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Animations;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.modules.stfexp;
using UnityEditor;
using com.squirrelbite.stf_unity.processors;
using VRC.SDK3.Dynamics.Constraint.Components;

namespace com.squirrelbite.stf_unity.ava.vrchat.processors
{
	public class VRC_STFEXP_Constraint_Rotation_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STFEXP_Constraint_Rotation);
		public uint Order => 10;
		public int Priority => 100;

		public (List<UnityEngine.Object>, List<UnityEngine.Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var stfConstraint = STFResource as STFEXP_Constraint_Rotation;
			var ret = stfConstraint.gameObject.AddComponent<VRCParentConstraint>();

			ret.GlobalWeight = stfConstraint.Weight;

			ret.AffectsRotationX = (stfConstraint.Axes & Axis.X) > 0;
			ret.AffectsRotationY = (stfConstraint.Axes & Axis.Y) > 0;
			ret.AffectsRotationZ = (stfConstraint.Axes & Axis.Z) > 0;

			ret.AffectsPositionX = false;
			ret.AffectsPositionY = false;
			ret.AffectsPositionZ = false;

			foreach(var stfSource in stfConstraint.Sources)
			{
				if (stfSource.SourcePath.Count > 0)
					stfSource.SourceGo = STFUtil.ResolveBinding(Context, stfConstraint, stfSource.SourcePath);
				if (stfSource.SourceGo)
				{
					ret.Sources.Add(new VRC.Dynamics.VRCConstraintSource(stfSource.SourceGo.transform, stfSource.Weight, Vector3.zero, (Quaternion.Inverse(stfSource.SourceGo.transform.rotation) * ret.transform.rotation).eulerAngles));
				}
			}
			ret.Locked = true;
			ret.IsActive = true;

			return (new() { ret }, null);
		}
	}

	[InitializeOnLoad]
	public class Register_VRC_STFEXP_Constraint_Rotation_Processor
	{
		static Register_VRC_STFEXP_Constraint_Rotation_Processor()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorVRC.STF_VRC_AVATAR_CONTEXT, new VRC_STFEXP_Constraint_Rotation_Processor());
		}
	}
}

#endif
#endif
