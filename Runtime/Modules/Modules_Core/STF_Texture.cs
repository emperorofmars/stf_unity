using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public class STF_Texture : STF_DataComponentResource
	{
		public const string _STF_Type = "stf.texture";
		public override string STF_Type => _STF_Type;

		public uint width = 8;
		public uint height = 8;
		public uint downscale_priority = 0;
		public float quality = 1;
	}

	public class STF_Texture_Module : ISTF_Module
	{
		public string STF_Type => STF_Texture._STF_Type;

		public string STF_Kind => "component";

		public int Priority => 1;

		public List<string> LikeTypes => new(){"texture"};

		public List<Type> UnderstoodApplicationTypes => new(){typeof(STF_Texture)};

		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var ret = ScriptableObject.CreateInstance<STF_Texture>();
			ret.SetFromJson(JsonResource, STF_Id, "STF Texture");

			if (JsonResource.ContainsKey("width")) ret.width = JsonResource.Value<uint>("width");
			if (JsonResource.ContainsKey("height")) ret.height = JsonResource.Value<uint>("height");
			if (JsonResource.ContainsKey("downscale_priority")) ret.downscale_priority = JsonResource.Value<uint>("downscale_priority");
			if (JsonResource.ContainsKey("quality")) ret.quality = JsonResource.Value<float>("quality");

			return (ret, new() { ret });
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			throw new NotImplementedException();
		}
	}
}