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
	public class VRC_Collider_Serializer : IUnity_Serializer
	{
		public static readonly System.Type _Target = typeof(VRCPhysBoneCollider);
		public System.Type Target => _Target;

		public List<SerializerResult> Serialize(UnitySerializerContext Context, Object UnityObject)
		{
			var collider = (VRCPhysBoneCollider)UnityObject;

			if (collider.shapeType == VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Sphere)
			{
				var ret = new JObject() {
					{ "type", AVA_Collider_Sphere._STF_Type},
					{ "radius", collider.radius },
					{ "offset_position", TRSUtil.SerializeVector3(collider.position) },
				};
				return new List<SerializerResult>{new() {
					STFType = AVA_Collider_Sphere._STF_Type,
					Origin = UnityObject,
					Result = ret,
					IsComplete = true,
					Confidence = SerializerResultConfidenceLevel.MANUAL,
				}};
			}
			if (collider.shapeType == VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Capsule)
			{
				var ret = new JObject() {
					{ "type", AVA_Collider_Capsule._STF_Type},
					{ "radius", collider.radius },
					{ "height", collider.height },
					{ "offset_position", TRSUtil.SerializeVector3(collider.position) },
					{ "offset_rotation", TRSUtil.SerializeQuat(collider.rotation) },
				};
				return new List<SerializerResult>{new() {
					STFType = AVA_Collider_Capsule._STF_Type,
					Origin = UnityObject,
					Result = ret,
					IsComplete = true,
					Confidence = SerializerResultConfidenceLevel.MANUAL,
				}};
			}
			if (collider.shapeType == VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Plane)
			{
				var ret = new JObject() {
					{ "type", AVA_Collider_Plane._STF_Type},
					{ "offset_position", TRSUtil.SerializeVector3(collider.position) },
					{ "offset_rotation", TRSUtil.SerializeQuat(collider.rotation) },
				};
				return new List<SerializerResult>{new() {
					STFType = AVA_Collider_Plane._STF_Type,
					Origin = UnityObject,
					Result = ret,
					IsComplete = true,
					Confidence = SerializerResultConfidenceLevel.MANUAL,
				}};
			}
			else return null;
		}
	}

	[InitializeOnLoad]
	public class Register_VRC_Collider_Serializer
	{
		static Register_VRC_Collider_Serializer()
		{
			Unity_Serializer_Registry.RegisterSerializer(new VRC_Collider_Serializer());
		}
	}
}

#endif
#endif
