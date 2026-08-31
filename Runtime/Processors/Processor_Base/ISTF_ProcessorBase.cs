
using UnityEngine.UIElements;

namespace com.squirrelbite.stf_unity.processors
{
	public interface ISTF_ProcessorBase
	{
		abstract System.Type TargetType { get; }
		abstract uint Order { get; }
		abstract int Priority { get; }

		string SettingsKey {get => null;}
		string HeroSettingsLabel {get => null;}
		bool HasHeroSettings {get => false;}
		bool HasAdvancedSettings {get => false;}
		VisualElement CreateHeroSettingsGUI(ImportOptions.ResourceImportOption Option, System.Action EmitChange) { return null; }
		VisualElement CreateAdvancedSettingsGUI(ImportOptions.ResourceImportOption Option, System.Action EmitChange) { return null; }
	}
}
