#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using System.Collections.Generic;
using com.squirrelbite.stf_unity.ava.vrchat.modules;
using com.squirrelbite.stf_unity.serialization;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Dynamics.Contact.Components;

namespace com.squirrelbite.stf_unity.ava.vrchat.serialization
{
	public abstract class VRC_Contact_SerializerBase : IUnity_Serializer
	{
		public abstract System.Type Target { get; }
		protected abstract string STF_Type { get; }

		protected virtual void SerializeSub(VRC.Dynamics.ContactBase Receiver, JObject JsonResource) {}

		public List<SerializerResult> Serialize(UnitySerializerContext Context, Object UnityObject)
		{
			var contact = (VRC.Dynamics.ContactBase)UnityObject;

			var ret = new JObject() {
				{ "type", STF_Type},
			};

			if (contact.shapeType == VRC.Dynamics.ContactBase.ShapeType.Sphere)
			{
				ret["shape"] = "sphere";
				ret["radius"] = contact.radius;
				ret["offset_position"] = TRSUtil.SerializeVector3(contact.position);
			}
			else if (contact.shapeType == VRC.Dynamics.ContactBase.ShapeType.Capsule)
			{
				ret["shape"] = "capsule";
				ret["radius"] = contact.radius;
				ret["height"] = contact.height;
				ret["offset_position"] = TRSUtil.SerializeVector3(contact.position);
				ret["offset_rotation"] = TRSUtil.SerializeQuat(contact.rotation);
			}

			ret["filter_avatar"] = (contact.contentTypes & VRC.Dynamics.DynamicsUsageFlags.Avatar) > 0;
			ret["filter_world"] = (contact.contentTypes & VRC.Dynamics.DynamicsUsageFlags.World) > 0;
			ret["local_only"] = contact.localOnly;

			ret["collision_tags"] = new JArray(contact.collisionTags);

			SerializeSub(contact, ret);

			return new List<SerializerResult>{new() {
				STFType = STF_Type,
				Origin = UnityObject,
				Result = ret,
				IsComplete = true,
				Confidence = SerializerResultConfidenceLevel.MANUAL,
			}};
		}
	}

	public class VRC_ContactSender_Serializer : VRC_Contact_SerializerBase
	{
		public static readonly System.Type _Target = typeof(VRCContactSender);
		public override System.Type Target => _Target;

		protected override string STF_Type => VRC_ContactSender._STF_Type;
	}

	public class VRC_ContactReceiver_Serializer : VRC_Contact_SerializerBase
	{
		public static readonly System.Type _Target = typeof(VRCContactReceiver);
		public override System.Type Target => _Target;

		protected override string STF_Type => VRC_ContactReceiver._STF_Type;

		protected override void SerializeSub(VRC.Dynamics.ContactBase Contact, JObject JsonResource)
		{
			if(Contact is VRCContactReceiver receiver)
			{
				JsonResource["receiver_type"] = receiver.receiverType switch {
					VRC.Dynamics.ContactReceiver.ReceiverType.Constant => "constant",
					VRC.Dynamics.ContactReceiver.ReceiverType.OnEnter => "on_enter",
					VRC.Dynamics.ContactReceiver.ReceiverType.Proximity => "proximity",
					_ => throw new System.NotImplementedException(),
				};
				JsonResource["parameter"] = receiver.parameter;
			}
		}
	}

	[InitializeOnLoad]
	public class Register_VRC_Contact_Serializer
	{
		static Register_VRC_Contact_Serializer()
		{
			Unity_Serializer_Registry.RegisterSerializer(new VRC_ContactSender_Serializer());
			Unity_Serializer_Registry.RegisterSerializer(new VRC_ContactReceiver_Serializer());
		}
	}
}

#endif
#endif
