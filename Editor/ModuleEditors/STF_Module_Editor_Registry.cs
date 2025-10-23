using System.Collections.Generic;
using com.squirrelbite.stf_unity.tools;
using UnityEditor;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules.editors
{
	public static class STF_Module_Editor_Registry
	{
		public static readonly List<ISTF_Module_Editor> DefaultModules = new()
		{
			new STF_Mesh_Import_Editor(),
			new STF_Material_Import_Editor(),
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

		public static void DrawSettings(STFScriptedImporter Importer)
		{
			EditorGUILayout.LabelField("Module Settings", EditorStyles.largeLabel);
			foreach(var module in Modules)
			{
				GUILayout.Space(10);
				GUILayout.Label(module.Key, EditorStyles.whiteLargeLabel);
				EditorGUI.indentLevel++;
				// todo module settings
				foreach(var option in Importer.ImportConfig.ResourceImportOptions.FindAll(o => o.Module == module.Key))
				{
					try
					{
						GUILayout.Space(5);
						module.Value.Draw(Importer, option);
					}
					catch
					{
						// todo
						Debug.Log("FAIL");
					}
				}
				EditorGUI.indentLevel--;
			}
		}
	}
}
