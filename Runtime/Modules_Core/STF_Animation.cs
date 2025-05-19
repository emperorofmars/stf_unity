
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

		public AnimationClip ProcessedUnityAnimation;

		public override (string RelativePath, System.Type Type, List<string> PropertyNames, System.Func<List<float>, List<float>> ConvertValueFunc) ConvertPropertyPath(List<string> STFPath)
		{
			throw new System.NotImplementedException();
		}

		public override List<string> ConvertPropertyPath(string UnityPath)
		{
			throw new System.NotImplementedException();
		}
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

			float lastFrame = 1;

			if(JsonResource.ContainsKey("tracks")) foreach(var trackJson in JsonResource["tracks"])
			{
				var track = new STF_Animation.Track { target = trackJson["target"].ToObject<List<string>>() };
				foreach (var keyframeJson in trackJson["keyframes"])
				{
					var keyframe = new STF_Animation.Keyframe {
						frame = (float)keyframeJson.Value<float>("frame"),
					};
					foreach(var keyframeValueJson in keyframeJson["values"])
					{
						if(keyframeValueJson != null && keyframeValueJson.Type != JTokenType.Null && keyframeValueJson.Type != JTokenType.None && keyframeValueJson.Type != JTokenType.Undefined)
						{
							keyframe.values.Add(new STF_Animation.KeyframeValue {
								value = (float)keyframeValueJson[0],
								in_tangent = new Vector2((float)keyframeValueJson[1], (float)keyframeValueJson[2]),
								out_tangent = new Vector2((float)keyframeValueJson[3], (float)keyframeValueJson[4]),
							});
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


			ret.ProcessedUnityAnimation = ConvertToUnityAnimation(Context, ret, (STF_Prefab)ContextObject);
			if(ret.ProcessedUnityAnimation)
				return (ret, new(){ret, ret.ProcessedUnityAnimation});
			else
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

		public AnimationClip ConvertToUnityAnimation(ImportContext Context, STF_Animation STFAnimation, STF_Prefab ContextObject)
		{
			var ret = new AnimationClip {
				name = STFAnimation.STF_Name,
				frameRate = STFAnimation.fps,
				wrapMode = STFAnimation.loop ? WrapMode.Loop : WrapMode.Default
			};

			// TODO figure out if this is actually how it works
			var tangentWeightNormalizeFactor = Math.Max(1, STFAnimation.range_end - STFAnimation.range_start);

			foreach(var track in STFAnimation.tracks)
			{
				(string RelativePath, System.Type CurveType, List<string> PropertyNames, System.Func<List<float>, List<float>> ConvertValueFunc) = ContextObject.ConvertPropertyPath(track.target);

				if(!string.IsNullOrWhiteSpace(RelativePath) && PropertyNames != null && PropertyNames.Count > 0)
				{
					var curves = new List<AnimationCurve>();
					for(int curveIndex = 0; curveIndex < PropertyNames.Count; curveIndex++)
					{
						curves.Add(new AnimationCurve());
					}

					foreach(var stfKeyframe in track.keyframes)
					{
						var originalValues = new List<float>(new float[PropertyNames.Count]);
						for(int curveIndex = 0; curveIndex < PropertyNames.Count; curveIndex++) originalValues[curveIndex] = stfKeyframe.values[curveIndex] != null ? stfKeyframe.values[curveIndex].value : 0;
						var values = ConvertValueFunc != null ? ConvertValueFunc(originalValues) : originalValues;

						for(int curveIndex = 0; curveIndex < PropertyNames.Count; curveIndex++)
						{
							if(stfKeyframe.values[curveIndex] != null)
								curves[curveIndex].AddKey(new Keyframe {
									time = stfKeyframe.frame / STFAnimation.fps,
									value = values[curveIndex],
									inTangent = stfKeyframe.values[curveIndex].in_tangent.x < 0 ? -stfKeyframe.values[curveIndex].in_tangent.y * (1 / -stfKeyframe.values[curveIndex].in_tangent.x) : 0,
									inWeight = stfKeyframe.values[curveIndex].in_tangent.magnitude / tangentWeightNormalizeFactor,
									outTangent = stfKeyframe.values[curveIndex].out_tangent.x < 0 ? -stfKeyframe.values[curveIndex].out_tangent.y * (1 / stfKeyframe.values[curveIndex].out_tangent.x) : 0,
									outWeight = stfKeyframe.values[curveIndex].out_tangent.magnitude / tangentWeightNormalizeFactor,
								});
						}
					}

					for(int curveIndex = 0; curveIndex < PropertyNames.Count; curveIndex++)
					{
						//Debug.Log($"Curve: {RelativePath} - {PropertyNames[curveIndex]} ({CurveType})");

						if(RelativePath != null && CurveType != null && PropertyNames != null)
							ret.SetCurve(RelativePath, CurveType, PropertyNames[curveIndex], curves[curveIndex]);
					}
				}
				// Else Warning
			}

			return ret;
		}
	}
}
