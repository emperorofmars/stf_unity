#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace com.squirrelbite.stf_unity.tools
{
	// A UI for the STFScriptedImporter.
	[CustomEditor(typeof(STFScriptedImporter))]
	public class STFScriptedImporterInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			var importer = (STFScriptedImporter)target;

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Authoring Import");
			var authoringImport = EditorGUILayout.Toggle(importer.AuthoringImport);
			EditorGUILayout.EndHorizontal();
			if(authoringImport != importer.AuthoringImport)
			{
				importer.AuthoringImport = authoringImport;
				EditorUtility.SetDirty(importer);
			}

			if(!importer.AuthoringImport)
			{
				var availableContexts = STF_Processor_Registry.GetAvaliableContexts();

				int selectedIndex = availableContexts.FindIndex(c => c == importer.SelectedApplication);

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Select Import Context");
				var newSelectedIndex = EditorGUILayout.Popup(selectedIndex, availableContexts.ToArray());
				EditorGUILayout.EndHorizontal();

				if(newSelectedIndex != selectedIndex && newSelectedIndex >= 0 && newSelectedIndex < availableContexts.Count)
				{
					importer.SelectedApplication = availableContexts[newSelectedIndex];
					EditorUtility.SetDirty(importer);
				}
			}

			drawHLine();

			if(AssetDatabase.LoadAssetAtPath<STF_Import>(importer.assetPath) is var stfImport)
			{
				renderAsset(stfImport);
			}
			else
			{
				EditorGUILayout.LabelField("Import Failed");
			}

			drawHLine();

			if(GUILayout.Button("Reimport"))
			{
				EditorUtility.SetDirty(importer);
				importer.SaveAndReimport();
			}
		}

		private void renderAsset(STF_Import asset)
		{
			if(asset != null)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Asset Name");
				EditorGUILayout.LabelField(asset.Meta?.STFAssetInfo?.AssetName);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Asset Version");
				EditorGUILayout.LabelField(asset.Meta?.STFAssetInfo?.Version);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Author");
				EditorGUILayout.LabelField(asset.Meta?.STFAssetInfo?.Author);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("URL");
				EditorGUILayout.LabelField(asset.Meta?.STFAssetInfo?.URL);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("License");
				EditorGUILayout.LabelField(asset.Meta?.STFAssetInfo?.License);
				EditorGUILayout.EndHorizontal();

				// TODO make these clickable
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("License URL");
				EditorGUILayout.LabelField(asset.Meta?.STFAssetInfo?.LicenseURL);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Documentation URL");
				EditorGUILayout.LabelField(asset.Meta?.STFAssetInfo?.DocumentationURL);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Binary Version");
				EditorGUILayout.LabelField($"{asset.BinaryVersionMajor}.{asset.BinaryVersionMinor}");
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Definition Version");
				EditorGUILayout.LabelField($"{asset.Meta?.DefinitionVersionMajor}.{asset.Meta?.DefinitionVersionMinor}");
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				EditorGUILayout.LabelField("Invalid Asset");
			}
		}

		private void drawHLine() {
			GUILayout.Space(10);
			EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), Color.gray);
			GUILayout.Space(10);
		}
	}
}

#endif
