using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.stfexp;
using Newtonsoft.Json.Linq;
using UnityEngine.Animations;

namespace com.squirrelbite.stf_unity.serialization
{
	public class NNA_Twist_Serializer : IUnity_Serializer
	{
		public static readonly System.Type _Target = typeof(RotationConstraint);
		public System.Type Target => _Target;

		public List<SerializerResult> Serialize(UnitySerializerContext Context, UnityEngine.Object UnityObject)
		{
			if(UnityObject is RotationConstraint c && c.rotationAxis == Axis.Y && c.sourceCount == 1)
			{
				var source = c.GetSource(0).sourceTransform;
				var targetNode = source.gameObject.GetComponent<STF_NodeResource>();
				var node = c.gameObject.GetComponent<STF_NodeResource>();
				var target = new JArray();
				var IsComplete = true;
				if (targetNode)
				{
					if (targetNode is STF_Bone tb && node is STF_Bone b && tb.STF_Owner == node.STF_Owner)
					{
						target.Add(tb.STF_Id);
					}
					else if (targetNode is STF_Node tn)
					{
						target.Add(tn.STF_Id);
					}
					else if (targetNode is STF_Bone tb1)
					{
						target.Add(tb1.STF_Owner.STF_Id);
						target.Add("instance");
						target.Add(tb1.STF_Id);
					}
					else
					{
						IsComplete = false;
					}
				}
				else
				{
					IsComplete = false;
				}

				var retJson = new JObject {
					{ "type", STFEXP_Constraint_Twist._STF_Type },
					{ "weight", c.weight }
				};
				
				if (target.Count > 0)
				{
					retJson.Add("target", target);
				}

				return new List<SerializerResult> {new(){
					STFType = STFEXP_Constraint_Twist._STF_Type,
					Result = retJson,
					Origin = UnityObject,
					IsComplete = IsComplete,
					Confidence = SerializerResultConfidenceLevel.MANUAL,
				}};
			}
			else return null;
		}
	}
}