#if UNITY_EDITOR

using System.Collections.Generic;
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
				mainSettings.style.marginTop = mainSettings.style.marginBottom = 8;
				mainSettings.style.paddingTop = mainSettings.style.paddingRight = mainSettings.style.paddingBottom = mainSettings.style.paddingLeft = 5;
				mainSettings.style.paddingRight = 10;
				mainSettings.style.borderTopLeftRadius = mainSettings.style.borderBottomLeftRadius = mainSettings.style.borderTopRightRadius = mainSettings.style.borderBottomRightRadius = 3;
				ui.Add(mainSettings);

				var p_AuthoringImport = new PropertyField(serializedObject.FindProperty("ImportConfig").FindPropertyRelative("AuthoringImport"), "<size=+1>Authoring Import</size>");
				mainSettings.Add(p_AuthoringImport);
				p_AuthoringImport.style.marginBottom = 3;

				string formatValue(string key) { return STF_Processor_Registry.GetAvailableContextDisplayNames().Find(e => e.Item1 == key).Item2; }
				var p_SelectedApplication = new DropdownField(STF_Processor_Registry.GetAvailableContextDisplayNames().Select(p => p.Item1).ToList(), 0, formatValue, formatValue) { label = "<size=+1>Select Import Context</size>" };
				p_SelectedApplication.BindProperty(serializedObject.FindProperty("ImportConfig").FindPropertyRelative("SelectedApplication"));
				p_SelectedApplication.AddToClassList(BaseField<DropdownField>.alignedFieldUssClassName);
				mainSettings.Add(p_SelectedApplication);
			}

			{
				// Unity plz
				//var tabBar = new TabView();

				var tabBar = new VisualElement();
				ui.Add(tabBar);
				var tabContentContainer = new VisualElement();
				ui.Add(tabContentContainer);

				tabBar.style.flexDirection = FlexDirection.Row;
				tabBar.style.marginTop = tabBar.style.marginBottom = 6;
				//tabBar.style.paddingTop = tabBar.style.paddingRight = tabBar.style.paddingBottom = tabBar.style.paddingLeft = 3;

				var tabs = new List<VisualElement>();
				var tabContent = new Dictionary<VisualElement, VisualElement>();
				Label currentTab = null;

				Label setupLabel(string text, VisualElement content, int roundSide = 0)
				{
					var tab = new Label(text);
					tabBar.Add(tab);
					tabs.Add(tab);
					tabContentContainer.Add(content);
					tabContent.Add(tab, content);
					tab.style.minWidth = 100;
					tab.style.flexGrow = 1;
					tab.style.unityTextAlign = TextAnchor.MiddleCenter;
					tab.style.paddingTop = tab.style.paddingRight = tab.style.paddingBottom = tab.style.paddingLeft = 6;
					if(roundSide == -1)
						tab.style.borderTopLeftRadius = tab.style.borderBottomLeftRadius = 3;
					else if(roundSide == 0)
					{
						tab.style.borderLeftWidth = tab.style.borderRightWidth = 1;
						tab.style.borderLeftColor = tab.style.borderRightColor = new StyleColor(new Color(0.12f, 0.12f, 0.12f));
					}
					else if(roundSide == 1)
						tab.style.borderTopRightRadius = tab.style.borderBottomRightRadius = 3;
					tab.style.backgroundColor = new StyleColor(new Color(0.17f, 0.17f, 0.17f));

					tab.RegisterCallback<MouseOverEvent>(e => {
						if(tab != currentTab)
							tab.style.backgroundColor = new StyleColor(new Color(0.19f, 0.19f, 0.19f));
					});
					tab.RegisterCallback<MouseOutEvent>(e => {
						if(tab != currentTab)
							tab.style.backgroundColor = new StyleColor(new Color(0.17f, 0.17f, 0.17f));
					});
					tab.RegisterCallback<PointerUpEvent>(e => {
						currentTab = e.currentTarget as Label;
						foreach(var tab in tabs)
						{
							if(tab != currentTab)
							{
								tab.style.backgroundColor = new StyleColor(new Color(0.17f, 0.17f, 0.17f));
								tab.style.borderBottomWidth = 0;
								tabContent[tab].style.display = DisplayStyle.None;
							}
						}
						currentTab.style.backgroundColor = new StyleColor(new Color(0.28f, 0.38f, 0.52f));
						if(tabContent.ContainsKey(tab))
							tabContent[tab].style.display = DisplayStyle.Flex;
					});
					return tab;
				}
				currentTab = setupLabel("Info", new IMGUIContainer { onGUIHandler = RenderAssetInfoGUI }, -1);
				currentTab.style.backgroundColor = new StyleColor(new Color(0.28f, 0.38f, 0.52f));

				var mainTab = new IMGUIContainer { onGUIHandler = RenderMainSettingsGUI };
				mainTab.style.display = DisplayStyle.None;
				setupLabel("Main Settings", mainTab);

				var advancedTab = new IMGUIContainer { onGUIHandler = RenderAdvancedSettingsGUI };
				setupLabel("Advanced", advancedTab, 1);
				advancedTab.style.display = DisplayStyle.None;
			}

			{
				var spacer = new VisualElement();
				spacer.style.marginTop = 15;
				spacer.style.borderBottomWidth = 5;
				spacer.style.borderBottomColor = new StyleColor(new Color(0.17f, 0.17f, 0.17f));
				ui.Add(spacer);
			}

			ui.Add(new IMGUIContainer { onGUIHandler = ApplyRevertGUI });


			return ui;
		}

		private void RenderMainSettingsGUI()
		{
			var importer = (STFScriptedImporter)target;
			STF_Module_Editor_Registry.DrawHeroSettings(importer);
		}
		private void RenderAdvancedSettingsGUI()
		{
			var importer = (STFScriptedImporter)target;
			STF_Module_Editor_Registry.DrawAdvancedSettings(importer);
		}

		private void RenderAssetInfoGUI()
		{
			var importer = (STFScriptedImporter)target;
			GUILayout.Space(10);
			if (AssetDatabase.LoadAssetAtPath<STF_Import>(importer.assetPath) is var asset && asset != null)
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
				EditorGUILayout.LabelField("Import Failed");
			}
		}
	}
}

#endif
