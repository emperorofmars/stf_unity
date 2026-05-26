using System.Collections.Generic;
using com.squirrelbite.stf_unity.resources;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	public class ComponentAnimationConverterBase<T> : ISTF_PropertyConverter where T : Component
	{
		public ImportPropertyPathPart ConvertPropertyPath(ISTF_Resource STFResource, List<string> STFPath)
		{
			if (STFPath.Count == 1 && STFPath[0] == "enabled")
				return new ImportPropertyPathPart(typeof(T), new() { "enabled" });
			else
				return ConvertComponentPropertyPath(STFResource, STFPath);
		}

		public virtual ImportPropertyPathPart ConvertComponentPropertyPath(ISTF_Resource STFResource, List<string> STFPath)
		{
			return null;
		}
	}
}
