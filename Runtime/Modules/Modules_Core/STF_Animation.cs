
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public class STF_Animation : STF_DataResource
	{
		[System.Serializable]
		public class KeyframeValue
		{
			public bool isBaked = false;
			public float value;
			public Vector2 in_tangent;
			public Vector2 out_tangent;
		}

		[System.Serializable]
		public class Keyframe
		{
			public float frame;
			public List<KeyframeValue> values = new();

		}

		[System.Serializable]
		public class Track
		{
			public List<string> target = new();
			public List<Keyframe> keyframes = new();
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
				foreach (var keyframeJson in trackJson["keyframes"])
				{
					var keyframe = new STF_Animation.Keyframe {
						frame = keyframeJson.Value<float>("frame"),
					};
					foreach(var keyframeValueJson in keyframeJson["values"])
					{
						if(keyframeValueJson != null && keyframeValueJson.Type == JTokenType.Array)
						{
							JArray keyframeValues = keyframeValueJson as JArray;
							// todo legacy, remove at some point
							if (keyframeValues.Count == 5)
							{
								keyframe.values.Add(new STF_Animation.KeyframeValue {
									isBaked = false,
									value = (float)keyframeValueJson[0],
									in_tangent = new Vector2((float)keyframeValueJson[1], (float)keyframeValueJson[2]),
									out_tangent = new Vector2((float)keyframeValueJson[3], (float)keyframeValueJson[4]),
								});
							}
							else if (keyframeValues.Count == 1)
							{
								keyframe.values.Add(new STF_Animation.KeyframeValue {
									isBaked = true,
									value = (float)keyframeValueJson[0],
								});
							}
							// not legacy, keep this
							else if (keyframeValues.Count == 6)
							{
								keyframe.values.Add(new STF_Animation.KeyframeValue {
									isBaked = !(bool)keyframeValueJson[0],
									value = (float)keyframeValueJson[1],
									in_tangent = new Vector2((float)keyframeValueJson[2], (float)keyframeValueJson[3]),
									out_tangent = new Vector2((float)keyframeValueJson[4], (float)keyframeValueJson[5]),
								});
							}
							else if (keyframeValues.Count == 2)
							{
								keyframe.values.Add(new STF_Animation.KeyframeValue {
									isBaked = !(bool)keyframeValueJson[0],
									value = (float)keyframeValueJson[1],
								});
							}
							else
							{
								keyframe.values.Add(null);
							}
						}
						else
						{
							keyframe.values.Add(null);
						}
					}

					track.keyframes.Add(keyframe);
					if(keyframe.frame > lastFrame) lastFrame = keyframe.frame;
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
