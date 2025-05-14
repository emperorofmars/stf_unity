
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public class STF_Animation : STF_DataResource
	{
		[System.Serializable]
		public class Keyframe
		{
			public float time;
			public float value;
			public float in_tangent_x;
			public float in_tangent_y;
			public float out_tangent_x;
			public float out_tangent_y;
		}

		[System.Serializable]
		public class Track
		{
			public List<string> target = new();
			public List<Keyframe> keyframes = new();
		}

		public const string STF_TYPE = "stf.animation";
		public override string STF_Type => STF_TYPE;
		public bool loop = false;
		public float range_start = 0;
		public float range_end = 1;
		public List<Track> tracks = new();
	}

	public class STF_Animation_Module : ISTF_Module
	{
		public string STF_Type => STF_Animation.STF_TYPE;

		public string STF_Kind => "data";

		public int Priority => 0;

		public List<string> LikeTypes => new(){"animation"};

		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(STF_Animation)};

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var ret = ScriptableObject.CreateInstance<STF_Animation>();
			ret.SetFromJson(JsonResource, STF_Id, "STF Animation");

			ret.loop = JsonResource.Value<bool>("loop");
			if(JsonResource.ContainsKey("range"))
			{
				ret.range_start = (float)JsonResource["range"][0];
				ret.range_end = (float)JsonResource["range"][1];
			}

			if(JsonResource.ContainsKey("tracks")) foreach(var trackJson in JsonResource["tracks"])
			{
				var track = new STF_Animation.Track { target = trackJson["target"].ToObject<List<string>>() };
				foreach (var keyframeJson in trackJson["keyframes"])
				{
					track.keyframes.Add(new STF_Animation.Keyframe {
						time = (float)keyframeJson[0],
						value = (float)keyframeJson[1],
						in_tangent_x = (float)keyframeJson[2],
						in_tangent_y = (float)keyframeJson[3],
						out_tangent_x = (float)keyframeJson[4],
						out_tangent_y = (float)keyframeJson[5]
					});
				}
				ret.tracks.Add(track);
			}

			return (ret, new(){ret});
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			var Animation = ApplicationObject as STF_Animation;
			var ret = new JObject {
				{"type", STF_Type},
				{"name", Animation.STF_Name},
			};

			return (ret, Animation.STF_Id);
		}
	}
}
