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
				wrapMode = STFAnimation.loop ? WrapMode.Loop : WrapMode.Default
			};

			foreach (var track in STFAnimation.tracks)
			{
				if (STFAnimation.AnimationRoot.PropertyConverter == null)
				{
					continue; //TODO Report Warning
				}
				(string RelativePath, System.Type CurveType, List<string> PropertyNames, System.Func<List<float>, List<float>> ConvertValueFunc) = STFAnimation.AnimationRoot.PropertyConverter.ConvertPropertyPath(STFAnimation.AnimationRoot, track.target);

				if (!string.IsNullOrWhiteSpace(RelativePath) && CurveType != null && PropertyNames != null && PropertyNames.Count > 0)
				{
					var curves = new List<AnimationCurve>();
					for (int curveIndex = 0; curveIndex < PropertyNames.Count; curveIndex++)
					{
						curves.Add(new AnimationCurve());
					}

					var len = -1;
					foreach(var subtrack in track.subtracks) if(subtrack != null) if(subtrack.keyframes.Count > len) len = subtrack.keyframes.Count;
					if(len <= 0) continue; // error

					for (int i = 0; i < len; i++)
					{
						var originalValues = new List<float>(new float[track.subtracks.Count]);
						for(int subtrackIndex = 0; subtrackIndex < track.subtracks.Count; subtrackIndex++)
							if(track.subtracks[subtrackIndex] != null && track.subtracks[subtrackIndex].keyframes[i] is var stfKeyframe && stfKeyframe != null)
								originalValues[subtrackIndex] = stfKeyframe.value;
							else
								originalValues[subtrackIndex] = 0;
						var values = ConvertValueFunc != null ? ConvertValueFunc(originalValues) : originalValues;

						for(int subtrackIndex = 0; subtrackIndex < track.subtracks.Count; subtrackIndex++) if(track.subtracks[subtrackIndex] != null && track.subtracks[subtrackIndex].keyframes[i] is var stfKeyframe && stfKeyframe != null && stfKeyframe.source_of_truth)
						{
							var prevKeyframe = i > 0 ? track.subtracks[subtrackIndex].keyframes[i - 1] : null;
							var nextKeyframe = i < track.subtracks[subtrackIndex].keyframes.Count - 1 ? track.subtracks[subtrackIndex].keyframes[i + 1] : null;

							var keyframeDistanceLeft = prevKeyframe != null ? System.Math.Abs(prevKeyframe.frame - stfKeyframe.frame) : 1;
							var keyframeDistanceRight = nextKeyframe != null ? System.Math.Abs(nextKeyframe.frame - stfKeyframe.frame) : 1;

							// todo handle interpolation type
							curves[subtrackIndex].AddKey(new Keyframe
							{
								time = stfKeyframe.frame / STFAnimation.fps,
								value = values[subtrackIndex],
								inTangent = stfKeyframe.in_tangent.x < 0 ? (stfKeyframe.in_tangent.y / stfKeyframe.in_tangent.x) * STFAnimation.fps : 0,
								inWeight = stfKeyframe.in_tangent.magnitude / STFAnimation.fps,
								outTangent = stfKeyframe.out_tangent.x > 0 ? (stfKeyframe.out_tangent.y / stfKeyframe.out_tangent.x) * STFAnimation.fps : 0,
								outWeight = stfKeyframe.out_tangent.magnitude / STFAnimation.fps,
								weightedMode = WeightedMode.Both,
							});
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
