#if UNITY_EDITOR

using com.squirrelbite.stf_unity.tools;
using UnityEngine.UIElements;

namespace com.squirrelbite.stf_unity.modules.editors
{
	public interface ISTF_Module_Editor
	{
		string STF_Type {get;}
		string HeroSettingsLabel {get;}
		bool HasHeroSettings {get;}
		bool HasAdvancedSettings {get;}
		VisualElement CreateHeroSettingsGUI(STFScriptedImporter Importer, ImportOptions.ResourceImportOption Option);
		VisualElement CreateAdvancedSettingsGUI(STFScriptedImporter Importer, ImportOptions.ResourceImportOption Option);
	}
}

#endif
