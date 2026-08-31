#if UNITY_EDITOR

using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.tools;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.squirrelbite.stf_unity.tools
{
	public static class STF_ImportSettingsEditor
	{
		public static VisualElement CreateHeroSettingsGUI(STFScriptedImporter Importer)
		{
			var handleChange = new System.Action(() => { EditorUtility.SetDirty(Importer); });
			var ret = new VisualElement();

			// Application context settings
			if(STF_Processor_Registry.GetApplicationContextDefinition(Importer.ImportConfig.SelectedApplication) is var context && context != null)
			{
				var contextSettingsGUI = context.CreateSettingsGUI(Importer.ImportConfig, handleChange);
				if(contextSettingsGUI != null)
				{
					var ContextPanel = new Box();
					ContextPanel.Add(new Label($"<size=+2><font-weight=700>{context.DisplayName} Context Settings</font-weight></size>"));
					ApplyPanelStyle(ContextPanel);

					var ContextSettingsPanel = new VisualElement();
					ContextSettingsPanel.style.marginLeft = 10;
					ContextSettingsPanel.style.marginTop = ContextSettingsPanel.style.marginBottom = 3;
					ContextPanel.Add(ContextSettingsPanel);

					contextSettingsGUI.style.marginTop = contextSettingsGUI.style.marginBottom = 3;
					ContextSettingsPanel.Add(contextSettingsGUI);

					ret.Add(ContextPanel);
				}

				// Processor settings for each processor in the context
			}

			var resourceEditors = new List<ISTF_ProcessorBase>();
			foreach(var p in STF_Processor_Registry.GetProcessors(Importer.ImportConfig.SelectedApplication))
				if(!string.IsNullOrWhiteSpace(p.Value.SettingsKey) && p.Value.HasHeroSettings)
					resourceEditors.Add(p.Value);
			foreach(var p in STF_Processor_Registry.GetGlobalProcessors(Importer.ImportConfig.SelectedApplication))
				if(!string.IsNullOrWhiteSpace(p.Value.SettingsKey) && p.Value.HasHeroSettings)
					resourceEditors.Add(p.Value);

			foreach(var r in resourceEditors)
				Debug.Log(r);

			// Handler settings
			foreach(var editor in resourceEditors)
			{
				// todo general handler settings
				var resourceOptions = Importer.ImportConfig.ResourceImportOptions.FindAll(o => o.STF_Type == editor.SettingsKey);
				if(resourceOptions.Count > 0 && editor.HasHeroSettings)
				{
					var modulePanel = new Box();
					modulePanel.Add(new Label($"<size=+2><font-weight=700>{(!string.IsNullOrWhiteSpace(editor.HeroSettingsLabel) ? editor.HeroSettingsLabel : editor.SettingsKey)}</font-weight></size>"));
					ApplyPanelStyle(modulePanel);
					ret.Add(modulePanel);

					var moduleSettingsPanel = new VisualElement();
					moduleSettingsPanel.style.marginLeft = 10;
					moduleSettingsPanel.style.marginTop = moduleSettingsPanel.style.marginBottom = 3;
					modulePanel.Add(moduleSettingsPanel);

					foreach(var option in Importer.ImportConfig.ResourceImportOptions.FindAll(o => o.STF_Type == editor.SettingsKey))
					{
						var resourceSettingsPanel = editor.CreateHeroSettingsGUI(option, handleChange);
						resourceSettingsPanel.style.marginTop = resourceSettingsPanel.style.marginBottom = 3;
						moduleSettingsPanel.Add(resourceSettingsPanel);
					}
				}
			}
			return ret;
		}

		public static VisualElement CreateAdvancedSettingsGUI(STFScriptedImporter Importer)
		{
			// Processor settings for each processor in the context
			var handleChange = new System.Action(() => { EditorUtility.SetDirty(Importer); });

			var ret = new VisualElement();

			var resourceEditors = new List<ISTF_ProcessorBase>();
			foreach(var p in STF_Processor_Registry.GetProcessors(Importer.ImportConfig.SelectedApplication))
				if(!string.IsNullOrWhiteSpace(p.Value.SettingsKey) && p.Value.HasAdvancedSettings)
					resourceEditors.Add(p.Value);
			foreach(var p in STF_Processor_Registry.GetGlobalProcessors(Importer.ImportConfig.SelectedApplication))
				if(!string.IsNullOrWhiteSpace(p.Value.SettingsKey) && p.Value.HasAdvancedSettings)
					resourceEditors.Add(p.Value);

			foreach(var editor in resourceEditors)
			{
				var resourceOptions = Importer.ImportConfig.ResourceImportOptions.FindAll(o => o.STF_Type == editor.SettingsKey);
				if(resourceOptions.Count > 0 && editor.HasAdvancedSettings)
				{
					var foldout = new Foldout { text = $"<size=+1><font-weight=700>{editor.SettingsKey}</font-weight></size>", value = false, viewDataKey = $"{editor.SettingsKey}_advanced_settings" };
					foldout.style.marginTop = foldout.style.marginBottom = 3;
					foldout.style.marginLeft = 10;
					foldout.contentContainer.style.marginLeft = 0;
					ret.Add(foldout);

					var resourcePanel = new Box();
					ApplyPanelStyle(resourcePanel);
					foldout.Add(resourcePanel);

					var resourceSettingsPanel = new ScrollView(ScrollViewMode.Vertical) { horizontalScrollerVisibility = ScrollerVisibility.Hidden };
					resourceSettingsPanel.style.maxHeight = 400;
					resourceSettingsPanel.style.marginTop = resourceSettingsPanel.style.marginBottom = 3;
					resourcePanel.Add(resourceSettingsPanel);

					foreach(var option in Importer.ImportConfig.ResourceImportOptions.FindAll(o => o.STF_Type == editor.SettingsKey))
					{
						resourceSettingsPanel.Add(new Label($"<font-weight=700>{option.DisplayName}</font-weight>"));
						var resourceSettingsGUI = editor.CreateAdvancedSettingsGUI(option, handleChange);
						resourceSettingsGUI.style.marginLeft = 10;
						resourceSettingsGUI.style.marginTop = resourceSettingsGUI.style.marginBottom = 3;
						resourceSettingsPanel.Add(resourceSettingsGUI);
					}
				}
			}
			return ret;
		}

		private static void ApplyPanelStyle(VisualElement Panel)
		{
			Panel.style.marginTop = Panel.style.marginBottom = 2;
			Panel.style.paddingTop = Panel.style.paddingLeft = Panel.style.paddingBottom = Panel.style.paddingRight = 6;
			Panel.style.borderTopLeftRadius = Panel.style.borderBottomLeftRadius = Panel.style.borderTopRightRadius = Panel.style.borderBottomRightRadius = 3;
		}
	}
}

#endif
