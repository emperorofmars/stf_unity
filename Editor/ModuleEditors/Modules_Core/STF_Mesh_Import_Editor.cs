#if UNITY_EDITOR

using com.squirrelbite.stf_unity.tools;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace com.squirrelbite.stf_unity.modules.editors
{
	public class STF_Mesh_Import_Editor : ISTF_Module_Editor
	{
		public string STF_Type => STF_Mesh.STF_TYPE;
		public string HeroSettingsLabel => null;
		public bool HasHeroSettings => false;
		public bool HasAdvancedSettings => true;

		public VisualElement CreateHeroSettingsGUI(STFScriptedImporter Importer, ImportOptions.ResourceImportOption Option)
		{
			return null;
		}

		public VisualElement CreateAdvancedSettingsGUI(STFScriptedImporter Importer, ImportOptions.ResourceImportOption Option)
		{
			var ret = new VisualElement();
			var options = JObject.Parse(Option.Json);

			if(options.ContainsKey("vertex_colors") && options.Value<bool>("vertex_colors") is bool vertexColors)
			{
				var toggleVertexColors = new Toggle("Import Vertex Colors") { value = vertexColors };
				toggleVertexColors.RegisterValueChangedCallback(e => {
					var options = JObject.Parse(Option.Json);
					options["vertex_colors"] = e.newValue;
					Option.Json = options.ToString();
					EditorUtility.SetDirty(Importer);
				});
				ret.Add(toggleVertexColors);
			}

			if(options.ContainsKey("max_weights") && options.Value<int>("max_weights") is int maxWeights)
			{
				var hbar = new VisualElement();
				hbar.style.flexDirection = FlexDirection.Row;
				var valueLabel = new Label(maxWeights.ToString());
				valueLabel.style.marginLeft = 10;
				var sliderMaxWeights = new SliderInt("Max. Weights", 1, 32) { value = maxWeights };
				sliderMaxWeights.style.flexGrow = 1;
				sliderMaxWeights.RegisterValueChangedCallback(e => {
					var options = JObject.Parse(Option.Json);
					options["max_weights"] = e.newValue;
					Option.Json = options.ToString();
					EditorUtility.SetDirty(Importer);
					valueLabel.text = e.newValue.ToString();
				});
				hbar.Add(sliderMaxWeights);
				hbar.Add(valueLabel);
				ret.Add(hbar);
			}

			return ret;
		}
	}
}

#endif
