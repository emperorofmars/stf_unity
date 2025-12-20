using UnityEngine;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules.stfexp;
using com.squirrelbite.stf_unity.modules;
using System.Linq;

namespace com.squirrelbite.stf_unity.processors.stfexp
{
	public class Humanoid_Armature_Mappings
	{
		public static readonly Dictionary<string, string> STF_To_Unity_Mappings = new() {
			{"hip", HumanBodyBones.Hips.ToString()},
			{"spine", HumanBodyBones.Spine.ToString()},
			{"chest", HumanBodyBones.Chest.ToString()},
			{"upper_chest", HumanBodyBones.UpperChest.ToString()},
			{"neck", HumanBodyBones.Neck.ToString()},
			{"head", HumanBodyBones.Head.ToString()},
			{"jaw", HumanBodyBones.Jaw.ToString()},
			{"eye.l", HumanBodyBones.LeftEye.ToString()},
			{"eye.r", HumanBodyBones.RightEye.ToString()},
			{"shoulder.l", HumanBodyBones.LeftShoulder.ToString()},
			{"upper_arm.l", HumanBodyBones.LeftUpperArm.ToString()},
			{"lower_arm.l", HumanBodyBones.LeftLowerArm.ToString()},
			{"wrist.l", HumanBodyBones.LeftHand.ToString()},
			{"thumb_1.l", "Left Thumb Proximal"},
			{"thumb_2.l", "Left Thumb Intermediate"},
			{"thumb_3.l", "Left Thumb Distal"},
			{"index_1.l", "Left Index Proximal"},
			{"index_2.l", "Left Index Intermediate"},
			{"index_3.l", "Left Index Distal"},
			{"middle_1.l", "Left Middle Proximal"},
			{"middle_2.l", "Left Middle Intermediate"},
			{"middle_3.l", "Left Middle Distal"},
			{"ring_1.l", "Left Ring Proximal"},
			{"ring_2.l", "Left Ring Intermediate"},
			{"ring_3.l", "Left Ring Distal"},
			{"little_1.l", "Left Little Proximal"},
			{"little_2.l", "Left Little Intermediate"},
			{"little_3.l", "Left Little Distal"},
			{"shoulder.r", HumanBodyBones.RightShoulder.ToString()},
			{"upper_arm.r", HumanBodyBones.RightUpperArm.ToString()},
			{"lower_arm.r", HumanBodyBones.RightLowerArm.ToString()},
			{"wrist.r", HumanBodyBones.RightHand.ToString()},
			{"thumb_1.r", "Right Thumb Proximal"},
			{"thumb_2.r", "Right Thumb Intermediate"},
			{"thumb_3.r", "Right Thumb Distal"},
			{"index_1.r", "Right Index Proximal"},
			{"index_2.r", "Right Index Intermediate"},
			{"index_3.r", "Right Index Distal"},
			{"middle_1.r", "Right Middle Proximal"},
			{"middle_2.r", "Right Middle Intermediate"},
			{"middle_3.r", "Right Middle Distal"},
			{"ring_1.r", "Right Ring Proximal"},
			{"ring_2.r", "Right Ring Intermediate"},
			{"ring_3.r", "Right Ring Distal"},
			{"little_1.r", "Right Little Proximal"},
			{"little_2.r", "Right Little Intermediate"},
			{"little_3.r", "Right Little Distal"},
			{"upper_leg.l", HumanBodyBones.LeftUpperLeg.ToString()},
			{"lower_leg.l", HumanBodyBones.LeftLowerLeg.ToString()},
			{"foot.l", HumanBodyBones.LeftFoot.ToString()},
			{"toes.l", HumanBodyBones.LeftToes.ToString()},
			{"upper_leg.r", HumanBodyBones.RightUpperLeg.ToString()},
			{"lower_leg.r", HumanBodyBones.RightLowerLeg.ToString()},
			{"foot.r", HumanBodyBones.RightFoot.ToString()},
			{"toes.r", HumanBodyBones.RightToes.ToString()},
		};

		public static string TranslateHumanoidSTFtoUnity(string STFMapping, string LocomotionType, bool NoJaw)
		{
			if(NoJaw && STFMapping == "jaw") return null;
			if(LocomotionType.StartsWith("digi"))
			{
				switch(STFMapping)
				{
					case "toes.l":
						return HumanBodyBones.LeftFoot.ToString();
					case "toes.r":
						return HumanBodyBones.RightFoot.ToString();
					case "foot.l":
						return null;
					case "foot.r":
						return null;
				}
			}
			return STF_To_Unity_Mappings[STFMapping];
		}
	}

	public class STFEXP_Humanoid_Armature_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STFEXP_Humanoid_Armature);

		public uint Order => 10;

		public int Priority => 1;

		public (List<UnityEngine.Object>, List<UnityEngine.Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var humanoid = STFResource as STFEXP_Humanoid_Armature;

			Avatar avatar = null;
			if(humanoid.bone_mappings.Count >= 15)
			{
				var armature = humanoid.gameObject.GetComponent<STF_Armature>();
				var potentialBoneList = Context.Root.GetComponentsInChildren<Transform>().ToArray();

				var humanDescription = new HumanDescription
				{
					upperArmTwist = humanoid.settings.upper_arm_twist,
					lowerArmTwist = humanoid.settings.lower_arm_twist,
					upperLegTwist = humanoid.settings.upper_leg_twist,
					lowerLegTwist = humanoid.settings.lower_leg_twist,
					armStretch = humanoid.settings.arm_stretch,
					legStretch = humanoid.settings.leg_stretch,
					feetSpacing = humanoid.settings.feet_spacing,
					hasTranslationDoF = humanoid.settings.use_translation,

					skeleton = potentialBoneList.Select(t => {
						return new SkeletonBone() {
							name = t.name,
							position = t.localPosition,
							rotation = t.localRotation,
							scale = t.localScale,
						};
					}).ToArray(),

					human = humanoid.bone_mappings.Where(mapping => {
						return Humanoid_Armature_Mappings.TranslateHumanoidSTFtoUnity(mapping.Mapping, humanoid.locomotion_type, humanoid.no_jaw) != null;
					}).Select(mapping => {
						var bone = humanoid.gameObject.GetComponentsInChildren<STF_Bone>().FirstOrDefault(b => b.STF_Id == mapping.BoneID && b.STF_Owner == armature);

						if(bone == null || !Humanoid_Armature_Mappings.STF_To_Unity_Mappings.ContainsKey(mapping.Mapping))
							return default;

						var humanLimit = new HumanLimit {useDefaultValues = true};
						if(mapping.set_rotation_limits)
						{
							humanLimit.useDefaultValues = false;
							humanLimit.axisLength = bone.Length;
							humanLimit.min = new Vector3(Mathf.Rad2Deg * mapping.p_min, Mathf.Rad2Deg * mapping.s_min, Mathf.Rad2Deg * mapping.t_min);
							humanLimit.center = new Vector3(Mathf.Rad2Deg * mapping.p_center, Mathf.Rad2Deg * mapping.s_center, Mathf.Rad2Deg * mapping.t_center);
							humanLimit.max = new Vector3(Mathf.Rad2Deg * mapping.p_max, Mathf.Rad2Deg * mapping.s_max, Mathf.Rad2Deg * mapping.t_max);
						}

						var humanBone = new HumanBone {
							humanName = Humanoid_Armature_Mappings.TranslateHumanoidSTFtoUnity(mapping.Mapping, humanoid.locomotion_type, humanoid.no_jaw),
							boneName = bone.name,
							limit = humanLimit,
						};
						return humanBone;
					}).ToArray(),
				};

				avatar = AvatarBuilder.BuildHumanAvatar(Context.Root, humanDescription);
				avatar.name = humanoid.transform.name + "Avatar";

				if (!avatar.isValid)
					throw new System.Exception("Invalid humanoid avatar");
			}
			else
			{
				avatar = UnityHumanoidMappingUtil.GenerateAvatar(Context, humanoid.transform, humanoid.locomotion_type, humanoid.no_jaw);
			}

			avatar.name = "Unity Avatar";

			Animator animator = Context.Root.GetComponent<Animator>();
			if(!animator) animator = Context.Root.AddComponent<Animator>();

			animator.applyRootMotion = true;
			animator.updateMode = AnimatorUpdateMode.Normal;
			animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
			animator.avatar = avatar;

			return (new() { avatar }, new() { avatar });
		}
	}
}
