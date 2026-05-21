#if UNITY_EDITOR

using com.squirrelbite.stf_unity.resources.stfexp;
using com.squirrelbite.stf_unity.tools;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace com.squirrelbite.stf_unity.resources.editors
{
	public class STFEXP_Node_Ethereal_Import_Editor : ISTF_Module_Editor
	{
		public string STF_Type => STFEXP_Node_Ethereal._STF_Type;
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

			if(options.ContainsKey("preserve") && options.Value<bool>("preserve") is bool preserveEthereal)
			{
				var togglePreserveEthereal = new Toggle("Preserve ethereal node") { value = preserveEthereal };
				togglePreserveEthereal.RegisterValueChangedCallback(e => {
					var options = JObject.Parse(Option.Json);
					options["preserve"] = e.newValue;
					Option.Json = options.ToString();
					EditorUtility.SetDirty(Importer);
				});
				ret.Add(togglePreserveEthereal);
			}

			return ret;
		}
	}
}

#endif
