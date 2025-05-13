using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public class STF_Instance_Armature : STF_InstanceResource
	{
		public const string STF_TYPE = "stf.instance.armature";
		public override string STF_Type => STF_TYPE;

		public STF_Armature Armature;

		// TODO pose & mods
		[System.Serializable]
		public class Pose
		{
			public string TargetId;
			public Vector3 Translation = Vector3.zero;
			public Quaternion Rotation = Quaternion.identity;
			public Vector3 Scale = Vector3.one;
		}
		public List<Pose> Poses = new();
	}

	public class STF_Instance_Armature_Module : ISTF_Module
	{
		public string STF_Type => STF_Instance_Armature.STF_TYPE;

		public string STF_Kind => "instance";

		public int Priority => 0;

		public List<string> LikeTypes => new(){"instance.armature", "instance"};

		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(STF_Instance_Armature)};

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = (STF_Node)ContextObject;
			var ret = go.gameObject.AddComponent<STF_Instance_Armature>();
			go.Instance = ret;
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "STF Armature Instance");

			ret.Armature = (STF_Armature)Context.ImportResource((string)JsonResource["armature"], "data");

			var instance = Object.Instantiate(ret.Armature.gameObject);

			if(JsonResource.ContainsKey("pose"))
			{
				foreach((string id, var stfPose) in (JObject)JsonResource["pose"])
				{
					var bone = instance.GetComponentsInChildren<STF_Bone>().FirstOrDefault(b => b.STF_Id == id);
					var pose = new STF_Instance_Armature.Pose {
						TargetId = id,
						Translation = TRSUtil.ParseLocation((JArray)stfPose[0]),
						Rotation = TRSUtil.ParseRotation((JArray)stfPose[1]),
						Scale = TRSUtil.ParseScale((JArray)stfPose[2])
					};
					ret.Poses.Add(pose);
					bone.transform.SetLocalPositionAndRotation(pose.Translation, pose.Rotation);
					bone.transform.localScale = pose.Scale;
				}
			}


			foreach(var bone in instance.GetComponentsInChildren<STF_Bone>())
			{
				bone.STF_Owner = go.gameObject;
			}
			for(var child_index = 0; child_index < instance.transform.childCount; child_index++)
			{
				instance.transform.GetChild(child_index).SetParent(go.transform, false);
			}

			// TODO also handle component mods and stuff

			#if UNITY_EDITOR
			Object.DestroyImmediate(instance);
			#else
			Object.Destroy(instance);
			#endif

			return (ret, null);
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			var PrefabObject = ApplicationObject as STF_Instance_Armature;
			var ret = new JObject {
				{"type", STF_Type},
				{"name", PrefabObject.STF_Name},
			};

			return (ret, PrefabObject.STF_Id);
		}
	}
}
