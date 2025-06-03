
namespace com.squirrelbite.stf_unity.processors
{
	public interface STF_ApplicationContextDefinition
	{
		string ContextId { get; }
		string DisplayName { get; }
		ProcessorContextBase Create(ProcessorState State);
	}
}
