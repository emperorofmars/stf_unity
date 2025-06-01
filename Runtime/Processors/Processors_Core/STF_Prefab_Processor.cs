using System.Collections.Generic;
using System.Linq;
using com.squirrelbite.stf_unity.modules;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	public class STF_Prefab_Converter : ISTF_PropertyConverter
	{
		public (string RelativePath, System.Type Type, List<string> PropertyNames, System.Func<List<float>, List<float>> ConvertValueFunc) ConvertPropertyPath(ISTF_Resource Resource, List<string> STFPath)
		{
			var resource = Resource as STF_Prefab;
			if(STFPath.Count > 1)
			{
				var nodeId = STFPath[0];
				var target = resource.gameObject.GetComponentsInChildren<STF_NodeResource>().FirstOrDefault(c => c.STF_Owner == resource && c.STF_Id == nodeId);
				if(target && target.PropertyConverter != null)
				{
					var ret = UnityUtil.getPath(resource.transform, target.transform, true);

					(string retRelativePath, System.Type retType, List<string> retPropNames, System.Func<List<float>, List<float>> convertValueFunc) = target.PropertyConverter.ConvertPropertyPath(target, STFPath.GetRange(1, STFPath.Count - 1));
					return (string.IsNullOrEmpty(retRelativePath) ? ret : ret + "/" + retRelativePath, retType, retPropNames, convertValueFunc);
				}
			}
			return ("", null, null, null);
		}
	}

	public class STF_Prefab_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STF_Prefab);
		public uint Order => 0;
		public int Priority => 1;

		public List<Object> Process(ProcessorContext Context, ISTF_Resource STFResource)
		{
			STFResource.PropertyConverter = new STF_Prefab_Converter();
			return null;
		}
	}
}
