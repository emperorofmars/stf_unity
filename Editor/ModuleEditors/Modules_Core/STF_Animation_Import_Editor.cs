#if UNITY_EDITOR

using com.squirrelbite.stf_unity.tools;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace com.squirrelbite.stf_unity.modules.editors
{
	public class STF_Animation_Import_Editor : ISTF_Module_Editor
	{
		public string STF_Type => STF_Animation.STF_TYPE;
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

			{
				if(options.ContainsKey("prefer_baked") && options.Value<bool>("prefer_baked") is bool value)
				{
					var toggle = new Toggle("Prefer Baked Keyframes") { value = value };
					toggle.RegisterValueChangedCallback(e => {
						var options = JObject.Parse(Option.Json);
						options["prefer_baked"] = e.newValue;
						Option.Json = options.ToString();
						EditorUtility.SetDirty(Importer);
					});
					ret.Add(toggle);
				}
			}
			{
				if(options.ContainsKey("import_baked") && options.Value<bool>("import_baked") is bool value)
				{
					var toggle = new Toggle("Import Baked Tracks (i.e. baked IK)") { value = value };
					toggle.RegisterValueChangedCallback(e => {
						var options = JObject.Parse(Option.Json);
						options["import_baked"] = e.newValue;
						Option.Json = options.ToString();
						EditorUtility.SetDirty(Importer);
					});
					ret.Add(toggle);
				}
			}

			return ret;
		}
	}
}

#endif
