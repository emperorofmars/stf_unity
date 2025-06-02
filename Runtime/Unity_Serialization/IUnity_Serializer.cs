using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace com.squirrelbite.stf_unity.serialization
{
	public enum SerializerResultConfidenceLevel
	{
		MANUAL,
		GENERATED
	}

	[System.Serializable]
	public struct SerializerResult
	{
		public string STFType;
		public bool IsComplete;
		public JObject Result;
		public UnityEngine.Object Origin;
		public SerializerResultConfidenceLevel Confidence;
	}

	public class UnitySerializerContext
	{
		public UnitySerializerContext(List<UnityEngine.Object> UnityObjects)
		{
			foreach(var o in UnityObjects)
			{
				RegisterObject(o);
			}
		}
		public UnitySerializerContext(UnityEngine.Object[] UnityObjects)
		{
			foreach(var o in UnityObjects)
			{
				RegisterObject(o);
			}
		}
		public UnitySerializerContext(UnityEngine.Object UnityObject)
		{
			RegisterObject(UnityObject);
		}

		private readonly Dictionary<UnityEngine.Object, string> IdMap = new();

		private void RegisterObject(UnityEngine.Object UnityObject)
		{
			if(!IdMap.ContainsKey(UnityObject))
			{
				IdMap.Add(UnityObject, UnityObject.name + "_" + System.Guid.NewGuid().ToString().Split("-")[0]);
			}
		}

		public string GetId(UnityEngine.Object UnityObject)
		{
			if(IdMap.TryGetValue(UnityObject, out var ret)) return ret;
			else return UnityObject.name;
		}
	}

	/// <summary>
	/// STF Unity Json Serializers manually convert Unity Objects into STF Json Resources.
	/// </summary>
	/// Once C# 11 becomes available in Unity, convert to using static virtual interface members (https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/static-virtual-interface-members)
	public interface IUnity_Serializer
	{
		System.Type Target {get;}
		List<SerializerResult> Serialize(UnitySerializerContext Context, UnityEngine.Object UnityObject);
	}
}