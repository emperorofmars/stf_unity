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

				if (!string.IsNullOrWhiteSpace(RelativePath) && PropertyNames != null && PropertyNames.Count > 0)
				{
					var curves = new List<AnimationCurve>();
					for (int curveIndex = 0; curveIndex < PropertyNames.Count; curveIndex++)
					{
						curves.Add(new AnimationCurve());
					}

					//foreach (var stfKeyframe in track.keyframes)
					for(int i = 0; i < track.keyframes.Count; i++)
					{
						var stfKeyframe = track.keyframes[i];
						var originalValues = new List<float>(new float[PropertyNames.Count]);
						for (int curveIndex = 0; curveIndex < PropertyNames.Count; curveIndex++) originalValues[curveIndex] = stfKeyframe.values[curveIndex] != null ? stfKeyframe.values[curveIndex].value : 0;
						var values = ConvertValueFunc != null ? ConvertValueFunc(originalValues) : originalValues;

						for (int curveIndex = 0; curveIndex < PropertyNames.Count; curveIndex++)
						{
							var prevKeyframe = i > 0 ? track.keyframes[i - 1] : null;
							var nextKeyframe = i < track.keyframes.Count - 1 ? track.keyframes[i + 1] : null;

							var keyframeDistanceLeft = prevKeyframe != null ? System.Math.Abs(prevKeyframe.frame - stfKeyframe.frame) : 1;
							var keyframeDistanceRight = nextKeyframe != null ? System.Math.Abs(nextKeyframe.frame - stfKeyframe.frame) : 1;

							var k = stfKeyframe.values[curveIndex];

							if (k != null && !k.isBaked)
							{
								/*curves[curveIndex].AddKey(new Keyframe
								{
									time = stfKeyframe.frame / STFAnimation.fps,
									value = values[curveIndex],
									inTangent = k.in_tangent.x < 0 ? k.in_tangent.y / k.in_tangent.x : 0,
									inWeight = k.in_tangent.magnitude / keyframeDistanceLeft,
									outTangent = k.out_tangent.x > 0 ? k.out_tangent.y / k.out_tangent.x : 0,
									outWeight = k.out_tangent.magnitude / keyframeDistanceRight,
									weightedMode = WeightedMode.Both,
								});*/

								curves[curveIndex].AddKey(new Keyframe
								{
									time = stfKeyframe.frame / STFAnimation.fps,
									value = values[curveIndex],
									inTangent = -k.in_tangent.y,
									inWeight = k.in_tangent.magnitude / keyframeDistanceLeft,
									outTangent = k.out_tangent.y,
									outWeight = k.out_tangent.magnitude / keyframeDistanceRight,
									weightedMode = WeightedMode.Both,
								});
							}
						}
					}

					for (int curveIndex = 0; curveIndex < PropertyNames.Count; curveIndex++)
					{
						//Debug.Log($"Curve: {RelativePath} - {PropertyNames[curveIndex]} ({CurveType})");

						if (RelativePath != null && CurveType != null && PropertyNames != null)
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
