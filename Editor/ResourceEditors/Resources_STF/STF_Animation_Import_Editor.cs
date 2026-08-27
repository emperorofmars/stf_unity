#if UNITY_EDITOR

using Newtonsoft.Json.Linq;
using UnityEngine.UIElements;

namespace com.squirrelbite.stf_unity.resources.editors
{
	public class STF_Animation_Import_Editor : ISTF_Resource_Editor
	{
		public string STF_Type => STF_Animation.STF_TYPE;
		public string HeroSettingsLabel => null;
		public bool HasHeroSettings => false;
		public bool HasAdvancedSettings => true;

		public VisualElement CreateHeroSettingsGUI(ImportOptions.ResourceImportOption Option, System.Action EmitChange)
		{
			return null;
		}

		public VisualElement CreateAdvancedSettingsGUI(ImportOptions.ResourceImportOption Option, System.Action EmitChange)
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
						EmitChange();
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
						EmitChange();
					});
					ret.Add(toggle);
				}
			}

			return ret;
		}
	}
}

#endif
