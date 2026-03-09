using System.Collections.Generic;
using com.squirrelbite.stf_unity.resources;
using Newtonsoft.Json.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.ava.vrchat.modules
{
	public abstract class VRC_ContactBase : STF_NodeComponentResource
	{
		public string shape = "sphere";
		public float radius = 1;
		public float height = 1;
		public Vector3 offset_position;
		public Quaternion offset_rotation;
		public bool filter_avatar = false;
		public bool filter_world = false;
		public bool local_only = false;
		public List<string> collision_tags = new ();

		public void ParseBase(JObject JsonResource)
		{
			if(JsonResource.ContainsKey("shape")) this.shape = JsonResource.Value<string>("shape");
			if(JsonResource.ContainsKey("radius")) this.radius = JsonResource.Value<float>("radius");
			if(JsonResource.ContainsKey("height")) this.height = JsonResource.Value<float>("height");

			if (JsonResource.ContainsKey("radius")) this.radius = JsonResource.Value<float>("radius");
			if (JsonResource.ContainsKey("height")) this.height = JsonResource.Value<float>("height");
			if (JsonResource.ContainsKey("offset_position")) this.offset_position = TRSUtil.ParseVector3(JsonResource["offset_position"] as JArray);
			if (JsonResource.ContainsKey("offset_rotation")) this.offset_rotation = TRSUtil.ParseQuat(JsonResource["offset_rotation"] as JArray);

			if(JsonResource.ContainsKey("filter_avatar")) this.filter_avatar = JsonResource.Value<bool>("filter_avatar");
			if(JsonResource.ContainsKey("filter_world")) this.filter_world = JsonResource.Value<bool>("filter_world");
			if(JsonResource.ContainsKey("local_only")) this.local_only = JsonResource.Value<bool>("local_only");

			if (JsonResource.ContainsKey("collision_tags")) this.collision_tags = JsonResource["collision_tags"].ToObject<List<string>>();

			if (JsonResource.ContainsKey("enabled") && JsonResource.Value<bool>("enabled") == false)
				this.enabled = false;
		}
	}
}
