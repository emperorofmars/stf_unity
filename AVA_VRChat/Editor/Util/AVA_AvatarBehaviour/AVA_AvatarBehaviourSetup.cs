#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDKBase.Editor.BuildPipeline;

namespace com.squirrelbite.stf_unity.ava.vrchat.util
{
	[System.Serializable]
	public class AvatarEmote
	{
		public string Meaning;
		public AnimationClip Animation;
		public bool EyeblinkActive = true;
		public float BreathingSpeed = 0.5f;
		public float BreathingIntensity = 0.5f;
	}

	[System.Serializable]
	public class AvatarEmoteBinding
	{
		public string Emote;
		public string GuestureLeftHand;
		public string GuestureRightHand;
		public bool UseTriggerIntensity = true;
	}

	[HelpURL("https://github.com/emperorofmars/stf_unity")]
	public class AVA_AvatarBehaviourSetup : MonoBehaviour, IEditorOnly
	{
		public List<AvatarEmote> Emotes = new();
		public List<AvatarEmoteBinding> EmoteBindings = new();

		public void AddAvatarEmote(AvatarEmote Emote)
		{
			Emotes.Add(Emote);
			EmoteBindings.Add(new AvatarEmoteBinding() { Emote = Emote.Meaning });
		}
	}

	[InitializeOnLoad]
	public class AVA_AvatarBehaviourSetup_BuildHook : IVRCSDKPreprocessAvatarCallback
	{
		public int callbackOrder => 1;

		public bool OnPreprocessAvatar(GameObject Root)
		{
			var fixComponent = Root.GetComponent<AVA_AvatarBehaviourSetup>();
			try
			{
				Debug.Log("Wooo");
				// TODO
				return true;
			}
			catch (System.Exception exception)
			{
				Debug.LogError(exception);
				return false;
			}
		}
	}
}

#endif
#endif
