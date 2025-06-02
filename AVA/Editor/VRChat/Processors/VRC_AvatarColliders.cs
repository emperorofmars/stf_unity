#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using System;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules.vrchat;
using com.squirrelbite.stf_unity.serialization;
using Newtonsoft.Json.Linq;
using UnityEditor;
using VRC.SDK3.Avatars.Components;

namespace com.squirrelbite.stf_unity.processors.ava.vrchat
{
	public class VRC_AvatarColliders_Processor : ISTF_Processor
	{
		public Type Type => typeof(VRC_AvatarColliders);
		public uint Order => AVA_Avatar_VRC_Processor._Order + 10;
		public int Priority => 10000000;

		public Type TargetType => typeof(VRC_AvatarColliders);


		public List<UnityEngine.Object> Process(ProcessorContext Context, ISTF_Resource STFResource)
		{
			var avatar = Context.Root.GetComponent<VRCAvatarDescriptor>();
			if (!avatar) Context.Report(new STFReport("No Avatar Component created!", ErrorSeverity.FATAL_ERROR, VRC_AvatarColliders._STF_Type));

			var avatarColliders = STFResource as VRC_AvatarColliders;
			var Json = JObject.Parse(avatarColliders.Json);

			avatar.collider_head = ParseVRCCollider(Json["head"]);
			avatar.collider_torso = ParseVRCCollider(Json["torso"]);
			avatar.collider_footL = ParseVRCCollider(Json["footL"]);
			avatar.collider_footR = ParseVRCCollider(Json["footR"]);
			avatar.collider_handL = ParseVRCCollider(Json["handL"]);
			avatar.collider_handR = ParseVRCCollider(Json["handR"]);
			avatar.collider_fingerIndexL = ParseVRCCollider(Json["fingerIndexL"]);
			avatar.collider_fingerIndexR = ParseVRCCollider(Json["fingerIndexR"]);
			avatar.collider_fingerMiddleL = ParseVRCCollider(Json["fingerMiddleL"]);
			avatar.collider_fingerMiddleR = ParseVRCCollider(Json["fingerMiddleR"]);
			avatar.collider_fingerRingL = ParseVRCCollider(Json["fingerRingL"]);
			avatar.collider_fingerRingR = ParseVRCCollider(Json["fingerRingR"]);
			avatar.collider_fingerLittleL = ParseVRCCollider(Json["fingerLittleL"]);
			avatar.collider_fingerLittleR = ParseVRCCollider(Json["fingerLittleR"]);

			return null;
		}

		private static VRCAvatarDescriptor.ColliderConfig ParseVRCCollider(JToken ColliderDef)
		{
			var ColliderConfig = new VRCAvatarDescriptor.ColliderConfig();
			if(ColliderDef == null) return ColliderConfig;
			if(ColliderDef.Type == JTokenType.Boolean)
			{
				ColliderConfig.state = (bool)ColliderDef ? VRCAvatarDescriptor.ColliderConfig.State.Automatic : VRCAvatarDescriptor.ColliderConfig.State.Disabled;
			}
			else if(ColliderDef.Type == JTokenType.Object)
			{
				ColliderConfig.state = VRCAvatarDescriptor.ColliderConfig.State.Custom;
				if(((JObject)ColliderDef).ContainsKey("m")) ColliderConfig.isMirrored = (bool)ColliderDef["m"];
				if(((JObject)ColliderDef).ContainsKey("r")) ColliderConfig.radius = (float)ColliderDef["r"];
				if(((JObject)ColliderDef).ContainsKey("h")) ColliderConfig.height = (float)ColliderDef["h"];
				if(((JObject)ColliderDef).ContainsKey("pos"))ColliderConfig.position = TRSUtil.ParseVector3((JArray)ColliderDef["pos"]);
				if(((JObject)ColliderDef).ContainsKey("rot")) ColliderConfig.rotation = TRSUtil.ParseQuat((JArray)ColliderDef["rot"]);
			}
			return ColliderConfig;
		}
	}

	public class VRC_AvatarColliders_VRCSerializer : IUnity_Serializer
	{
		public Type Target => typeof(VRCAvatarDescriptor);

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
			STF_Processor_Registry.RegisterProcessor(DetectorVRC.STF_VRC_AVATAR_CONTEXT, new VRC_AvatarColliders_Processor());
			Unity_Serializer_Registry.RegisterSerializer(new VRC_AvatarColliders_VRCSerializer());
		}
	}
}

#endif
#endif