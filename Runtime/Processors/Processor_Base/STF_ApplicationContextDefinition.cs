
using UnityEngine.UIElements;

namespace com.squirrelbite.stf_unity.processors
{
	public interface STF_ApplicationContextDefinition
	{
		string ContextId { get; }
		string DisplayName { get; }
		ProcessorContextBase Create(ProcessorState State);

		VisualElement CreateSettingsGUI(ImportOptions Options, System.Action EmitChange) { return null; }
	}
}
