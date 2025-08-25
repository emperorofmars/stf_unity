#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using System.Collections.Generic;
using com.squirrelbite.stf_unity.serialization;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Dynamics.PhysBone.Components;

namespace com.squirrelbite.stf_unity.ava.vrchat.serialization
{
	public class VRC_Physbone_Serializer : IUnity_Serializer
	{
		public static readonly System.Type _Target = typeof(VRCPhysBone);
		public System.Type Target => _Target;

		public List<SerializerResult> Serialize(UnitySerializerContext Context, Object UnityObject)
		{
			var physbone = (VRCPhysBone)UnityObject;

			var parsed = JObject.Parse(JsonUtility.ToJson(physbone));
			parsed.Remove("rootTransform");
			parsed.Remove("ignoreTransforms");
			parsed.Remove("colliders");

			parsed.Remove("foldout_transforms");
			parsed.Remove("foldout_forces");
			parsed.Remove("foldout_collision");
			parsed.Remove("foldout_stretchsquish");
			parsed.Remove("foldout_limits");
			parsed.Remove("foldout_grabpose");
			parsed.Remove("foldout_options");
			parsed.Remove("foldout_gizmos");

			return new List<SerializerResult>{new() {
				STFType = "vrc.physbone",
				Origin = UnityObject,
				Result = parsed,
				IsComplete = true,
				Confidence = SerializerResultConfidenceLevel.MANUAL,
			}};
		}
	}

	[InitializeOnLoad]
	public class Register_VRC_Physbone_Serializer
	{
		static Register_VRC_Physbone_Serializer()
		{
			Unity_Serializer_Registry.RegisterSerializer(new VRC_Physbone_Serializer());
		}
	}
}

#endif
#endif
