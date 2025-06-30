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

		private static AnimatorState AddHandGestureState(AVA_AvatarBehaviourSetup Setup, AnimatorControllerLayer Layer, HandGesture Gesture, bool IsLeft, string OverrideGestureName = null)
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

		private static void SetupEmotes(AVA_AvatarBehaviourSetup Setup, AnimatorController animatorFX)
		{
			var layerHandLeft = new AnimatorControllerLayer { name = "Left Hand", stateMachine = new AnimatorStateMachine(), defaultWeight = 1 };
			{
				AddHandGestureState(Setup, layerHandLeft, HandGesture.None, true, "Idle");
				AddHandGestureState(Setup, layerHandLeft, HandGesture.Fist, true);
				AddHandGestureState(Setup, layerHandLeft, HandGesture.Open, true);
				AddHandGestureState(Setup, layerHandLeft, HandGesture.Point, true);
				AddHandGestureState(Setup, layerHandLeft, HandGesture.Peace, true);
				AddHandGestureState(Setup, layerHandLeft, HandGesture.RockNRoll, true);
				AddHandGestureState(Setup, layerHandLeft, HandGesture.Gun, true);
				AddHandGestureState(Setup, layerHandLeft, HandGesture.ThumbsUp, true);
			}

			var layerHandRight = new AnimatorControllerLayer { name = "Right Hand", stateMachine = new AnimatorStateMachine(), defaultWeight = 1 };
			{
				AddHandGestureState(Setup, layerHandRight, HandGesture.None, false, "Idle");
				AddHandGestureState(Setup, layerHandRight, HandGesture.Fist, false);
				AddHandGestureState(Setup, layerHandRight, HandGesture.Open, false);
				AddHandGestureState(Setup, layerHandRight, HandGesture.Point, false);
				AddHandGestureState(Setup, layerHandRight, HandGesture.Peace, false);
				AddHandGestureState(Setup, layerHandRight, HandGesture.RockNRoll, false);
				AddHandGestureState(Setup, layerHandRight, HandGesture.Gun, false);
				AddHandGestureState(Setup, layerHandRight, HandGesture.ThumbsUp, false);
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
			}
		}
	}

}

#endif
#endif