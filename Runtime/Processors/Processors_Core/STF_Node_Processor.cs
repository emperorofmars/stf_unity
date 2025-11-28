using System.Collections.Generic;
using System.Linq;
using com.squirrelbite.stf_unity.modules;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	public class STF_NodeResource_Converter : ISTF_PropertyConverter
	{
		public ImportPropertyPathPart ConvertPropertyPath(ISTF_Resource Resource, List<string> STFPath)
		{
			var resource = Resource as STF_NodeResource;
			var convert = new System.Func<List<float>, List<float>>(Values =>
			{
				if (STFPath[0] == "t")
				{
					return new() { -Values[0], Values[1], Values[2] };
				}
				else if (STFPath[0] == "r")
				{
					return new() { Values[0], -Values[1], -Values[2], Values[3] };
				}
				else if (STFPath[0] == "r_euler")
				{
					return new() { Mathf.Rad2Deg * Values[0], Mathf.Rad2Deg * Values[1], -Mathf.Rad2Deg * Values[2] };
				}
				else return Values;
			});

			if (STFPath.Count > 0)
			{
				if (STFPath[0] == "t") return new ImportPropertyPathPart(typeof(Transform), new() { "localPosition.x", "localPosition.y", "localPosition.z" }, convert);
				else if (STFPath[0] == "r") return new ImportPropertyPathPart(typeof(Transform), new() { "localRotation.x", "localRotation.y", "localRotation.z", "localRotation.w" }, convert);
				else if (STFPath[0] == "r_euler") return new ImportPropertyPathPart(typeof(Transform), new() { "localEulerAngles.x", "localEulerAngles.y", "localEulerAngles.z" }, convert);
				else if (STFPath[0] == "s") return new ImportPropertyPathPart(typeof(Transform), new() { "localScale.x", "localScale.y", "localScale.z" }, convert);
				else if (STFPath[0] == "enabled") return new ImportPropertyPathPart(typeof(GameObject), new() { "active" });
				else if (STFPath[0] == "instance")
				{
					var instance = resource.gameObject.GetComponent<STF_InstanceResource>();
					if (instance && instance.PropertyConverter != null)
					{
						return instance.PropertyConverter.ConvertPropertyPath(instance, STFPath.GetRange(1, STFPath.Count - 1));
					}
				}
				else if (STFPath[0] == "component")
				{
					var component = resource.gameObject.GetComponents<STF_MonoBehaviour>().FirstOrDefault(c => c.STF_Owner == resource && c.STF_Id == STFPath[1]);
					if (component && component.PropertyConverter != null)
					{
						return component.PropertyConverter.ConvertPropertyPath(component, STFPath.GetRange(2, STFPath.Count - 2));
					}
				}
			}
			return null;
		}
	}

	public class STF_Node_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STF_Node);
		public uint Order => 0;
		public int Priority => 1;

		public (List<Object>, List<Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			STFResource.PropertyConverter = new STF_NodeResource_Converter();
			return (null, null);
		}
	}

	public class STF_Bone_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STF_Bone);
		public uint Order => 0;
		public int Priority => 1;

		public (List<Object>, List<Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			STFResource.PropertyConverter = new STF_NodeResource_Converter();
			return (null, null);
		}
	}
}
