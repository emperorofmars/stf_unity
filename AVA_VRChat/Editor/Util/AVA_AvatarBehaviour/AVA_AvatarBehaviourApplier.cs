#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace com.squirrelbite.stf_unity.ava.vrchat.util
{

	public static class AVA_AvatarBehaviourApplier
	{
		public static readonly ReadOnlyDictionary<HandGesture, int> HandGestureToParameterIndex = new(new Dictionary<HandGesture, int>()
		{
			{ HandGesture.None, 0 },
			{ HandGesture.Fist, 1 },
			{ HandGesture.Open, 2 },
			{ HandGesture.Point, 3 },
			{ HandGesture.Peace, 4 },
			{ HandGesture.RockNRoll, 5 },
			{ HandGesture.Gun, 6 },
			{ HandGesture.ThumbsUp, 7 },
		});

		public static void Apply(AVA_AvatarBehaviourSetup Setup, bool WriteToDisk = false)
		{
			var avatar = Setup.gameObject.GetComponent<VRCAvatarDescriptor>();

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

		private static AnimatorState AddSingleSideHandGestureState(AVA_AvatarBehaviourSetup Setup, AnimatorControllerLayer Layer, HandGesture Gesture, bool IsLeft, string OverrideGestureName = null)
		{
			var state = new AnimatorState { name = string.IsNullOrWhiteSpace(OverrideGestureName) ? Gesture.ToString() : OverrideGestureName, writeDefaultValues = true, timeParameterActive = true };
			Layer.stateMachine.AddState(state, new Vector3(300, 60 * HandGestureToParameterIndex[Gesture], 0));
			var transitionIdle = Layer.stateMachine.AddAnyStateTransition(state);
			transitionIdle.AddCondition(AnimatorConditionMode.Equals, HandGestureToParameterIndex[Gesture], IsLeft ? "GestureLeft" : "GestureRight");
			transitionIdle.hasExitTime = false;
			
			if (Setup.EmoteBindings.Find(b => IsLeft ? b.GuestureLeftHand == Gesture : b.GuestureRightHand == Gesture) is var emoteBinding && emoteBinding != null && !string.IsNullOrWhiteSpace(emoteBinding.Emote))
			{
				if (Setup.Emotes.Find(e => e.Emote == emoteBinding.Emote) is var emote && emote != null)
				{
					state.motion = emote.Animation;
					state.timeParameter = emoteBinding.UseTriggerIntensity > 0 ? emoteBinding.UseTriggerIntensity == TriggerIntensity.Left ? "GestureLeftWeight" : "GestureRightWeight" : null;
				}
			}
			return state;
		}

		private static AnimatorState AddHandGestureState(AVA_AvatarBehaviourSetup Setup, AnimatorControllerLayer Layer, HandGesture GestureLeft, HandGesture GestureRight, string Name, int Index)
		{
			var state = new AnimatorState { name = Name, writeDefaultValues = true, timeParameterActive = true };
			Layer.stateMachine.AddState(state, new Vector3(300, 60 * Index, 0));
			var transitionIdle = Layer.stateMachine.AddAnyStateTransition(state);
			transitionIdle.AddCondition(AnimatorConditionMode.Equals, HandGestureToParameterIndex[GestureLeft], "GestureLeft");
			transitionIdle.AddCondition(AnimatorConditionMode.Equals, HandGestureToParameterIndex[GestureRight], "GestureRight");
			transitionIdle.hasExitTime = false;

			var transitionExitLeft = state.AddExitTransition(false);
			transitionExitLeft.AddCondition(AnimatorConditionMode.NotEqual, HandGestureToParameterIndex[GestureLeft], "GestureLeft");
			var transitionExitRight = state.AddExitTransition(false);
			transitionExitRight.AddCondition(AnimatorConditionMode.NotEqual, HandGestureToParameterIndex[GestureRight], "GestureRight");
			
			if (Setup.EmoteBindings.Find(b => b.GuestureLeftHand == GestureLeft && b.GuestureRightHand == GestureRight) is var emoteBinding && emoteBinding != null && !string.IsNullOrWhiteSpace(emoteBinding.Emote))
			{
				if (Setup.Emotes.Find(e => e.Emote == emoteBinding.Emote) is var emote && emote != null)
				{
					state.motion = emote.Animation;
					state.timeParameter = emoteBinding.UseTriggerIntensity > 0 ? emoteBinding.UseTriggerIntensity == TriggerIntensity.Left ? "GestureLeftWeight" : "GestureRightWeight" : null;
				}
			}
			return state;
		}

		private static void SetupEmotes(AVA_AvatarBehaviourSetup Setup, AnimatorController animatorFX)
		{
			var layerHandLeft = new AnimatorControllerLayer { name = "Left Hand", stateMachine = new AnimatorStateMachine(), defaultWeight = 1 };
			{
				AddSingleSideHandGestureState(Setup, layerHandLeft, HandGesture.None, true, "Idle");
				AddSingleSideHandGestureState(Setup, layerHandLeft, HandGesture.Fist, true);
				AddSingleSideHandGestureState(Setup, layerHandLeft, HandGesture.Open, true);
				AddSingleSideHandGestureState(Setup, layerHandLeft, HandGesture.Point, true);
				AddSingleSideHandGestureState(Setup, layerHandLeft, HandGesture.Peace, true);
				AddSingleSideHandGestureState(Setup, layerHandLeft, HandGesture.RockNRoll, true);
				AddSingleSideHandGestureState(Setup, layerHandLeft, HandGesture.Gun, true);
				AddSingleSideHandGestureState(Setup, layerHandLeft, HandGesture.ThumbsUp, true);
			}

			var layerHandRight = new AnimatorControllerLayer { name = "Right Hand", stateMachine = new AnimatorStateMachine(), defaultWeight = 1 };
			{
				AddSingleSideHandGestureState(Setup, layerHandRight, HandGesture.None, false, "Idle");
				AddSingleSideHandGestureState(Setup, layerHandRight, HandGesture.Fist, false);
				AddSingleSideHandGestureState(Setup, layerHandRight, HandGesture.Open, false);
				AddSingleSideHandGestureState(Setup, layerHandRight, HandGesture.Point, false);
				AddSingleSideHandGestureState(Setup, layerHandRight, HandGesture.Peace, false);
				AddSingleSideHandGestureState(Setup, layerHandRight, HandGesture.RockNRoll, false);
				AddSingleSideHandGestureState(Setup, layerHandRight, HandGesture.Gun, false);
				AddSingleSideHandGestureState(Setup, layerHandRight, HandGesture.ThumbsUp, false);
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
				var layerHands = new AnimatorControllerLayer { name = "Hands", stateMachine = new AnimatorStateMachine(), defaultWeight = 1 };
				{
					var stateIdle = new AnimatorState { name = "Idle", writeDefaultValues = true, timeParameterActive = true };
					layerHands.stateMachine.AddState(stateIdle, new Vector3(300, 0, 0));
				}
				animatorFX.AddLayer(layerHands);
				var index = 1;
				foreach (var binding in Setup.EmoteBindings)
				{
					if (binding.GuestureLeftHand > 0 && binding.GuestureRightHand > 0 && !string.IsNullOrWhiteSpace(binding.Emote))
					{
						AddHandGestureState(Setup, layerHands, binding.GuestureLeftHand, binding.GuestureRightHand, binding.Emote, index);
					}
					index++;
				}
			}
			else
			{
				var layerHands = new AnimatorControllerLayer { name = "Hands", stateMachine = new AnimatorStateMachine(), defaultWeight = 1 };
				{
					var stateIdle = new AnimatorState { name = "Idle", writeDefaultValues = true, timeParameterActive = true };
					layerHands.stateMachine.AddState(stateIdle, new Vector3(300, 0, 0));
				}
				animatorFX.AddLayer(layerHands);
				var index = 1;
				foreach (var binding in Setup.EmoteBindings)
				{
					if (binding.GuestureLeftHand > 0 && binding.GuestureRightHand > 0 && !string.IsNullOrWhiteSpace(binding.Emote))
					{
						AddHandGestureState(Setup, layerHands, binding.GuestureLeftHand, binding.GuestureRightHand, binding.Emote, index);
					}
					index++;
				}
			}
		}
	}

}

#endif
#endif