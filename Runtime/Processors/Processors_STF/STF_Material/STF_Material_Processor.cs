using System.Collections.Generic;
using System.Linq;
using com.squirrelbite.stf_unity.processors.stf_material;
using com.squirrelbite.stf_unity.resources;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.squirrelbite.stf_unity.processors
{
	public class STF_Material_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STF_Material);
		public uint Order => 10;
		public int Priority => 1;

		public (List<Object>, List<Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var STFMaterial = STFResource as STF_Material;

			// todo handle shader targets & style hints

			var materialMapping = Context.ImportConfig.GetAndConfirmImportOption(STF_Material.STF_TYPE, STFMaterial.STF_Id, STFMaterial.STF_Name, "target_shader", STF_Material_Converter_Registry.DefaultShader);

			var (ConvertedMaterial, GeneratedObjects) = STF_Material_Converter_Registry.Converters[materialMapping].ConvertToUnityMaterial(STFMaterial);

			var ret = new List<Object>() { ConvertedMaterial };
			if (GeneratedObjects != null) ret.AddRange(GeneratedObjects);
			return (ret, ret);
		}

		public string SettingsKey => STF_Material.STF_TYPE;
		public string HeroSettingsLabel => "Material Selection";
		public bool HasHeroSettings => true;

		public VisualElement CreateHeroSettingsGUI(ImportOptions.ResourceImportOption Option, System.Action EmitChange)
		{
			var availableConverters = STF_Material_Converter_Registry.Converters.Select(c => c.Key).ToList();
			var options = JObject.Parse(Option.Json);
			if(options.ContainsKey("target_shader") && options.Value<string>("target_shader") is string targetShader && !string.IsNullOrWhiteSpace(targetShader))
			{
				int selectedIndex = availableConverters.FindIndex(c => c == targetShader);
				if (selectedIndex < 0)
					selectedIndex = 0; // Default Shader
				var ret = new PopupField<string>(availableConverters, selectedIndex) { label = Option.DisplayName };
				ret.RegisterValueChangedCallback(e => {
					var options = JObject.Parse(Option.Json);
					options["target_shader"] = e.newValue;
					Option.Json = options.ToString();
					EmitChange();
				});
				return ret;
			}
			else return new VisualElement();
		}

		public VisualElement CreateAdvancedSettingsGUI(ImportOptions.ResourceImportOption Option, System.Action EmitChange)
		{
			return null;
		}
	}
}
