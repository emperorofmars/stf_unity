#if UNITY_EDITOR

using com.squirrelbite.stf_unity.tools;
using UnityEngine.UIElements;

namespace com.squirrelbite.stf_unity.resources.editors
{
	public interface ISTF_Resource_Editor
	{
		string STF_Type {get;}
		string HeroSettingsLabel {get;}
		bool HasHeroSettings {get;}
		bool HasAdvancedSettings {get;}
		VisualElement CreateHeroSettingsGUI(ImportOptions.ResourceImportOption Option, System.Action EmitChange);
		VisualElement CreateAdvancedSettingsGUI(ImportOptions.ResourceImportOption Option, System.Action EmitChange);
	}
}

#endif
