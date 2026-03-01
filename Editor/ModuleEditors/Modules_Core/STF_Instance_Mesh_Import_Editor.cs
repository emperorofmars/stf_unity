#if UNITY_EDITOR

using com.squirrelbite.stf_unity.tools;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace com.squirrelbite.stf_unity.modules.editors
{
	public class STF_Instance_Mesh_Import_Editor : ISTF_Module_Editor
	{
		public string STF_Type => STF_Instance_Mesh.STF_TYPE;
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

			if(options.ContainsKey("reparent_skinned") && options.Value<bool>("reparent_skinned") is bool reparentSkinned)
			{
				var toggleReparentSkinned = new Toggle("Reparent besides Armature") { value = reparentSkinned };
				toggleReparentSkinned.RegisterValueChangedCallback(e => {
					var options = JObject.Parse(Option.Json);
					options["reparent_skinned"] = e.newValue;
					Option.Json = options.ToString();
					EditorUtility.SetDirty(Importer);
				});
				ret.Add(toggleReparentSkinned);
			}

			return ret;
		}
	}
}

#endif
