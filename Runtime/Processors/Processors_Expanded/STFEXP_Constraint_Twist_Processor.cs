using UnityEngine;
using System;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.stfexp;
using UnityEngine.Animations;
using com.squirrelbite.stf_unity.modules;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.processors.stfexp
{
	public class STFEXP_Constraint_Twist_Processor : ISTF_Processor
	{
		public Type TargetType => typeof(STFEXP_Constraint_Twist);

		public uint Order => 10;

		public int Priority => 1;

		public List<UnityEngine.Object> Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var stfConstraint = STFResource as STFEXP_Constraint_Twist;

			Transform target = null;
			if (stfConstraint.Target == null || stfConstraint.Target.Count == 0)
			{
				target = stfConstraint.transform?.parent?.parent;
			}
			else if (stfConstraint.Target.Count == 1 && stfConstraint.STF_Owner is STF_Bone)
			{
				var ownerGo = (stfConstraint.STF_Owner as STF_Bone).STF_Owner;
				target = ownerGo.GetComponentsInChildren<STF_Bone>().FirstOrDefault(b => b.STF_Id == stfConstraint.Target[0] && b.STF_Owner == ownerGo)?.transform;
			}
			else if (stfConstraint.Target.Count == 1)
			{
				target = Context.Root.GetComponentsInChildren<STF_NodeResource>().FirstOrDefault(n => n.STF_Id == stfConstraint.Target[0])?.transform;
			}
			else if (stfConstraint.Target.Count == 3)
			{
				var ownerGo = Context.Root.GetComponentsInChildren<STF_NodeResource>().FirstOrDefault(n => n.STF_Id == stfConstraint.Target[0]);
				var armatureInstance = ownerGo.GetComponent<STF_Instance_Armature>();
				target = ownerGo.GetComponentsInChildren<STF_Bone>().FirstOrDefault(b => b.STF_Id == stfConstraint.Target[2] && b.STF_Owner == armatureInstance)?.transform;
			}

			if (target)
				CreateConstraint(stfConstraint.transform, target, stfConstraint.Weight);

			return null;
		}

		public static RotationConstraint CreateConstraint(Transform Node, Transform Source, float Weight)
		{
			var converted = Node.gameObject.AddComponent<RotationConstraint>();

			converted.weight = Weight;
			converted.rotationAxis = Axis.Y;

			var source = new ConstraintSource
			{
				weight = 1,
				sourceTransform = Source,
			};
			converted.AddSource(source);

			Quaternion rotationOffset = Quaternion.Inverse(source.sourceTransform.rotation) * converted.transform.rotation;
			converted.rotationOffset = rotationOffset.eulerAngles;

			converted.locked = true;
			converted.constraintActive = true;

			return converted;
		}
	}

#if UNITY_EDITOR
	[InitializeOnLoad]
	public class Register_STFEXP_Constraint_Twist_Processor
	{
		static Register_STFEXP_Constraint_Twist_Processor()
		{
			STF_Processor_Registry.RegisterProcessor("default", new STFEXP_Constraint_Twist_Processor());
		}
	}
#endif
}
