
using System.Collections.Generic;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	public interface ISTF_GlobalProcessor
	{
		abstract System.Type TargetType { get; }
		abstract uint Order { get; }
		abstract int Priority { get; }

		List<Object> Process(ProcessorContextBase Context);
	}
}
