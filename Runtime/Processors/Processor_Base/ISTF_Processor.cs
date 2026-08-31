
using System.Collections.Generic;
using com.squirrelbite.stf_unity.resources;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	/// <summary>
	/// Interface for a processor that converts an imported ISTF_Resource into a Unity native construct.
	/// </summary>
	public interface ISTF_Processor
	{
		abstract System.Type TargetType { get; }
		abstract uint Order { get; }
		abstract int Priority { get; }

		(List<Object> ProcessedObjects, List<Object> ObjectsToRegister) Process(ProcessorContextBase Context, ISTF_Resource STFResource);
	}
}
