#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using UnityEditor;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.resources;
using UnityEngine;
using com.squirrelbite.stf_unity.ava.vrchat.modules;
using VRC.SDK3.Dynamics.Contact.Components;

namespace com.squirrelbite.stf_unity.ava.vrchat.processors
{
	public class VRC_ContactReceiver_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(VRC_ContactReceiver);

		public const uint _Order = 100;
		public uint Order => _Order;

		public int Priority => 1;

		public (List<Object> ProcessedObjects, List<Object> ObjectsToRegister) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var stfReceiver = STFResource as VRC_ContactReceiver;
			var receiver = stfReceiver.gameObject.AddComponent<VRCContactReceiver>();
			receiver.shapeType = stfReceiver.shape == "capsule" ? VRC.Dynamics.ContactBase.ShapeType.Capsule : VRC.Dynamics.ContactBase.ShapeType.Sphere;
			receiver.radius = stfReceiver.radius;
			receiver.height = stfReceiver.height;
			receiver.position = stfReceiver.offset_position;
			receiver.rotation = stfReceiver.offset_rotation;

			receiver.contentTypes = VRC.Dynamics.DynamicsUsageFlags.Nothing;
			if(stfReceiver.filter_avatar) receiver.contentTypes |= VRC.Dynamics.DynamicsUsageFlags.Avatar;
			if(stfReceiver.filter_world) receiver.contentTypes |= VRC.Dynamics.DynamicsUsageFlags.World;
			receiver.localOnly = stfReceiver.local_only;

			receiver.collisionTags = stfReceiver.collision_tags;

			receiver.receiverType = stfReceiver.receiver_type switch {
				"constant" => VRC.Dynamics.ContactReceiver.ReceiverType.Constant,
				"on_enter" => VRC.Dynamics.ContactReceiver.ReceiverType.OnEnter,
				"proximity" => VRC.Dynamics.ContactReceiver.ReceiverType.Proximity,
				_ => VRC.Dynamics.ContactReceiver.ReceiverType.Constant,
			};

			receiver.parameter = stfReceiver.parameter;

			receiver.enabled = stfReceiver.enabled;

			return (new() { receiver }, null);
		}
	}

	[InitializeOnLoad]
	class Register_VRC_ContactReceiver_Processor
	{
		static Register_VRC_ContactReceiver_Processor()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorVRC.STF_VRC_AVATAR_CONTEXT, new VRC_ContactReceiver_Processor());
		}
	}
}

#endif
#endif
