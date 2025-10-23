#if UNITY_EDITOR

using com.squirrelbite.stf_unity.tools;

namespace com.squirrelbite.stf_unity.modules.editors
{
	public interface ISTF_Module_Editor
	{
		string STF_Type {get;}
		string HeroSettingsLabel {get;}
		bool HasHeroSettings {get;}
		bool HasAdvancedSettings {get;}
		void DrawHeroSettings(STFScriptedImporter Importer, ImportOptions.ResourceImportOption Option);
		void DrawAdvancedSettings(STFScriptedImporter Importer, ImportOptions.ResourceImportOption Option);
	}
}

#endif
