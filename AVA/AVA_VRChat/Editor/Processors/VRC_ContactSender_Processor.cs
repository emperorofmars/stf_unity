#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using UnityEditor;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.modules;
using UnityEngine;
using com.squirrelbite.stf_unity.ava.vrchat.modules;
using VRC.SDK3.Dynamics.Contact.Components;

namespace com.squirrelbite.stf_unity.ava.vrchat.processors
{
	public class VRC_ContactSender_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(VRC_ContactSender);

		public const uint _Order = 100;
		public uint Order => _Order;

		public int Priority => 1;

		public (List<Object> ProcessedObjects, List<Object> ObjectsToRegister) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var stfSender = STFResource as VRC_ContactSender;
			var sender = stfSender.gameObject.AddComponent<VRCContactSender>();
			sender.shapeType = stfSender.shape == "capsule" ? VRC.Dynamics.ContactBase.ShapeType.Capsule : VRC.Dynamics.ContactBase.ShapeType.Sphere;
			sender.radius = stfSender.radius;
			sender.height = stfSender.height;
			sender.position = stfSender.offset_position;
			sender.rotation = stfSender.offset_rotation;

			sender.contentTypes = VRC.Dynamics.DynamicsUsageFlags.Nothing;
			if(stfSender.filter_avatar) sender.contentTypes |= VRC.Dynamics.DynamicsUsageFlags.Avatar;
			if(stfSender.filter_world) sender.contentTypes |= VRC.Dynamics.DynamicsUsageFlags.World;
			sender.localOnly = stfSender.local_only;

			sender.collisionTags = stfSender.collision_tags;

			sender.enabled = stfSender.enabled;

			return (new() { sender }, null);
		}
	}

	[InitializeOnLoad]
	class Register_VRC_ContactSender_Processor
	{
		static Register_VRC_ContactSender_Processor()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorVRC.STF_VRC_AVATAR_CONTEXT, new VRC_ContactSender_Processor());
		}
	}
}

#endif
#endif
