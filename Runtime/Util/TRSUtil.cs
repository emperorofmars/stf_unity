using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	public static class TRSUtil
	{
		public static Vector3 ParseLocation(JObject Json)
		{
			return new Vector3(-(float)Json["trs"][0][0], (float)Json["trs"][0][1], (float)Json["trs"][0][2]); // Flip the X-axis to convert from the glTF coordinate system
		}
		public static Vector3 ParseLocation(JArray Json)
		{
			return new Vector3(-(float)Json[0], (float)Json[1], (float)Json[2]);
		}
		public static Quaternion ParseRotation(JObject Json)
		{
			return new Quaternion((float)Json["trs"][1][0], -(float)Json["trs"][1][1], -(float)Json["trs"][1][2], (float)Json["trs"][1][3]); // Flipping the X-axis with quats is funny
		}
		public static Quaternion ParseRotation(JArray Json)
		{
			return new Quaternion((float)Json[0], -(float)Json[1], -(float)Json[2], (float)Json[3]); // Flipping the X-axis with quats is funny
		}
		public static Vector3 ParseScale(JObject Json)
		{
			return new Vector3((float)Json["trs"][2][0], (float)Json["trs"][2][1], (float)Json["trs"][2][2]);
		}
		public static Vector3 ParseScale(JArray Json)
		{
			return new Vector3((float)Json[0], (float)Json[1], (float)Json[2]);
		}
		public static void ParseTRS(Transform Target, JObject Json)
		{
			Target.SetLocalPositionAndRotation(TRSUtil.ParseLocation(Json), TRSUtil.ParseRotation(Json));
			Target.localScale = TRSUtil.ParseScale(Json);
		}
		public static void ParseTRS(GameObject Target, JObject Json)
		{
			ParseTRS(Target.transform, Json);
		}

		public static Vector3 ParseVector3(JArray Json)
		{
			return new Vector3(-(float)Json[0], (float)Json[1], (float)Json[2]);
		}
		public static Quaternion ParseQuat(JArray Json)
		{
			return new Quaternion((float)Json[0], -(float)Json[1], -(float)Json[2], (float)Json[3]);
		}


		public static JArray SerializeLocation(Transform T)
		{
			return new JArray { -T.localPosition.x, T.localPosition.y, T.localPosition.z }; // Flip the X-axis to convert to the glTF coordinate system
		}
		public static JArray SerializeRotation(Transform T)
		{
			return new JArray { T.transform.localRotation.x, -T.transform.localRotation.y, -T.transform.localRotation.z, T.transform.localRotation.w }; // Flipping the X-axis with quats is funny
		}
		public static JArray SerializeScale(Transform T)
		{
			return new JArray { T.transform.localScale.x, T.transform.localScale.y, T.transform.localScale.z };
		}
		public static JArray SerializeTRS(Transform T)
		{
			return new JArray { SerializeLocation(T), SerializeRotation(T), SerializeScale(T) };
		}
		public static JArray SerializeTRS(GameObject Go)
		{
			return SerializeTRS(Go.transform);
		}

		public static JArray SerializeVector3(Vector3 T)
		{
			return new JArray {-T.x, T.y, T.z}; // Flip the X-axis to convert to the glTF coordinate system
		}
		public static JArray SerializeQuat(Quaternion T)
		{
			return new JArray {T.x, -T.y, -T.z, T.w}; // Flipping the X-axis with quats is funny
		}
	}
}
