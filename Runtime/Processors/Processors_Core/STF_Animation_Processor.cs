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

			foreach (var track in STFAnimation.tracks)
			{
				if (STFAnimation.AnimationRoot.PropertyConverter == null)
				{
					continue; //TODO Report Warning
				}
				var pathRet = STFAnimation.AnimationRoot.PropertyConverter.ConvertPropertyPath(STFAnimation.AnimationRoot, track.target);
				if (pathRet == null)
				{
					continue; //TODO Report Warning
				}

				var RelativePath = pathRet.RelativePath;
				var CurveType = pathRet.TargetType;
				var PropertyNames = pathRet.PropertyNames;
				var ConvertValueFunc = pathRet.ConvertValueFunc;

				if (!string.IsNullOrWhiteSpace(RelativePath) && CurveType != null && PropertyNames != null && PropertyNames.Count > 0)
				{
					var curves = new List<AnimationCurve>();
					for (int curveIndex = 0; curveIndex < PropertyNames.Count; curveIndex++)
					{
						curves.Add(new AnimationCurve());
					}

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
						convertedValues.Add(ConvertValueFunc != null ? ConvertValueFunc(originalValues) : originalValues);
						convertedInTangentValues.Add(ConvertValueFunc != null ? ConvertValueFunc(originalInTangentValues) : originalInTangentValues);
						convertedOutTangentValues.Add(ConvertValueFunc != null ? ConvertValueFunc(originalOutTangentValues) : originalOutTangentValues);
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

					for (int curveIndex = 0; curveIndex < PropertyNames.Count; curveIndex++)
					{
						//Debug.Log($"Curve: {RelativePath} - {PropertyNames[curveIndex]} ({CurveType})");

						if (curves[curveIndex].keys.Length > 0)
							ret.SetCurve(RelativePath, CurveType, PropertyNames[curveIndex], curves[curveIndex]);
					}
				}
				else
				{
					// TODO Report Warning
				}
			}

			return (new() { ret }, new() { ret });
		}
	}
}
