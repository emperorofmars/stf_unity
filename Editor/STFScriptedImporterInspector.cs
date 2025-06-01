#if UNITY_EDITOR

using System.Linq;
using com.squirrelbite.stf_unity.modules.stf_material;
using UnityEditor;
using UnityEngine;

namespace com.squirrelbite.stf_unity.tools
{
	[CustomEditor(typeof(STFScriptedImporter))]
	public class STFScriptedImporterInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			var importer = (STFScriptedImporter)target;

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Authoring Import");
			var authoringImport = EditorGUILayout.Toggle(importer.ImportConfig.AuthoringImport);
			EditorGUILayout.EndHorizontal();
			if(authoringImport != importer.ImportConfig.AuthoringImport)
			{
				importer.ImportConfig.AuthoringImport = authoringImport;
				EditorUtility.SetDirty(importer);
			}

			var availableContexts = STF_Processor_Registry.GetAvaliableContextDisplayNames();

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

			drawImportConfig(importer);

			drawHLine();
			
			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(importer);
			}

			if (EditorUtility.IsDirty(importer))
			{
				if (GUILayout.Button("Reimport to apply changes!"))
				{
					GUILayout.Space(10);
					importer.SaveAndReimport();
				}
			}
			else
			{
				if (GUILayout.Button("Reimport"))
				{
					EditorUtility.SetDirty(importer);
					importer.SaveAndReimport();
				}
			}

			drawHLine();

			if (AssetDatabase.LoadAssetAtPath<STF_Import>(importer.assetPath) is var stfImport)
			{
				renderAsset(stfImport);
			}
			else
			{
				EditorGUILayout.LabelField("Import Failed");
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

		private void drawImportConfig(STFScriptedImporter Importer)
		{
			var availableConverters = STF_Material_Converter_Registry.Converters.Select(c => c.Key).ToList();

			if (Importer.ImportConfig.MaterialMappings.Count > 0)
			{
				drawHLine();

				EditorGUILayout.LabelField("Material Mappings");
				foreach (var mapping in Importer.ImportConfig.MaterialMappings)
				{
					EditorGUI.indentLevel++;

					//EditorGUILayout.LabelField(mapping.MaterialName + " (" + mapping.ID + ")");

					//EditorGUI.indentLevel++;

					int selectedIndex = availableConverters.FindIndex(c => c == mapping.TargetShader);
					if (selectedIndex < 0)
					{
						selectedIndex = 0; // Standard Shader

						/*EditorGUILayout.BeginHorizontal();
						EditorGUILayout.PrefixLabel("Unsupported Target Shader");
						EditorGUILayout.LabelField(mapping.TargetShader);
						EditorGUILayout.EndHorizontal();*/
					}

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel(mapping.MaterialName);
					var newSelectedIndex = EditorGUILayout.Popup(selectedIndex, availableConverters.ToArray());
					EditorGUILayout.EndHorizontal();

					if(newSelectedIndex != selectedIndex) mapping.TargetShader = availableConverters[newSelectedIndex];

					//EditorGUI.indentLevel--;
					EditorGUI.indentLevel--;
				}
			}
		}

		private void drawHLine()
		{
			GUILayout.Space(10);
			EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), Color.gray);
			GUILayout.Space(10);
		}
	}
}

#endif
