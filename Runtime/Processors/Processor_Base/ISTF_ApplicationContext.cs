
namespace com.squirrelbite.stf_unity.processors
{
	public interface ISTF_ApplicationContextFactory
	{
		ProcessorContextBase Create(ProcessorState State);
	}
}
