#if UNITY_EDITOR
#if STF_AVA_UNIVRM0_FOUND

using System.Collections.Generic;
using com.squirrelbite.stf_unity.serialization;
using Newtonsoft.Json.Linq;
using UnityEditor;
using VRM;

namespace com.squirrelbite.stf_unity.ava.univrm0.serialization
{
	public class VRM_Springbone_Serializer : IUnity_Serializer
	{
		public static readonly System.Type _Target = typeof(VRMSpringBone);
		public System.Type Target => _Target;

		public List<SerializerResult> Serialize(UnitySerializerContext Context, UnityEngine.Object UnityObject)
		{
			if(UnityObject is VRMSpringBone springbone)
			{
				var IsComplete = true;

				var retJson = new JObject {
					{ "type", VRM_Springbone._STF_Type },
					{ "stiffness", springbone.m_stiffnessForce },
					{ "gravityPower", springbone.m_gravityPower },
					{ "gravityDir", TRSUtil.SerializeVector3(springbone.m_gravityDir) },
					{ "dragForce", springbone.m_dragForce },
					// todo center
					{ "hitRadius", springbone.m_hitRadius },
					// todo colliders
					{ "enabled", springbone.enabled },
				};

				return new List<SerializerResult> {new(){
					STFType = VRM_Springbone._STF_Type,
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
			Unity_Serializer_Registry.RegisterSerializer(new VRM_Springbone_Serializer());
		}
	}
}

#endif
#endif
