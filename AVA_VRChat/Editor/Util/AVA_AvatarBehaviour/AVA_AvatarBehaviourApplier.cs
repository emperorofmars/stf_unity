#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace com.squirrelbite.stf_unity.ava.vrchat.util
{

	public static class AVA_AvatarBehaviourApplier
	{
		public static void Apply(AVA_AvatarBehaviourSetup Setup, bool WriteToDisk = false)
		{
			var avatar = Setup.GetComponent<VRCAvatarDescriptor>();

			var animatorFX = SetupBaseFX();
			SetupEmotes(Setup, animatorFX);

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

		private static AnimatorController SetupBaseFX()
		{
			var animatorFX = new AnimatorController { name = "FX" };
			var animatorFXLayer0 = new AnimatorControllerLayer
			{
				name = "All Parts",
				stateMachine = new AnimatorStateMachine()
			};
			animatorFX.AddLayer(animatorFXLayer0);
			animatorFX.AddParameter("GestureLeft", AnimatorControllerParameterType.Int);
			animatorFX.AddParameter("GestureLeftWeight", AnimatorControllerParameterType.Float);
			animatorFX.AddParameter("GestureRight", AnimatorControllerParameterType.Int);
			animatorFX.AddParameter("GestureRightWeight", AnimatorControllerParameterType.Float);

			return animatorFX;
		}

		private static void SetupEmotes(AVA_AvatarBehaviourSetup Setup, AnimatorController animatorFX)
		{
			var layerHandLeft = new AnimatorControllerLayer { name = "Left Hand", stateMachine = new AnimatorStateMachine() };
			{
				var stateIdle = new AnimatorState { name = "Idle", writeDefaultValues = true };

				layerHandLeft.stateMachine.AddState(stateIdle, new Vector3(0, 0, 0));
			}

			var layerHandRight = new AnimatorControllerLayer { name = "Right Hand", stateMachine = new AnimatorStateMachine() };
			{
				var stateIdle = new AnimatorState { name = "Idle", writeDefaultValues = true };

				layerHandRight.stateMachine.AddState(stateIdle, new Vector3(0, 0, 0));
			}

			if (Setup.HandDominance == HandDominance.Right)
			{
				animatorFX.AddLayer(layerHandLeft);
				animatorFX.AddLayer(layerHandRight);
			}
			else if (Setup.HandDominance == HandDominance.Left)
			{
				animatorFX.AddLayer(layerHandRight);
				animatorFX.AddLayer(layerHandLeft);
			}

			if (Setup.HandDominance == HandDominance.Explicit)
			{
				var layerHands = new AnimatorControllerLayer { name = "Left Hand", stateMachine = new AnimatorStateMachine() };
				{
					var stateIdle = new AnimatorState { name = "Idle", writeDefaultValues = true };

					layerHands.stateMachine.AddState(stateIdle, new Vector3(0, 0, 0));
				}
				animatorFX.AddLayer(layerHands);
			}
		}
	}

}

#endif
#endif