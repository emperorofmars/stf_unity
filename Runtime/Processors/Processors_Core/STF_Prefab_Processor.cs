using System.Collections.Generic;
using System.Linq;
using com.squirrelbite.stf_unity.modules;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	public class STF_Prefab_Converter : ISTF_PropertyConverter
	{
		public ImportPropertyPathPart ConvertPropertyPath(ISTF_Resource Resource, List<string> STFPath)
		{
			var resource = Resource as STF_Prefab;
			if(STFPath.Count > 1)
			{
				var nodeId = STFPath[0];
				var target = resource.gameObject.GetComponentsInChildren<STF_NodeResource>().FirstOrDefault(c => c.STF_Owner == resource && c.STF_Id == nodeId);
				if(target && target.PropertyConverter != null)
				{
					var relativePath = UnityUtil.getPath(resource.transform, target.transform, true);

					return new ImportPropertyPathPart(relativePath) + target.PropertyConverter.ConvertPropertyPath(target, STFPath.GetRange(1, STFPath.Count - 1));
				}
			}
			return null;
		}
	}

	public class STF_Prefab_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STF_Prefab);
		public uint Order => 0;
		public int Priority => 1;

		public (List<Object>, List<Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			STFResource.PropertyConverter = new STF_Prefab_Converter();
			return (null, null);
		}
	}
}
