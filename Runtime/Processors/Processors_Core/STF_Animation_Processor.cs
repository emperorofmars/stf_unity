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

			// TODO figure out if this is actually how it works
			var tangentWeightNormalizeFactor = System.Math.Max(1, STFAnimation.range_end - STFAnimation.range_start);

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

					foreach (var stfKeyframe in track.keyframes)
					{
						var originalValues = new List<float>(new float[PropertyNames.Count]);
						for (int curveIndex = 0; curveIndex < PropertyNames.Count; curveIndex++) originalValues[curveIndex] = stfKeyframe.values[curveIndex] != null ? stfKeyframe.values[curveIndex].value : 0;
						var values = ConvertValueFunc != null ? ConvertValueFunc(originalValues) : originalValues;

						for (int curveIndex = 0; curveIndex < PropertyNames.Count; curveIndex++)
						{
							if (stfKeyframe.values[curveIndex] != null)
								curves[curveIndex].AddKey(new Keyframe
								{
									time = stfKeyframe.frame / STFAnimation.fps,
									value = values[curveIndex],
									inTangent = stfKeyframe.values[curveIndex].in_tangent.x < 0 ? -stfKeyframe.values[curveIndex].in_tangent.y * (1 / -stfKeyframe.values[curveIndex].in_tangent.x) : 0,
									inWeight = stfKeyframe.values[curveIndex].in_tangent.magnitude / tangentWeightNormalizeFactor,
									outTangent = stfKeyframe.values[curveIndex].out_tangent.x < 0 ? -stfKeyframe.values[curveIndex].out_tangent.y * (1 / stfKeyframe.values[curveIndex].out_tangent.x) : 0,
									outWeight = stfKeyframe.values[curveIndex].out_tangent.magnitude / tangentWeightNormalizeFactor,
								});
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
