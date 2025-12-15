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

				string formatValue(string key) { return STF_Processor_Registry.GetAvailableContextDisplayNames().Find(e => e.Item1 == key).Item2; }
				var p_SelectedApplication = new DropdownField(STF_Processor_Registry.GetAvailableContextDisplayNames().Select(p => p.Item1).ToList(), 0, formatValue, formatValue) { label = "<size=+1>Select Import Context</size>" };
				p_SelectedApplication.BindProperty(serializedObject.FindProperty("ImportConfig").FindPropertyRelative("SelectedApplication"));
				p_SelectedApplication.AddToClassList(BaseField<DropdownField>.alignedFieldUssClassName);
				mainSettings.Add(p_SelectedApplication);

				var p_AuthoringImport = new PropertyField(serializedObject.FindProperty("ImportConfig").FindPropertyRelative("AuthoringImport"), "<size=+1>Authoring Import</size>");
				p_AuthoringImport.style.marginTop = 3;
				mainSettings.Add(p_AuthoringImport);
			}

			{
				// Unity plz
				//var tabBar = new TabView();

				var tabBar = new VisualElement();
				ui.Add(tabBar);
				var tabContentContainer = new VisualElement();
				ui.Add(tabContentContainer);

				tabBar.style.flexDirection = FlexDirection.Row;
				tabBar.style.marginTop = 6;
				tabBar.style.marginBottom = 10;
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
						tab.style.borderLeftColor = tab.style.borderRightColor = new StyleColor(UIConstants.ColorBorder);
					}
					else if(roundSide == 1)
						tab.style.borderTopRightRadius = tab.style.borderBottomRightRadius = 3;
					tab.style.backgroundColor = new StyleColor(UIConstants.ColorBackground);

					tab.RegisterCallback<MouseOverEvent>(e => {
						if(tab != currentTab)
							tab.style.backgroundColor = new StyleColor(UIConstants.ColorMouseOver);
					});
					tab.RegisterCallback<MouseOutEvent>(e => {
						if(tab != currentTab)
							tab.style.backgroundColor = new StyleColor(UIConstants.ColorBackground);
					});
					tab.RegisterCallback<PointerUpEvent>(e => {
						currentTab = e.currentTarget as Label;
						foreach(var tab in tabs)
						{
							if(tab != currentTab)
							{
								tab.style.backgroundColor = new StyleColor(UIConstants.ColorBackground);
								tabContent[tab].style.display = DisplayStyle.None;
							}
						}
						currentTab.style.backgroundColor = new StyleColor(UIConstants.ColorActive);
						if(tabContent.ContainsKey(tab))
							tabContent[tab].style.display = DisplayStyle.Flex;
					});
					return tab;
				}
				currentTab = setupLabel("Info", CreateAssetInfoGUI(), -1);
				currentTab.style.backgroundColor = new StyleColor(UIConstants.ColorActive);

				var mainTab = STF_Module_Editor_Registry.CreateHeroSettingsGUI(importer);
				mainTab.style.display = DisplayStyle.None;
				setupLabel("Main Settings", mainTab);

				var advancedTab = STF_Module_Editor_Registry.CreateAdvancedSettingsGUI(importer);
				setupLabel("Advanced", advancedTab, 1);
				advancedTab.style.display = DisplayStyle.None;
			}

			{
				var spacer = new VisualElement();
				spacer.style.marginTop = 15;
				spacer.style.borderBottomWidth = 5;
				spacer.style.borderBottomColor = new StyleColor(UIConstants.ColorBackground);
				ui.Add(spacer);
			}

			ui.Add(new IMGUIContainer { onGUIHandler = ApplyRevertGUI });

			return ui;
		}

		private VisualElement CreateAssetInfoGUI()
		{
			var importer = (STFScriptedImporter)target;
			var ret = new VisualElement();
			if (AssetDatabase.LoadAssetAtPath<STF_Import>(importer.assetPath) is var asset && asset != null)
			{
				ret.Add(CreateEditor(asset).CreateInspectorGUI());
			}
			else
			{
				ret.Add(new HelpBox("Import Failed :(", HelpBoxMessageType.Error));
			}
			return ret;
		}
	}
}

#endif
