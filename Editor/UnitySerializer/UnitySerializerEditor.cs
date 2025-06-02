#if UNITY_EDITOR

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace com.squirrelbite.stf_unity.serialization
{
	/// <summary>
	/// Editor window into which a user can drag any object. This will try to serialize it into Json and name definitions, either based on manual implementations in the Unity_Serializer_Registry, or with Unity's JsonUtility as a fallback.
	/// </summary>
	public class UnitySerializerEditor : EditorWindow
	{
		private Vector2 scrollPos;
		private Object Selected;

		private List<SerializerResult> SerializerResult = new();
		private string SetupString = "";
		private string FallbackJson = "";

		[MenuItem("STF Tools/Unity Serialization Utility")]
		public static void Init()
		{
			UnitySerializerEditor window = EditorWindow.GetWindow(typeof(UnitySerializerEditor)) as UnitySerializerEditor;
			window.titleContent = new GUIContent("Convert Objects to STF Json Resources");
			window.minSize = new Vector2(600, 700);
			window.Selected = null;
		}

		void OnGUI()
		{
			GUILayout.Space(5);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Select Object", EditorStyles.whiteLargeLabel, GUILayout.ExpandWidth(false));
			var newSelected = (Object)EditorGUILayout.ObjectField(
				Selected,
				typeof(Object),
				true,
				GUILayout.ExpandWidth(true)
			);
			GUILayout.EndHorizontal();

			// Run the registered serializers on the selected object
			if(newSelected != Selected || SerializerResult == null || SerializerResult.Count == 0)
			{
				Selected = newSelected;
				SerializerResult = new List<SerializerResult>();
				if(Selected)
				{
					SerializerResult = Unity_Serializer_Runner.Run(Selected);
					SetupString = Unity_Serializer_Runner.CreateSetupString(SerializerResult);
				}
				else
				{
					try
					{
						FallbackJson = JObject.Parse(JsonUtility.ToJson(Selected)).ToString(Newtonsoft.Json.Formatting.Indented);
					}
					catch(System.Exception)
					{
						FallbackJson = "No Serializer detected! Fallback JsonUtility Failed!";
					}
				}
			}

			// Draw the results
			if(Selected && SerializerResult.Count == 0)
			{
				GUILayout.Space(10);
				GUILayout.Label("JsonUtility Fallback Result", GUILayout.ExpandWidth(false));
				scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(position.height - 20));
				EditorGUILayout.TextArea(FallbackJson);
				EditorGUILayout.EndScrollView();
			}
			else if(Selected)
			{
				GUILayout.Space(10);
				GUILayout.BeginHorizontal();
					GUILayout.Label("Parsed NNA Definitions", GUILayout.ExpandWidth(false));
					//GUILayout.Label("In Blender create a new 'Raw Json' component on the appropriate Object or Bone, and paste the text inside.", GUILayout.ExpandWidth(false));

					/*GUILayout.Space(10);

					var oldJsonPreference = JsonPreference;
					JsonPreference = GUILayout.Toggle(JsonPreference, "Prefer Json Definition");
					if(oldJsonPreference != JsonPreference)
					{
						SetupString = RunNNASerializer.CreateSetupString(SerializerResult, JsonPreference);
					}

					if(!string.IsNullOrWhiteSpace(SetupString) && GUILayout.Button("Copy Full Setup to Clipboard", GUILayout.ExpandWidth(false)))
					{
						GUIUtility.systemCopyBuffer = SetupString;
					}*/

					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

				GUILayout.Space(5);
				DrawHLine(2, 0);

				scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(position.height - 67));
				foreach(var result in SerializerResult)
				{
					GUILayout.BeginHorizontal();
						GUILayout.Label(result.STFType, EditorStyles.whiteLargeLabel);
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
						GUILayout.Space(20);
						GUILayout.BeginVertical();
							GUILayout.BeginHorizontal();
								GUILayout.Label("Origin");
								EditorGUILayout.ObjectField(result.Origin, typeof(UnityEngine.Object), true, GUILayout.Width(400));
							GUILayout.EndHorizontal();
						GUILayout.EndVertical();
						GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();

					GUILayout.Space(5);

					GUILayout.BeginHorizontal();
						GUILayout.Space(20);
						GUILayout.BeginVertical();
							GUILayout.BeginHorizontal(GUILayout.Width(250));
								GUILayout.Label("Json STF Resource");
								if(!result.IsComplete && result.Result != null && result.Result.Count > 0) GUILayout.Label("(Incomplete)");
							GUILayout.EndHorizontal();
							GUILayout.BeginHorizontal();
								GUILayout.Space(20);
								GUILayout.BeginVertical();
									if(result.Result != null && result.Result.Count > 0)
									{
										//GUILayout.Label("Target: " + result.JsonTargetNode);
										if(GUILayout.Button("Copy to Clipboard", GUILayout.ExpandWidth(false)))
										{
											GUIUtility.systemCopyBuffer = result.Result.ToString(Newtonsoft.Json.Formatting.None);
										}
									}
									else
									{
										GUILayout.Label("Not Serialized");
									}
								GUILayout.EndVertical();
							GUILayout.EndHorizontal();
						GUILayout.EndVertical();

						GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();

					DrawHLine(1);
				}
				EditorGUILayout.EndScrollView();
			}
		}

		private void DrawHLine(float Thickness = 2, float Spacers = 10) {
			if(Spacers > 0) GUILayout.Space(Spacers);
			EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, Thickness), Color.gray);
			if(Spacers > 0) GUILayout.Space(Spacers);
		}
	}
}

#endif