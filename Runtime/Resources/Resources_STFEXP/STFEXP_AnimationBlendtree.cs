using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.handlers.stfexp
{
	[CreateAssetMenu(menuName = "STF/Resources/stfexp/stfexp.animation_blendtree")]
	[HelpURL("https://docs.stfform.at/modules/stfexp/stfexp_animation_blendtree.html")]
	public class STFEXP_AnimationBlendtree : STF_DataResource
	{
		public const string _STF_Type = "stfexp.animation_blendtree";
		public override string STF_Type => _STF_Type;

		[System.Serializable]
		public class AnimationMapping
		{
			public Vector2 Position;
			public STF_Animation Animation;
		}

		public string blendtree_type = "2d";
		public List<AnimationMapping> Animations = new();
	}

	public class STFEXP_AnimationBlendtree_Handler : ISTF_Handler
	{
		public string STF_Type => STFEXP_AnimationBlendtree._STF_Type;
		public string STF_Category => "data";
		public int Priority => 1;
		public List<string> LikeTypes => new(){"animation_blendtree", "blendtree"};
		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(STFEXP_AnimationBlendtree)};
		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }
		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var ret = ScriptableObject.CreateInstance<STFEXP_AnimationBlendtree>();
			ret.SetFromJson(JsonResource, STF_Id, "STFEXP Animation Blendtree");

			if(JsonResource.ContainsKey("blendtree_type")) ret.blendtree_type = JsonResource.Value<string>("blendtree_type");

			if(JsonResource.ContainsKey("animations") && JsonResource["animations"] is JArray jsonAnimations) foreach(JObject jsonAnimation in jsonAnimations.Cast<JObject>())
			{
				var position = new Vector2((float)jsonAnimation["position"][0], ret.blendtree_type == "2d" ? (float)jsonAnimation["position"][1] : 0f);
				ret.Animations.Add(new STFEXP_AnimationBlendtree.AnimationMapping {
					Position = position,
					Animation = STFUtil.ImportResource(Context, JsonResource, jsonAnimation.Value<int>("animation")) as STF_Animation,
				});
			}

			return (ret, new() {ret});
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			throw new System.NotImplementedException();
		}
	}
}
