#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace com.squirrelbite.stf_unity.ava.vrchat.util
{
	[CustomEditor(typeof(AVA_AvatarBehaviourSetup))]
	public class AVA_AvatarBehaviourInspector : Editor
	{

		void OnEnable()
		{
		}

		public override void OnInspectorGUI()
		{
			var c = (AVA_AvatarBehaviourSetup)target;

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.LabelField("Bind Emotes to Hand-Gestures");

			GUILayout.Space(20f);

			EditorGUILayout.LabelField("Emotes");
			EditorGUI.indentLevel++;
			for (int i = 0; i < c.Emotes.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Emote Meaning");
				c.Emotes[i].Emote = EditorGUILayout.TextField(c.Emotes[i].Emote);
				if (GUILayout.Button("X"))
				{
					c.Emotes.RemoveAt(i);
					i--;
					continue;
				}
				EditorGUILayout.EndHorizontal();

				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("Emotes").GetArrayElementAtIndex(i).FindPropertyRelative("Animation"));
				EditorGUI.indentLevel--;

				if (i < c.Emotes.Count) GUILayout.Space(10f);
			}
			EditorGUI.indentLevel--;
			if (GUILayout.Button("Add Emote"))
			{
				c.Emotes.Add(new AvatarEmote());
			}

			GUILayout.Space(20f);

			EditorGUILayout.LabelField("Bindings");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("HandDominance"));
			
			GUILayout.Space(10f);

			EditorGUI.indentLevel++;
			if (c.Emotes.Count == 0)
			{
				EditorGUILayout.LabelField("No emotes to bind!");
			}
			else
			{
				var bindingDropdownSet = new HashSet<string>();
				foreach (var emote in c.Emotes)
				{
					if (!bindingDropdownSet.Contains(emote.Emote))
						bindingDropdownSet.Add(emote.Emote);
				}
				var bindingDropdown = bindingDropdownSet.ToList();
				bindingDropdown.Sort();

				for (int i = 0; i < c.EmoteBindings.Count; i++)
				{
					var selectedIndex = Math.Min(bindingDropdown.FindIndex(b => b == c.EmoteBindings[i].Emote), 0);
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel("Emote");
					var newSelectedIndex = EditorGUILayout.Popup(selectedIndex, bindingDropdown.ToArray());
					if (GUILayout.Button("X"))
					{
						c.EmoteBindings.RemoveAt(i);
						i--;
						continue;
					}
					EditorGUILayout.EndHorizontal();
					if (newSelectedIndex != selectedIndex)
					{
						c.EmoteBindings[i].Emote = bindingDropdown[newSelectedIndex];
					}

					EditorGUI.indentLevel++;
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel("Left Hand");
					c.EmoteBindings[i].GuestureLeftHand = (HandGesture)EditorGUILayout.EnumPopup(c.EmoteBindings[i].GuestureLeftHand);
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel("Right Hand");
					c.EmoteBindings[i].GuestureRightHand = (HandGesture)EditorGUILayout.EnumPopup(c.EmoteBindings[i].GuestureRightHand);
					EditorGUILayout.EndHorizontal();

					if (string.IsNullOrWhiteSpace(c.EmoteBindings[i].Emote) || c.EmoteBindings[i].GuestureLeftHand == HandGesture.None && c.EmoteBindings[i].GuestureRightHand == HandGesture.None)
					{
						EditorGUILayout.LabelField("Empty Binding! Please select emote and mappings.");
					}
					else
					{
						for (int nestedI = 0; nestedI < c.EmoteBindings.Count; nestedI++)
						{
							if (nestedI != i)
							{
								if (c.EmoteBindings[nestedI].GuestureLeftHand == c.EmoteBindings[i].GuestureLeftHand && c.EmoteBindings[nestedI].GuestureRightHand == c.EmoteBindings[i].GuestureRightHand)
								{
									EditorGUILayout.LabelField("Duplicate Binding! Please select different mappings.");
								}
							}
						}
					}

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel("Use Trigger Intensity");
					if (c.EmoteBindings[i].GuestureLeftHand > 0 && c.EmoteBindings[i].GuestureRightHand > 0)
					{
						c.EmoteBindings[i].UseTriggerIntensity = (TriggerIntensity)EditorGUILayout.EnumPopup(c.EmoteBindings[i].UseTriggerIntensity);
					}
					else
					{
						bool useIntensity = c.EmoteBindings[i].UseTriggerIntensity > 0;
						bool useIntensityNew = EditorGUILayout.Toggle(useIntensity);
						if (useIntensity != useIntensityNew)
						{
							if (useIntensityNew)
								c.EmoteBindings[i].UseTriggerIntensity = c.EmoteBindings[i].GuestureLeftHand > 0 ? TriggerIntensity.Left : TriggerIntensity.Right;
							else
								c.EmoteBindings[i].UseTriggerIntensity = TriggerIntensity.None;
						}
					}
					EditorGUILayout.EndHorizontal();
					EditorGUI.indentLevel--;

					if (i < c.EmoteBindings.Count) GUILayout.Space(10f);
				}
				if (GUILayout.Button("Add Binding"))
				{
					c.EmoteBindings.Add(new AvatarEmoteBinding());
				}
			}
			EditorGUI.indentLevel--;

			GUILayout.Space(10f);

			if(EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(c);
			}
			
		}
	}
}

#endif
#endif
