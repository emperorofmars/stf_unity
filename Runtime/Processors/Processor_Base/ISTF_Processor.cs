
using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	public interface ISTF_Processor
	{
		abstract System.Type TargetType { get; }
		abstract uint Order { get; }
		abstract int Priority { get; }

		(List<Object> ProcessedObjects, List<Object> ObjectsToRegister) Process(ProcessorContextBase Context, ISTF_Resource STFResource);
	}
}
