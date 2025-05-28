
using System.Collections.Generic;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	public interface ISTF_Processor
	{
		abstract System.Type TargetType { get; }
		abstract uint Order { get; }
		abstract int Priority { get; }

		List<Object> Process(ProcessorContext Context, ISTF_Resource STFResource);
	}

}
