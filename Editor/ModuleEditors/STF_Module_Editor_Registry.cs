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
					modulePanel.Add(new Label($"<size=+1><font-weight=700>{(!string.IsNullOrWhiteSpace(module.Value.HeroSettingsLabel) ? module.Value.HeroSettingsLabel : module.Value.STF_Type)}</font-weight></size>"));
					ApplyPanelStyle(modulePanel);
					ret.Add(modulePanel);

					var moduleSettingsPanel = new VisualElement();
					moduleSettingsPanel.style.marginLeft = 10;
					modulePanel.Add(moduleSettingsPanel);

					foreach(var option in Importer.ImportConfig.ResourceImportOptions.FindAll(o => o.Module == module.Key))
					{
						moduleSettingsPanel.Add(module.Value.CreateHeroSettingsGUI(Importer, option));
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
					var modulePanel = new Box();
					modulePanel.Add(new Label($"<size=+1><font-weight=700>{module.Key}</font-weight></size>"));
					ApplyPanelStyle(modulePanel);
					ret.Add(modulePanel);

					var moduleSettingsPanel = new VisualElement();
					moduleSettingsPanel.style.marginLeft = 10;
					modulePanel.Add(moduleSettingsPanel);

					foreach(var option in Importer.ImportConfig.ResourceImportOptions.FindAll(o => o.Module == module.Key))
					{
						moduleSettingsPanel.Add(module.Value.CreateAdvancedSettingsGUI(Importer, option));
					}
				}
			}
			return ret;
		}

		private static void ApplyPanelStyle(VisualElement Panel)
		{
			Panel.style.paddingTop = Panel.style.paddingLeft = Panel.style.paddingBottom = Panel.style.paddingRight = 6;
			Panel.style.borderTopLeftRadius = Panel.style.borderBottomLeftRadius = Panel.style.borderTopRightRadius = Panel.style.borderBottomRightRadius = 3;
		}
	}
}
