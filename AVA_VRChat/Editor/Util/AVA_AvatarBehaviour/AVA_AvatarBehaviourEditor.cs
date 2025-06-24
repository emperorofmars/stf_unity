#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using UnityEditor;
using UnityEngine;
using VRC.SDKBase;

namespace com.squirrelbite.stf_unity.ava.vrchat.util
{
	public class AVA_AvatarBehaviourEditor : EditorWindow
	{
		private VRC_AvatarDescriptor Selected;
		private bool WriteAnimatorsToDisk = true;
		private string ErrorMessage = null;
		private bool Success = false;

		[MenuItem("STF Tools/AVA_VRChat/Apply Avatar Behaviour")]
		public static void Init()
		{
			AVA_AvatarBehaviourEditor window = GetWindow(typeof(AVA_AvatarBehaviourEditor)) as AVA_AvatarBehaviourEditor;
			window.titleContent = new GUIContent("Apply Avatar Behaviour");
			window.minSize = new Vector2(500, 700);
			window.Selected = null;
		}

		void OnGUI()
		{
			GUILayout.Space(5);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Select Object", EditorStyles.whiteLargeLabel, GUILayout.ExpandWidth(false));
			var newSelected = (VRC_AvatarDescriptor)EditorGUILayout.ObjectField(
				Selected,
				typeof(VRC_AvatarDescriptor),
				true,
				GUILayout.ExpandWidth(true)
			);
			GUILayout.EndHorizontal();
			GUILayout.Space(5);
			GUILayout.BeginHorizontal();
			WriteAnimatorsToDisk = GUILayout.Toggle(WriteAnimatorsToDisk, "WriteAnimatorsToDisk");
			GUILayout.EndHorizontal();

			if(newSelected != Selected && newSelected.GetComponent<AVA_AvatarBehaviourSetup>())
			{
				Selected = newSelected;
				ErrorMessage = null;
				Success = false;
			}

			if(Success)
			{
				GUILayout.Space(10);
				GUILayout.Label("Success!", EditorStyles.label, GUILayout.ExpandWidth(false));
			}
			if(!string.IsNullOrWhiteSpace(ErrorMessage))
			{
				GUILayout.Space(10);
				GUILayout.Label("Error: " + ErrorMessage, EditorStyles.label, GUILayout.ExpandWidth(false));
			}

			GUILayout.Space(15);

			if(Selected)
			{
				if(GUILayout.Button("Apply on Copy"))
				{
					ErrorMessage = null;
					Success = false;
					try
					{
						var instance = Instantiate(Selected.gameObject);
						instance.name = Selected.name + "_AvatarBehavoiur_Applied";
						AVA_AvatarBehaviourApplier.Apply(instance.GetComponent<AVA_AvatarBehaviourSetup>(), WriteAnimatorsToDisk);
						Success = true;
					}
					catch(System.Exception exception)
					{
						Debug.LogException(exception);
						ErrorMessage = exception.Message;
					}
				}
			}
		}
	}
}

#endif
#endif