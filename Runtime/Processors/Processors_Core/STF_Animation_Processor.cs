using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	public class STF_Animation_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STF_Animation);
		public const uint _Order = 100000000;
		public uint Order => _Order;
		public int Priority => 1;

		public (List<Object>, List<Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var STFAnimation = STFResource as STF_Animation;
			if (STFAnimation.AnimationRoot.PropertyConverter == null)
			{
				return (null, null); //TODO Report Warning
			}

			var preferBaked = Context.ImportConfig.GetAndConfirmImportOption(STF_Animation.STF_TYPE, STFAnimation.STF_Id, STFAnimation.STF_Name, "prefer_baked", false);

			var ret = new AnimationClip
			{
				name = STFAnimation.STF_Name,
				frameRate = STFAnimation.fps,
			};
			switch(STFAnimation.loop)
			{
				case "loop":
					ret.wrapMode = WrapMode.Loop;
					break;
				case "pingpong":
					ret.wrapMode = WrapMode.PingPong;
					break;
			}

			var handledTracks = new HashSet<string>();

			void handleTracks(List<STF_Animation.Track> tracks)
			{
				foreach (var track in tracks)
				{
					var pathRet = STFAnimation.AnimationRoot.PropertyConverter.ConvertPropertyPath(STFAnimation.AnimationRoot, track.target);
					if (pathRet == null || !pathRet.IsValid())
					{
						continue; //TODO Report Warning
					}
					if(handledTracks.Contains(pathRet.ToString()))
						continue;
					handledTracks.Add(pathRet.ToString());

					var curves = new List<AnimationCurve>();
					for (int curveIndex = 0; curveIndex < pathRet.PropertyNames.Count; curveIndex++)
						curves.Add(new AnimationCurve());

					var len = -1;
					foreach(var subtrack in track.subtracks) if(subtrack != null && subtrack.keyframes.Count > len) len = subtrack.keyframes.Count;
					if(len <= 0) continue; // error

					var convertedValues = new List<List<float>>();
					var convertedInTangentValues = new List<List<float>>();
					var convertedOutTangentValues = new List<List<float>>();
					for (int i = 0; i < len; i++)
					{
						var originalValues = new List<float>(new float[track.subtracks.Count]);
						var originalInTangentValues = new List<float>(new float[track.subtracks.Count]);
						var originalOutTangentValues = new List<float>(new float[track.subtracks.Count]);
						for(int subtrackIndex = 0; subtrackIndex < track.subtracks.Count; subtrackIndex++)
							if(track.subtracks[subtrackIndex] != null && track.subtracks[subtrackIndex].keyframes[i] is var stfKeyframe && stfKeyframe != null)
							{
								originalValues[subtrackIndex] = stfKeyframe.value;
								originalInTangentValues[subtrackIndex] = stfKeyframe.in_tangent.y;
								originalOutTangentValues[subtrackIndex] = stfKeyframe.out_tangent.y;
							}
							else
							{
								originalValues[subtrackIndex] = 0;
								originalInTangentValues[subtrackIndex] = 0;
								originalOutTangentValues[subtrackIndex] = 0;
							}
						convertedValues.Add(pathRet.ConvertValueFunc != null ? pathRet.ConvertValueFunc(originalValues) : originalValues);
						convertedInTangentValues.Add(pathRet.ConvertValueFunc != null ? pathRet.ConvertValueFunc(originalInTangentValues) : originalInTangentValues);
						convertedOutTangentValues.Add(pathRet.ConvertValueFunc != null ? pathRet.ConvertValueFunc(originalOutTangentValues) : originalOutTangentValues);
					}

					for (int i = 0; i < len; i++)
					{
						var values = convertedValues[i];
						var inTangentValues = convertedInTangentValues[i];
						var outTangentValues = convertedOutTangentValues[i];

						for(int subtrackIndex = 0; subtrackIndex < track.subtracks.Count; subtrackIndex++) if(track.subtracks[subtrackIndex] != null && track.subtracks[subtrackIndex].keyframes[i] is var stfKeyframe && stfKeyframe != null && stfKeyframe.source_of_truth)
						{
							STF_Animation.Keyframe prevKeyframe = null;
							List<float> prevValues = null;
							for(int j = i - 1; j >= 0; j--) if(track.subtracks[subtrackIndex].keyframes[j] != null && track.subtracks[subtrackIndex].keyframes[j].source_of_truth)
							{
								prevKeyframe = track.subtracks[subtrackIndex].keyframes[j];
								prevValues = convertedValues[j];
								break;
							}
							STF_Animation.Keyframe nextKeyframe = null;
							List<float> nextValues = null;
							for(int j = i + 1; j < track.subtracks[subtrackIndex].keyframes.Count; j--) if(track.subtracks[subtrackIndex].keyframes[j] != null && track.subtracks[subtrackIndex].keyframes[j].source_of_truth)
							{
								nextKeyframe = track.subtracks[subtrackIndex].keyframes[j];
								nextValues = convertedValues[j];
								break;
							}

							var keyframeDistanceLeft = prevKeyframe != null ? System.Math.Abs(prevKeyframe.frame - stfKeyframe.frame) : 1;
							var keyframeDistanceRight = nextKeyframe != null ? System.Math.Abs(nextKeyframe.frame - stfKeyframe.frame) : 1;

							var keyframe = new Keyframe
							{
								time = stfKeyframe.frame / STFAnimation.fps,
								value = values[subtrackIndex],
								weightedMode = WeightedMode.None,
							};
							switch(stfKeyframe.interpolation_type)
							{
								case "bezier":
									keyframe.outTangent = stfKeyframe.out_tangent.x > 0 ? -(outTangentValues[subtrackIndex] / stfKeyframe.out_tangent.x) * STFAnimation.fps : 0;
									keyframe.outWeight = stfKeyframe.out_tangent.x / keyframeDistanceRight;
									keyframe.weightedMode = WeightedMode.Out;
									break;
								case "constant":
									keyframe.outTangent = float.PositiveInfinity;
									break;
								case "linear":
									keyframe.outTangent = nextKeyframe != null && values[subtrackIndex] - nextValues[subtrackIndex] < 0 ? float.NegativeInfinity : float.PositiveInfinity;
									break;
							}
							if(prevKeyframe != null && prevKeyframe.interpolation_type == "bezier")
							{
								keyframe.inTangent = stfKeyframe.in_tangent.x < 0 ? -(inTangentValues[subtrackIndex] / stfKeyframe.in_tangent.x) * STFAnimation.fps : 0;
								keyframe.inWeight = -stfKeyframe.in_tangent.x / keyframeDistanceLeft;
								if(keyframe.weightedMode == WeightedMode.Out || keyframe.weightedMode == WeightedMode.Both)
									keyframe.weightedMode = WeightedMode.Both;
								else
									keyframe.weightedMode = WeightedMode.In;
							}

							curves[subtrackIndex].AddKey(keyframe);
						}
					}

					for (int curveIndex = 0; curveIndex < pathRet.PropertyNames.Count; curveIndex++)
					{
						//Debug.Log($"Curve: {pathRet.RelativePath} - {pathRet.PropertyNames[curveIndex]} ({pathRet.TargetType})");

						if (curves[curveIndex].keys.Length > 0)
							ret.SetCurve(pathRet.RelativePath, pathRet.TargetType, pathRet.PropertyNames[curveIndex], curves[curveIndex]);
					}
				}
			}

			if(STFAnimation.tracks_baked != null && STFAnimation.tracks_baked.Count > 0)
			{
				var importBaked = Context.ImportConfig.GetAndConfirmImportOption(STF_Animation.STF_TYPE, STFAnimation.STF_Id, STFAnimation.STF_Name, "import_baked", true);
				if(importBaked) handleTracks(STFAnimation.tracks_baked);
			}
			if(STFAnimation.tracks != null) handleTracks(STFAnimation.tracks);

			return (new() { ret }, new() { ret });
		}
	}
}
