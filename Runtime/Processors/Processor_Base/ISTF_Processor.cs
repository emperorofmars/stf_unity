
using System.Collections.Generic;
using com.squirrelbite.stf_unity.resources;

namespace com.squirrelbite.stf_unity.processors
{
	/// <summary>
	/// Interface for a processor that converts an imported ISTF_Resource into a Unity native construct.
	/// </summary>
	public interface ISTF_Processor : ISTF_ProcessorBase
	{
		(List<UnityEngine.Object> ProcessedObjects, List<UnityEngine.Object> ObjectsToRegister) Process(ProcessorContextBase Context, ISTF_Resource STFResource);
	}
}
