#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDKBase.Editor.BuildPipeline;

namespace com.squirrelbite.stf_unity.ava.vrchat.util
{
	public enum TriggerIntensity { None = 0, Left = 1, Right = 2 };
	public enum HandDominance { Explicit, Left, Right };
	public enum HandGesture { None, Fist, Open, Point, Peace, RockNRoll, Gun, ThumbsUp };

	[System.Serializable]
	public class AvatarEmote
	{
		public string Emote;
		public AnimationClip Animation;
	}

	[System.Serializable]
	public class AvatarEmoteBinding
	{
		public string Emote;
		public HandGesture GuestureLeftHand = HandGesture.None;
		public HandGesture GuestureRightHand = HandGesture.None;
		public TriggerIntensity UseTriggerIntensity = TriggerIntensity.None;
	}

	/// <summary>
	/// Opinionated base setup for VR avatars.
	/// </summary>
	[HelpURL("https://github.com/emperorofmars/stf_unity")]
	public class AVA_AvatarBehaviourSetup : MonoBehaviour, IEditorOnly
	{
		public HandDominance HandDominance = HandDominance.Right;
		public List<AvatarEmote> Emotes = new();
		public List<AvatarEmoteBinding> EmoteBindings = new();

		public bool CreateEyeJoystickPuppet = true;

		// TODO Toggles, JoystickPuppets, Other stuff

		public void AddAvatarEmote(AvatarEmote Emote)
		{
			Emotes.Add(Emote);
			EmoteBindings.Add(new AvatarEmoteBinding() { Emote = Emote.Emote });
		}
	}

	[InitializeOnLoad]
	public class AVA_AvatarBehaviourSetup_BuildHook : IVRCSDKPreprocessAvatarCallback
	{
		// Run before VRCFury (-10000) and ModularAvatar (-11000)
		public int callbackOrder => -20000;

		public bool OnPreprocessAvatar(GameObject Root)
		{
			if (Root.GetComponent<AVA_AvatarBehaviourSetup>() is var avatarBehaviour && avatarBehaviour != null)
			{
				try
				{
					AVA_AvatarBehaviourApplier.Apply(avatarBehaviour);
					return true;
				}
				catch (System.Exception exception)
				{
					Debug.LogError(exception);
					return false;
				}
				finally
				{
#if UNITY_EDITOR
					Object.DestroyImmediate(avatarBehaviour);
#else
					Object.Destroy(avatarBehaviour);
#endif
				}
			}
			return true; // Nothing to do
		}
	}
}

#endif
#endif
