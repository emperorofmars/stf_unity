using System.Collections.Generic;
using com.squirrelbite.stf_unity.tools;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.squirrelbite.stf_unity.modules.editors
{
	public static class STF_Module_Editor_Registry
	{
		public static readonly List<ISTF_Module_Editor> DefaultModules = new()
		{
			new STF_Material_Import_Editor(),
			new STF_Mesh_Import_Editor(),
			new STF_Animation_Import_Editor(),
		};

		private static readonly Dictionary<string, ISTF_Module_Editor> RegisteredModules = new();

		public static void RegisterModule(ISTF_Module_Editor Module)
		{
			if (!RegisteredModules.ContainsKey(Module.STF_Type))
				RegisteredModules.Add(Module.STF_Type, Module);
		}

		public static Dictionary<string, ISTF_Module_Editor> Modules
		{
			get
			{
				var ret = new Dictionary<string, ISTF_Module_Editor>(RegisteredModules);
				foreach(var module in DefaultModules)
				{
					if(!ret.ContainsKey(module.STF_Type))
						ret.Add(module.STF_Type, module);
				}
				return ret;
			}
		}

		public static VisualElement CreateHeroSettingsGUI(STFScriptedImporter Importer)
		{
			var ret = new VisualElement();
			foreach(var module in Modules)
			{
				// todo general module settings
				var moduleOptions = Importer.ImportConfig.ResourceImportOptions.FindAll(o => o.Module == module.Key);
				if(moduleOptions.Count > 0 && module.Value.HasHeroSettings)
				{
					var modulePanel = new Box();
					modulePanel.Add(new Label($"<size=+2><font-weight=700>{(!string.IsNullOrWhiteSpace(module.Value.HeroSettingsLabel) ? module.Value.HeroSettingsLabel : module.Value.STF_Type)}</font-weight></size>"));
					ApplyPanelStyle(modulePanel);
					ret.Add(modulePanel);

					var moduleSettingsPanel = new VisualElement();
					moduleSettingsPanel.style.marginLeft = 10;
					moduleSettingsPanel.style.marginTop = moduleSettingsPanel.style.marginBottom = 3;
					modulePanel.Add(moduleSettingsPanel);

					foreach(var option in Importer.ImportConfig.ResourceImportOptions.FindAll(o => o.Module == module.Key))
					{
						var resourceSettingsPanel = module.Value.CreateHeroSettingsGUI(Importer, option);
						resourceSettingsPanel.style.marginTop = resourceSettingsPanel.style.marginBottom = 3;
						moduleSettingsPanel.Add(resourceSettingsPanel);
					}
				}
			}
			return ret;
		}

		public static VisualElement CreateAdvancedSettingsGUI(STFScriptedImporter Importer)
		{
			var ret = new VisualElement();
			foreach(var module in Modules)
			{
				// todo general module settings
				var moduleOptions = Importer.ImportConfig.ResourceImportOptions.FindAll(o => o.Module == module.Key);
				if(moduleOptions.Count > 0 && module.Value.HasAdvancedSettings)
				{
					var foldout = new Foldout { text = $"<size=+1><font-weight=700>{module.Key}</font-weight></size>", value = false, viewDataKey = $"{module.Key}_advanced_settings" };
					foldout.style.marginTop = foldout.style.marginBottom = 3;
					foldout.style.marginLeft = 10;
					foldout.contentContainer.style.marginLeft = 0;
					ret.Add(foldout);

					var modulePanel = new Box();
					ApplyPanelStyle(modulePanel);
					foldout.Add(modulePanel);

					var moduleSettingsPanel = new ScrollView(ScrollViewMode.Vertical) { horizontalScrollerVisibility = ScrollerVisibility.Hidden };
					moduleSettingsPanel.style.maxHeight = 400;
					moduleSettingsPanel.style.marginTop = moduleSettingsPanel.style.marginBottom = 3;
					modulePanel.Add(moduleSettingsPanel);

					foreach(var option in Importer.ImportConfig.ResourceImportOptions.FindAll(o => o.Module == module.Key))
					{
						moduleSettingsPanel.Add(new Label($"<font-weight=700>{option.DisplayName}</font-weight>"));
						var resourceSettingsPanel = module.Value.CreateAdvancedSettingsGUI(Importer, option);
						resourceSettingsPanel.style.marginLeft = 10;
						resourceSettingsPanel.style.marginTop = resourceSettingsPanel.style.marginBottom = 3;
						moduleSettingsPanel.Add(resourceSettingsPanel);
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
