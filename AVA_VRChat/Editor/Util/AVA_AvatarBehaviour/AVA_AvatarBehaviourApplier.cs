#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using UnityEditor;
using UnityEditor.Animations;
using VRC.SDK3.Avatars.Components;

namespace com.squirrelbite.stf_unity.ava.vrchat.util
{

	public static class AVA_AvatarBehaviourApplier
	{
		public static void Apply(AVA_AvatarBehaviourSetup Setup, bool WriteToDisk = false)
		{
			var avatar = Setup.GetComponent<VRCAvatarDescriptor>();

			var animatorFX = new AnimatorController { name = "FX" };
			var animatorFXLayer0 = new AnimatorControllerLayer
			{
				name = "All Parts",
				stateMachine = new AnimatorStateMachine()
			};
			animatorFX.AddLayer(animatorFXLayer0);
			var animatorFXLayer1 = new AnimatorControllerLayer
			{
				name = "Left Hand",
				stateMachine = new AnimatorStateMachine()
			};
			animatorFXLayer1.stateMachine.AddState("Idle");
			animatorFX.AddLayer(animatorFXLayer1);

			avatar.customizeAnimationLayers = true;
			avatar.baseAnimationLayers = new VRCAvatarDescriptor.CustomAnimLayer[]
			{
				new () { type = VRCAvatarDescriptor.AnimLayerType.Base, isDefault = true },
				new () { type = VRCAvatarDescriptor.AnimLayerType.Additive, isDefault = true},
				new () { type = VRCAvatarDescriptor.AnimLayerType.Gesture, isDefault = true},
				new () { type = VRCAvatarDescriptor.AnimLayerType.Action, isDefault = true},
				new () { type = VRCAvatarDescriptor.AnimLayerType.FX, isDefault = false, animatorController = animatorFX, isEnabled = true },
			};

			if (WriteToDisk)
			{
				AssetDatabase.CreateAsset(animatorFX, "Assets/AnimatorFX.controller");
			}

			return;
		}
	}

}

#endif
#endif