#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using System;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.ava.vrchat.modules;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.processors;
using Newtonsoft.Json.Linq;
using UnityEditor;
using VRC.SDK3.Avatars.Components;

namespace com.squirrelbite.stf_unity.ava.vrchat.processors
{
	public class VRC_AvatarColliders_Processor : ISTF_Processor
	{
		public Type Type => typeof(VRC_AvatarColliders);
		public uint Order => AVA_Avatar_VRC_Processor._Order + 10;
		public int Priority => 10000000;

		public Type TargetType => typeof(VRC_AvatarColliders);


		public (List<UnityEngine.Object>, List<UnityEngine.Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
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

			return (new() { avatar }, null);
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

	[InitializeOnLoad]
	public class Register_VRC_AvatarColliders_VRC
	{
		static Register_VRC_AvatarColliders_VRC()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorVRC.STF_VRC_AVATAR_CONTEXT, new VRC_AvatarColliders_Processor());
		}
	}
}

#endif
#endif
