#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using System.Collections.Generic;
using com.squirrelbite.stf_unity.ava.vrchat.modules;
using com.squirrelbite.stf_unity.serialization;
using Newtonsoft.Json.Linq;
using UnityEditor;
using VRC.SDK3.Avatars.Components;

namespace com.squirrelbite.stf_unity.ava.vrchat.serialization
{
	public class VRC_AvatarColliders_Serializer : IUnity_Serializer
	{
		public System.Type Target => typeof(VRCAvatarDescriptor);

		private static JToken SerialilzeVRCCollider(VRCAvatarDescriptor.ColliderConfig ColliderConfig)
		{
			if(ColliderConfig.state == VRCAvatarDescriptor.ColliderConfig.State.Disabled) return false;
			if(ColliderConfig.state == VRCAvatarDescriptor.ColliderConfig.State.Automatic) return true;
			return new JObject
			{
				{ "m", ColliderConfig.isMirrored },
				{ "r", ColliderConfig.radius },
				{ "h", ColliderConfig.height },
				{ "pos", TRSUtil.SerializeVector3(ColliderConfig.position) },
				{ "rot", TRSUtil.SerializeQuat(ColliderConfig.rotation) }
			};
		}

		public List<SerializerResult> Serialize(UnitySerializerContext Context, UnityEngine.Object UnityObject)
		{
			var ret = new JObject {{"type", VRC_AvatarColliders._STF_Type}};
			var avatar = (VRCAvatarDescriptor)UnityObject;

			ret.Add("head", SerialilzeVRCCollider(avatar.collider_head));
			ret.Add("torso", SerialilzeVRCCollider(avatar.collider_torso));
			ret.Add("footL", SerialilzeVRCCollider(avatar.collider_footL));
			ret.Add("footR", SerialilzeVRCCollider(avatar.collider_footR));
			ret.Add("handL", SerialilzeVRCCollider(avatar.collider_handL));
			ret.Add("handR", SerialilzeVRCCollider(avatar.collider_handR));
			ret.Add("fingerIndexL", SerialilzeVRCCollider(avatar.collider_fingerIndexL));
			ret.Add("fingerIndexR", SerialilzeVRCCollider(avatar.collider_fingerIndexR));
			ret.Add("fingerMiddleL", SerialilzeVRCCollider(avatar.collider_fingerMiddleL));
			ret.Add("fingerMiddleR", SerialilzeVRCCollider(avatar.collider_fingerMiddleR));
			ret.Add("fingerRingL", SerialilzeVRCCollider(avatar.collider_fingerRingL));
			ret.Add("fingerRingR", SerialilzeVRCCollider(avatar.collider_fingerRingR));
			ret.Add("fingerLittleL", SerialilzeVRCCollider(avatar.collider_fingerLittleL));
			ret.Add("fingerLittleR", SerialilzeVRCCollider(avatar.collider_fingerLittleR));

			return new List<SerializerResult>{new() {
				STFType = VRC_AvatarColliders._STF_Type,
				Origin = UnityObject,
				Result = ret,
				IsComplete = true,
				Confidence = SerializerResultConfidenceLevel.MANUAL,
			}};
		}
	}

	[InitializeOnLoad]
	public class Register_VRC_AvatarColliders_VRC
	{
		static Register_VRC_AvatarColliders_VRC()
		{
			Unity_Serializer_Registry.RegisterSerializer(new VRC_AvatarColliders_Serializer());
		}
	}
}

#endif
#endif
