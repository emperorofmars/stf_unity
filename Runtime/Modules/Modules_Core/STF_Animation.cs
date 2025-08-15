
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public class STF_Animation : STF_DataResource
	{
		[System.Serializable]
		public class Keyframe
		{
			public bool source_of_truth = true;
			public float frame;
			public float value;
			public string interpolation_type = "bezier";
			public string tangent_type;
			public Vector2 out_tangent;
			public Vector2 in_tangent;
		}

		[System.Serializable]
		public class SubTrack
		{
			public List<Keyframe> keyframes = new();
			public int bake_interval = 1;
			public List<float> baked_values;
		}

		[System.Serializable]
		public class Track
		{
			public List<string> target = new();
			public List<SubTrack> subtracks = new();
		}

		public const string STF_TYPE = "stf.animation";
		public override string STF_Type => STF_TYPE;
		public float fps = 30;
		public bool loop = false;
		public float range_start = 0;
		public float range_end = 1;
		public List<Track> tracks = new();

		public STF_Prefab AnimationRoot;
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

			ret.fps = JsonResource.Value<float>("fps");
			ret.loop = JsonResource.Value<bool>("loop");

			ret.AnimationRoot = ContextObject is STF_Prefab ? ContextObject as STF_Prefab : null;

			float lastFrame = 1;

			if(JsonResource.ContainsKey("tracks")) foreach(var trackJson in JsonResource["tracks"])
			{
				var track = new STF_Animation.Track { target = trackJson["target"].ToObject<List<string>>() };
				foreach (var subtrackJson in trackJson["subtracks"])
				{
					if(subtrackJson.Type == JTokenType.Object)
					{
						var subTrack = new STF_Animation.SubTrack();
						track.subtracks.Add(subTrack);
						// todo also retrieve baked values
						foreach (var keyframeJson in subtrackJson["keyframes"])
						{
							if(keyframeJson.Type == JTokenType.Array)
							{
								var keyframe = new STF_Animation.Keyframe();
								subTrack.keyframes.Add(keyframe);

								keyframe.source_of_truth = (bool)keyframeJson[0];
								keyframe.frame = (float)keyframeJson[1];
								keyframe.value = (float)keyframeJson[2];
								keyframe.interpolation_type = (string)keyframeJson[3];

								switch(keyframe.interpolation_type)
								{
									case "bezier":
										keyframe.tangent_type = (string)keyframeJson[4];
										keyframe.out_tangent = new Vector2((float)keyframeJson[5][0], (float)keyframeJson[5][1]);
										if(keyframeJson.Count() > 6)
											keyframe.in_tangent = new Vector2((float)keyframeJson[6][0], (float)keyframeJson[6][1]);
										break;
									case "constant":
										if(keyframeJson.Count() > 4)
											keyframe.in_tangent = new Vector2((float)keyframeJson[4][0], (float)keyframeJson[4][1]);
										break;
									case "linear":
										if(keyframeJson.Count() > 4)
											keyframe.in_tangent = new Vector2((float)keyframeJson[4][0], (float)keyframeJson[4][1]);
										break;
									default:
										// todo warn
										break;
								}
							}
						}
					}
					else
					{
						track.subtracks.Add(null);
					}
				}
				ret.tracks.Add(track);
			}
			if(JsonResource.ContainsKey("range"))
			{
				ret.range_start = (float)JsonResource["range"][0];
				ret.range_end = (float)JsonResource["range"][1];
			}
			else
			{
				ret.range_end = lastFrame;
			}

			return (ret, new() { ret });
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
