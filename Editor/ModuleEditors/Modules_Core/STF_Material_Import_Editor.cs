#if UNITY_EDITOR

using System.Linq;
using com.squirrelbite.stf_unity.modules.stf_material;
using com.squirrelbite.stf_unity.tools;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace com.squirrelbite.stf_unity.modules.editors
{
	public class STF_Material_Import_Editor : ISTF_Module_Editor
	{
		public string STF_Type => STF_Material.STF_TYPE;
		public string HeroSettingsLabel => "Material Selection";
		public bool HasHeroSettings => true;
		public bool HasAdvancedSettings => false;

		public VisualElement CreateHeroSettingsGUI(STFScriptedImporter Importer, ImportOptions.ResourceImportOption Option)
		{
			var availableConverters = STF_Material_Converter_Registry.Converters.Select(c => c.Key).ToList();
			var options = JObject.Parse(Option.Json);
			if(options.ContainsKey("target_shader") && options.Value<string>("target_shader") is string targetShader && !string.IsNullOrWhiteSpace(targetShader))
			{
				int selectedIndex = availableConverters.FindIndex(c => c == targetShader);
				if (selectedIndex < 0)
					selectedIndex = 0; // Default Shader
				var ret = new PopupField<string>(availableConverters, selectedIndex);
				ret.RegisterValueChangedCallback(e => {
					var options = JObject.Parse(Option.Json);
					options["target_shader"] = e.newValue;
					Option.Json = options.ToString();
					EditorUtility.SetDirty(Importer);
				});
				return ret;
			}
			else return new VisualElement();
		}

		public VisualElement CreateAdvancedSettingsGUI(STFScriptedImporter Importer, ImportOptions.ResourceImportOption Option)
		{
			return null;
		}
	}
}

#endif
