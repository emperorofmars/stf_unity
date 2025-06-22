#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.modules.stfexp;
using com.squirrelbite.stf_unity.serialization;
using Newtonsoft.Json.Linq;
using UnityEditor;
using VRC.SDK3.Dynamics.Constraint.Components;

namespace com.squirrelbite.stf_unity.ava.vrchat.serialization
{
	public class VRC_Twist_Serializer : IUnity_Serializer
	{
		public static readonly System.Type _Target = typeof(VRCRotationConstraint);
		public System.Type Target => _Target;

		public List<SerializerResult> Serialize(UnitySerializerContext Context, UnityEngine.Object UnityObject)
		{
			if(UnityObject is VRCRotationConstraint c && c.AffectsRotationY == true && c.AffectsRotationX == false && c.AffectsRotationZ == false && c.Sources.Count == 1)
			{
				var source = c.Sources[0].SourceTransform;
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
					{ "weight", c.GlobalWeight }
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

	[InitializeOnLoad]
	public class Register_VRC_Twist_Serializer
	{
		static Register_VRC_Twist_Serializer()
		{
			Unity_Serializer_Registry.RegisterSerializer(new VRC_Twist_Serializer());
		}
	}
}

#endif
#endif
