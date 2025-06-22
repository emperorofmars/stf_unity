#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using UnityEditor;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDKBase.Editor.BuildPipeline;

namespace com.squirrelbite.stf_unity.ava.vrchat.components
{
	[HelpURL("https://github.com/emperorofmars/stf_unity")]
	public class VRC_AVA_BaseSetupApplier : MonoBehaviour, IEditorOnly
	{
	}

	[InitializeOnLoad]
	public class AvatarBuildHook : IVRCSDKPreprocessAvatarCallback
	{
		public int callbackOrder => 1;

		public bool OnPreprocessAvatar(GameObject Root)
		{
			var fixComponent = Root.GetComponent<VRC_AVA_BaseSetupApplier>();
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
