
using System.Collections.Generic;

namespace com.squirrelbite.stf_unity.processors
{
	/// <summary>
	/// Interface for a processor that runs regardless whether zero, one or more resources of it s target type exist. Converts all imported ISTF_Resources of its target type into Unity native constructs.
	/// </summary>
	public interface ISTF_GlobalProcessor: ISTF_ProcessorBase
	{
		List<UnityEngine.Object> Process(ProcessorContextBase Context);
	}
}
