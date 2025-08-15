using System.Collections.Generic;
using System.Linq;
using com.squirrelbite.stf_unity.modules;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	public class STF_NodeResource_Converter : ISTF_PropertyConverter
	{
		public (string RelativePath, System.Type Type, List<string> PropertyNames, System.Func<List<float>, List<float>> ConvertValueFunc) ConvertPropertyPath(ISTF_Resource Resource, List<string> STFPath)
		{
			var resource = Resource as STF_NodeResource;
			var convert = new System.Func<List<float>, List<float>>(Values =>
			{
				if (STFPath[0] == "t")
				{
					//var ret = resource.transform.localPosition + new Vector3(-Values[0], Values[1], Values[2]);
					//return new() { ret.x, ret.y, ret.z };
					return new() { -Values[0], Values[1], Values[2] };
				}
				else if (STFPath[0] == "r")
				{
					//var ret = resource.transform.localRotation * new Quaternion(Values[0], -Values[1], -Values[2], Values[3]).normalized;
					//return new() { ret.x, ret.y, ret.z, ret.w };
					return new() { Values[0], -Values[1], -Values[2], Values[3] };
				}
				else return Values;
			});

			if (STFPath.Count > 0)
			{
				if (STFPath[0] == "t") return ("", typeof(Transform), new() { "m_LocalPosition.x", "m_LocalPosition.y", "m_LocalPosition.z" }, convert);
				else if (STFPath[0] == "r") return ("", typeof(Transform), new() { "m_LocalRotation.x", "m_LocalRotation.y", "m_LocalRotation.z", "m_LocalRotation.w" }, convert);
				else if (STFPath[0] == "s") return ("", typeof(Transform), new() { "m_LocalScale.x", "m_LocalScale.y", "m_LocalScale.z" }, convert);
				else if (STFPath[0] == "instance")
				{
					var instance = resource.gameObject.GetComponent<STF_InstanceResource>();
					if (instance && instance.PropertyConverter != null)
					{
						(string retRelativePath, System.Type retType, List<string> retPropNames, System.Func<List<float>, List<float>> convertValueFunc) = instance.PropertyConverter.ConvertPropertyPath(instance, STFPath.GetRange(1, STFPath.Count - 1));
						return (retRelativePath, retType, retPropNames, convertValueFunc);
					}
				}
				else if (STFPath[0] == "component")
				{
					var component = resource.gameObject.GetComponents<STF_MonoBehaviour>().FirstOrDefault(c => c.STF_Owner == resource && c.STF_Id == STFPath[1]);
					if (component && component.PropertyConverter != null)
					{
						(string retRelativePath, System.Type retType, List<string> retPropNames, System.Func<List<float>, List<float>> convertValueFunc) = component.PropertyConverter.ConvertPropertyPath(component, STFPath.GetRange(2, STFPath.Count - 2));
						return (retRelativePath, retType, retPropNames, convertValueFunc);
					}
				}
			}
			return ("", null, null, null);
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
