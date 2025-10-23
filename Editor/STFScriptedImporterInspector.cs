#if UNITY_EDITOR

using System.Linq;
using com.squirrelbite.stf_unity.modules.editors;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace com.squirrelbite.stf_unity.tools
{
	[CustomEditor(typeof(STFScriptedImporter))]
	public class STFScriptedImporterInspector : ScriptedImporterEditor
	{
		private int SelectedToolbarTab = 0;
		private readonly string[] ToolbarOptions = new string[] {"Info", "Main Settings", "Advanced Settings"};

		public override void OnInspectorGUI()
		{
			var importer = (STFScriptedImporter)target;

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Authoring Import");
			var authoringImport = EditorGUILayout.Toggle(importer.ImportConfig.AuthoringImport);
			EditorGUILayout.EndHorizontal();
			if(authoringImport != importer.ImportConfig.AuthoringImport)
			{
				importer.ImportConfig.AuthoringImport = authoringImport;
				EditorUtility.SetDirty(importer);
			}

			var availableContexts = STF_Processor_Registry.GetAvailableContextDisplayNames();

			int selectedIndex = availableContexts.FindIndex(c => c.Item1 == importer.ImportConfig.SelectedApplication);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Select Import Context");
			var newSelectedIndex = EditorGUILayout.Popup(selectedIndex, availableContexts.Select(p => p.Item2).ToArray());
			EditorGUILayout.EndHorizontal();

			if (newSelectedIndex != selectedIndex && newSelectedIndex >= 0 && newSelectedIndex < availableContexts.Count)
			{
				importer.ImportConfig.SelectedApplication = availableContexts[newSelectedIndex].Item1;
				EditorUtility.SetDirty(importer);
			}

			GUILayout.Space(10);
			EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), Color.gray);
			GUILayout.Space(5);

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			SelectedToolbarTab = GUILayout.Toolbar(SelectedToolbarTab, ToolbarOptions, GUILayout.Height(25), GUILayout.MaxWidth(450));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			if(SelectedToolbarTab == 0)
			{
				GUILayout.Space(10);
				if (AssetDatabase.LoadAssetAtPath<STF_Import>(importer.assetPath) is var stfImport && stfImport != null)
				{
					RenderAsset(stfImport);
				}
				else
				{
					EditorGUILayout.LabelField("Import Failed");
				}
			}
			else if(SelectedToolbarTab == 1)
			{
				STF_Module_Editor_Registry.DrawHeroSettings(importer);
			}
			else if(SelectedToolbarTab == 2)
			{
				STF_Module_Editor_Registry.DrawAdvancedSettings(importer);
			}

			GUILayout.Space(10);
			EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), Color.gray);

			ApplyRevertGUI();
		}

		private void RenderAsset(STF_Import asset)
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
				if(!string.IsNullOrWhiteSpace(asset.Meta?.STFAssetInfo?.URL) && asset.Meta.STFAssetInfo.URL.StartsWith("https://"))
				{
					if(EditorGUILayout.LinkButton(asset.Meta?.STFAssetInfo?.URL))
						Application.OpenURL(asset.Meta?.STFAssetInfo?.URL);
				}
				else
					EditorGUILayout.LabelField(asset.Meta?.STFAssetInfo?.URL);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("License");
				EditorGUILayout.LabelField(asset.Meta?.STFAssetInfo?.License);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("License URL");
				if(!string.IsNullOrWhiteSpace(asset.Meta?.STFAssetInfo?.LicenseURL) && asset.Meta.STFAssetInfo.LicenseURL.StartsWith("https://"))
				{
					if(EditorGUILayout.LinkButton(asset.Meta?.STFAssetInfo?.LicenseURL))
						Application.OpenURL(asset.Meta?.STFAssetInfo?.LicenseURL);
				}
				else
					EditorGUILayout.LabelField(asset.Meta?.STFAssetInfo?.LicenseURL);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Documentation URL");
				if(!string.IsNullOrWhiteSpace(asset.Meta?.STFAssetInfo?.DocumentationURL) && asset.Meta.STFAssetInfo.DocumentationURL.StartsWith("https://"))
				{
					if(EditorGUILayout.LinkButton(asset.Meta?.STFAssetInfo?.DocumentationURL))
						Application.OpenURL(asset.Meta?.STFAssetInfo?.DocumentationURL);
				}
				else
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
	}
}

#endif
