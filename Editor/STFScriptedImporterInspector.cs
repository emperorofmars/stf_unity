#if UNITY_EDITOR

using System.Linq;
using com.squirrelbite.stf_unity.modules.editors;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.squirrelbite.stf_unity.tools
{
	[CustomEditor(typeof(STFScriptedImporter))]
	public class STFScriptedImporterInspector : ScriptedImporterEditor
	{
		private int SelectedToolbarTab = 0;
		private readonly string[] ToolbarOptions = new string[] {"Info", "Main Settings", "Advanced Settings"};

		public override VisualElement CreateInspectorGUI()
		{
			var importer = (STFScriptedImporter)target;
			VisualElement ui = new();

			{
				var mainSettings = new Box();
				mainSettings.style.marginTop = mainSettings.style.marginBottom = 10;
				mainSettings.style.paddingRight = 5;
				ui.Add(mainSettings);

				var p_AuthoringImport = new PropertyField(serializedObject.FindProperty("ImportConfig").FindPropertyRelative("AuthoringImport"), "<size=+1>Authoring Import</size>");
				mainSettings.Add(p_AuthoringImport);

				var p_SelectedApplication = new DropdownField(STF_Processor_Registry.GetAvailableContextDisplayNames().Select(p => p.Item2).ToList(), 0) { label = "<size=+1>Select Import Context</size>" };
				p_SelectedApplication.BindProperty(serializedObject.FindProperty("ImportConfig").FindPropertyRelative("SelectedApplication"));
				p_SelectedApplication.AddToClassList(BaseField<DropdownField>.alignedFieldUssClassName);
				mainSettings.Add(p_SelectedApplication);
			}

			{
				var spacer = new VisualElement();
				spacer.style.borderBottomWidth = 5;
				spacer.style.borderBottomColor = new StyleColor(new Color(0.17f, 0.17f, 0.17f));
				ui.Add(spacer);
			}

			// Unity plz
			//var tabBar = new TabView();
			var tabBar = new IMGUIContainer { onGUIHandler = DrawTabView };
			tabBar.style.marginTop = tabBar.style.marginBottom = 10;
			ui.Add(tabBar);

			{
				var spacer = new VisualElement();
				spacer.style.borderBottomWidth = 5;
				spacer.style.borderBottomColor = new StyleColor(new Color(0.17f, 0.17f, 0.17f));
				ui.Add(spacer);
			}

			ui.Add(new IMGUIContainer { onGUIHandler = ApplyRevertGUI });


			return ui;
		}

		private void DrawTabView()
		{
			var importer = (STFScriptedImporter)target;
			using(new EditorGUILayout.HorizontalScope())
			{
				GUILayout.FlexibleSpace();
				SelectedToolbarTab = GUILayout.Toolbar(SelectedToolbarTab, ToolbarOptions, GUILayout.Height(25), GUILayout.MaxWidth(450));
				GUILayout.FlexibleSpace();
			}

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
		}

		private void RenderAsset(STF_Import asset)
		{
			if(asset != null)
			{
				using(new EditorGUILayout.HorizontalScope())
				{
					EditorGUILayout.PrefixLabel("Asset Name");
					EditorGUILayout.LabelField(asset.Meta?.STFAssetInfo?.AssetName);
				}
				using(new EditorGUILayout.HorizontalScope())
				{
					EditorGUILayout.PrefixLabel("Asset Version");
					EditorGUILayout.LabelField(asset.Meta?.STFAssetInfo?.Version);
				}
				using(new EditorGUILayout.HorizontalScope())
				{
					EditorGUILayout.PrefixLabel("Author");
					EditorGUILayout.LabelField(asset.Meta?.STFAssetInfo?.Author);
				}
				using(new EditorGUILayout.HorizontalScope())
				{
					EditorGUILayout.PrefixLabel("URL");
					if(!string.IsNullOrWhiteSpace(asset.Meta?.STFAssetInfo?.URL) && asset.Meta.STFAssetInfo.URL.StartsWith("https://"))
					{
						if(EditorGUILayout.LinkButton(asset.Meta?.STFAssetInfo?.URL))
							Application.OpenURL(asset.Meta?.STFAssetInfo?.URL);
					}
					else
						EditorGUILayout.LabelField(asset.Meta?.STFAssetInfo?.URL);
				}
				using(new EditorGUILayout.HorizontalScope())
				{
					EditorGUILayout.PrefixLabel("License");
					EditorGUILayout.LabelField(asset.Meta?.STFAssetInfo?.License);
				}
				using(new EditorGUILayout.HorizontalScope())
				{
					EditorGUILayout.PrefixLabel("License URL");
					if(!string.IsNullOrWhiteSpace(asset.Meta?.STFAssetInfo?.LicenseURL) && asset.Meta.STFAssetInfo.LicenseURL.StartsWith("https://"))
					{
						if(EditorGUILayout.LinkButton(asset.Meta?.STFAssetInfo?.LicenseURL))
							Application.OpenURL(asset.Meta?.STFAssetInfo?.LicenseURL);
					}
					else
						EditorGUILayout.LabelField(asset.Meta?.STFAssetInfo?.LicenseURL);
				}
				using(new EditorGUILayout.HorizontalScope())
				{
					EditorGUILayout.PrefixLabel("Documentation URL");
					if(!string.IsNullOrWhiteSpace(asset.Meta?.STFAssetInfo?.DocumentationURL) && asset.Meta.STFAssetInfo.DocumentationURL.StartsWith("https://"))
					{
						if(EditorGUILayout.LinkButton(asset.Meta?.STFAssetInfo?.DocumentationURL))
							Application.OpenURL(asset.Meta?.STFAssetInfo?.DocumentationURL);
					}
					else
						EditorGUILayout.LabelField(asset.Meta?.STFAssetInfo?.DocumentationURL);
				}
				using(new EditorGUILayout.HorizontalScope())
				{
					EditorGUILayout.PrefixLabel("Binary Version");
					EditorGUILayout.LabelField($"{asset.BinaryVersionMajor}.{asset.BinaryVersionMinor}");
				}
				using(new EditorGUILayout.HorizontalScope())
				{
					EditorGUILayout.PrefixLabel("Definition Version");
					EditorGUILayout.LabelField($"{asset.Meta?.DefinitionVersionMajor}.{asset.Meta?.DefinitionVersionMinor}");
				}

				if(asset.Meta?.STFAssetInfo?.CustomProperties?.Count > 0)
				{
					GUILayout.Space(10);
					EditorGUILayout.LabelField("Custom Properties", EditorStyles.boldLabel);
					using(new EditorGUI.IndentLevelScope())
					{
						foreach(var custom in asset.Meta?.STFAssetInfo?.CustomProperties)
						{
							using(new EditorGUILayout.HorizontalScope())
							{
								EditorGUILayout.SelectableLabel(custom.Name);
								EditorGUILayout.SelectableLabel(custom.Value);
							}
						}
					}
				}
			}
			else
			{
				EditorGUILayout.LabelField("Invalid Asset");
			}
		}
	}
}

#endif
